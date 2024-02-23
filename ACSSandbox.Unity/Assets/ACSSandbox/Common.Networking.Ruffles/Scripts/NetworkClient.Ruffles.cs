using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ruffles.Channeling;
using Ruffles.Configuration;
using Ruffles.Connections;
using Ruffles.Core;
using Unity.Logging;

namespace ACSSandbox.Common.Network.Ruffles
{
    public class NetworkClientRuffles : INetworkClient
    {
        private Connection connection;
        private RuffleSocket client;

        private uint messagesSent = 0;
        private uint messagesReceived = 0;

        public async Task RunClientAsync(
            string host,
            int servicePort,
            INetworkClientEventProcessor eventProcessor,
            CancellationToken cancellationToken
        )
        {
            var clientConfig = new SocketConfig()
            {
                // Difficulty 20 is fairly hard
                ChallengeDifficulty = 20,

                // Port 0 means we get a port by the operating system
                DualListenPort = 0,

                ChannelTypes = new[]
                {
                    ChannelType.Reliable,
                    ChannelType.Unreliable,
                    ChannelType.ReliableOrdered,
                }
            };

            client = new RuffleSocket(clientConfig);
            client.Start();

            var ipv6Address = Dns.GetHostAddresses(host)
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetworkV6);
            var ipAddress = Dns.GetHostAddresses(host)
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

            var address =
                ipv6Address
                ?? ipAddress
                ?? throw new InvalidOperationException("Failed to resolve hostname.");

            connection = client.Connect(new IPEndPoint(address, servicePort));

            await RunClient(eventProcessor, cancellationToken);

            client.Stop();
        }

        public void Send(ReadOnlySpan<byte> data, TransportChannel channel)
        {
            if (connection == null)
            {
                throw new InvalidOperationException("Client is not connected.");
            }

            switch (channel)
            {
                case TransportChannel.Reliable:
                    connection.Send(data.ToArray(), 0, false, messagesSent);
                    break;
                case TransportChannel.Unreliable:
                    connection.Send(data.ToArray(), 1, false, messagesSent);
                    break;
                case TransportChannel.ReliableInOrder:
                    connection.Send(data.ToArray(), 2, false, messagesSent);
                    break;
                default:
                    throw new InvalidOperationException($"unhandled channel type {channel}.");
            }

            messagesSent++;
        }

        private async Task RunClient(
            INetworkClientEventProcessor eventProcessor,
            CancellationToken cancellationToken
        )
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                NetworkEvent networkEvent = client.Poll();

                if (networkEvent.Type != NetworkEventType.Nothing)
                {
                    switch (networkEvent.Type)
                    {
                        case NetworkEventType.Connect:
                            Log.Info("Ruffles socket connected to server.");
                            eventProcessor.HandleConnect();
                            break;
                        case NetworkEventType.Data:
                            Log.Debug(
                                "Ruffles socket inbound data {Bytes} bytes.",
                                networkEvent.Data.Count
                            );
                            messagesReceived++;
                            eventProcessor.HandleInboundData(
                                networkEvent.Data.Array.AsSpan(
                                    networkEvent.Data.Offset,
                                    networkEvent.Data.Count
                                )
                            );
                            break;
                        case NetworkEventType.Nothing:
                            break;
                        case NetworkEventType.Disconnect:
                            Log.Info("Ruffles socket disconnected from server.");
                            eventProcessor.HandleDisconnect();
                            break;
                        case NetworkEventType.Timeout:
                            Log.Info("Ruffles socket timeout.");
                            eventProcessor.HandleDisconnect();
                            break;
                        case NetworkEventType.UnconnectedData:
                            break;
                        case NetworkEventType.BroadcastData:
                            messagesReceived++;
                            eventProcessor.HandleInboundData(
                                networkEvent.Data.Array.AsSpan(
                                    networkEvent.Data.Offset,
                                    networkEvent.Data.Count
                                )
                            );
                            break;
                        case NetworkEventType.AckNotification:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    await Task.Yield();
                }

                networkEvent.Recycle();
            }
        }
    }
}
