using System;
using System.Collections.Generic;
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
        private NetworkStats networkStats = new();
        private INetworkServerEventProcessor eventProcessor;

        public void StartServer(int servicePort, INetworkServerEventProcessor eventProcessor)
        {
            this.eventProcessor = eventProcessor;

            var serverConfig = new SocketConfig()
            {
                ChallengeDifficulty = 20,
                DualListenPort = servicePort,
                ChannelTypes = NetworkSetup.ChannelTypes,
            };

            server = new(serverConfig);
            server.Start();
        }

        public void ProcessEvents()
        {
            if (!server.IsInitialized)
            {
                return;
            }

            try
            {
                while (true)
                {
                    var networkEvent = server.Poll();

                    if (networkEvent.Type == NetworkEventType.Nothing)
                    {
                        break;
                    }

                    var connection = networkEvent.Connection;
                    HandleNetworkEvent(networkEvent, eventProcessor);

                    networkEvent.Recycle();
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error while processing network messages.");
            }
        }

        public void StopServer()
        {
            if (server.IsInitialized)
            {
                server.Shutdown();
                server = null;
            }
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
                    networkStats.messagesReceived++;
                    networkStats.bytesReceived += (ulong)networkEvent.Data.Count;

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
