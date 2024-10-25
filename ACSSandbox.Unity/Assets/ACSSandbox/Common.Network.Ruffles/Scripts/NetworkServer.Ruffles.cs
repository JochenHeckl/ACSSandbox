using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ruffles.Channeling;
using Ruffles.Configuration;
using Ruffles.Connections;
using Ruffles.Core;
using Unity.Logging;

namespace ACSSandbox.Common.Network.Ruffles
{
    public class NetworkServerRuffles : INetworkServer
    {
        public IEnumerable<NetworkId> Connections => networkIdToConnectionMap.Keys;

        private Dictionary<NetworkId, Connection> networkIdToConnectionMap = new();
        private Dictionary<Connection, NetworkId> connectionToNetworkIdMap = new();
        private RuffleSocket server;

        private uint messagesSent = 0;
        private uint messagesReceived = 0;

        public async Task RunServerAsync(
            int servicePort,
            INetworkServerEventProcessor eventProcessor,
            CancellationToken cancellationToken
        )
        {
            var serverConfig = new SocketConfig()
            {
                ChallengeDifficulty = 20,
                DualListenPort = servicePort,
                ChannelTypes = new[]
                {
                    ChannelType.Reliable,
                    ChannelType.Unreliable,
                    ChannelType.ReliableOrdered,
                },
            };

            server = new(serverConfig);
            server.Start();

            await RunServer(eventProcessor, cancellationToken);

            server.Stop();
        }

        public void Send(NetworkId networkId, ReadOnlySpan<byte> data, TransportChannel channel)
        {
            var connection = networkIdToConnectionMap[networkId];

            if (connection == null)
            {
                throw new InvalidOperationException("Client is not connected.");
            }

            SendInternal(connection, data, channel);
        }

        public void Send(NetworkId[] networkIds, ReadOnlySpan<byte> data, TransportChannel channel)
        {
            foreach (var networkId in networkIds)
            {
                SendInternal(networkIdToConnectionMap[networkId], data, channel);
            }
        }

        public void Broadcast(ReadOnlySpan<byte> data, TransportChannel channel)
        {
            foreach (var connection in connectionToNetworkIdMap.Keys)
            {
                SendInternal(connection, data, channel);
            }
        }

        private async Task RunServer(
            INetworkServerEventProcessor eventProcessor,
            CancellationToken cancellationTokenSource
        )
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var networkEvent = server.Poll();

                    if (networkEvent.Type != NetworkEventType.Nothing)
                    {
                        var connection = networkEvent.Connection;

                        HandleNetworkEvent(networkEvent, eventProcessor);
                    }
                    else
                    {
                        await Task.Yield();
                    }

                    networkEvent.Recycle();
                }
                catch (Exception exception)
                {
                    Log.Error(exception, "Error while processing network messages.");
                    await Task.Yield();
                }
            }

            Log.Info("Exiting network server loop.");
        }

        private void HandleNetworkEvent(
            NetworkEvent networkEvent,
            INetworkServerEventProcessor eventProcessor
        )
        {
            switch (networkEvent.Type)
            {
                case NetworkEventType.Connect:
                    var newNetworkId = NetworkId.Create();
                    networkIdToConnectionMap[newNetworkId] = networkEvent.Connection;
                    connectionToNetworkIdMap[networkEvent.Connection] = newNetworkId;
                    break;

                case NetworkEventType.Data:
                    messagesReceived++;

                    if (
                        connectionToNetworkIdMap.TryGetValue(
                            networkEvent.Connection,
                            out var networkId
                        )
                    )
                    {
                        eventProcessor.HandleInboundData(
                            networkId,
                            networkEvent.Data.Array.AsSpan(
                                networkEvent.Data.Offset,
                                networkEvent.Data.Count
                            )
                        );
                    }
                    else
                    {
                        Log.Error(
                            "Discarding data message from {ClientId} because of unknown connection id.",
                            networkId
                        );
                    }
                    break;

                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.Disconnect:
                    if (
                        connectionToNetworkIdMap.TryGetValue(networkEvent.Connection, out networkId)
                    )
                    {
                        networkIdToConnectionMap.Remove(networkId);
                        eventProcessor.HandleDisconnect(networkId);
                    }
                    connectionToNetworkIdMap.Remove(networkEvent.Connection);
                    break;

                case NetworkEventType.Timeout:
                    if (
                        connectionToNetworkIdMap.TryGetValue(networkEvent.Connection, out networkId)
                    )
                    {
                        networkIdToConnectionMap.Remove(networkId);
                        eventProcessor.HandleDisconnect(networkId);
                    }
                    connectionToNetworkIdMap.Remove(networkEvent.Connection);
                    break;

                case NetworkEventType.UnconnectedData:
                    break;
                case NetworkEventType.BroadcastData:
                    break;
                case NetworkEventType.AckNotification:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SendInternal(
            Connection connection,
            ReadOnlySpan<byte> data,
            TransportChannel channel
        )
        {
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
    }
}
