using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

        private NetworkStats networkStats;
        private INetworkClientEventProcessor eventProcessor;

        public void StartClient(
            string host,
            int servicePort,
            INetworkClientEventProcessor eventProcessor
        )
        {
            this.eventProcessor = eventProcessor;

            var clientConfig = new SocketConfig()
            {
                // Port 0 means we get a port by the operating system
                DualListenPort = 0,
                ChannelTypes = NetworkSetup.ChannelTypes,
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
        }

        public void ProcessEvents()
        {
            if (!client.IsInitialized)
            {
                return;
            }

            try
            {
                while (true)
                {
                    NetworkEvent networkEvent = client.Poll();

                    if (networkEvent.Type == NetworkEventType.Nothing)
                    {
                        break;
                    }

                    ProcessEvent(networkEvent);
                    networkEvent.Recycle();
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error while processing network messages.");
            }
        }

        public void StopClient()
        {
            if (client.IsInitialized)
            {
                client.Shutdown();
                client = null;
            }
        }

        private void ProcessEvent(NetworkEvent networkEvent)
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

                    networkStats.messagesReceived++;
                    networkStats.bytesReceived += (ulong)networkEvent.Data.Count;

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
                    networkStats.messagesReceived++;
                    networkStats.bytesReceived += (ulong)networkEvent.Data.Count;

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
                    throw new ArgumentOutOfRangeException(nameof(networkEvent.Type));
            }
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
                    connection.Send(data.ToArray(), 0, false, networkStats.messagesSent);
                    break;
                case TransportChannel.Unreliable:
                    connection.Send(data.ToArray(), 1, false, networkStats.messagesSent);
                    break;
                case TransportChannel.ReliableInOrder:
                    connection.Send(data.ToArray(), 2, false, networkStats.messagesSent);
                    break;
                default:
                    throw new InvalidOperationException($"unhandled channel type {channel}.");
            }

            networkStats.messagesSent++;
            networkStats.bytesSent += (ulong)data.Length;
        }
    }
}
