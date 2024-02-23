using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ACSSandbox.AreaServiceProtocol;
using ACSSandbox.Common.Network;
using Unity.Logging;

namespace ACSSandbox.Server
{
    public partial class AreaService : IAreaService, INetworkServerEventProcessor
    {
        public ServerSend Send { get; private set; }
        public ServerReceive Receive { get; private set; }

        private readonly HashSet<NetworkId> authorizedConnections = new();
        private readonly HashSet<NetworkId> unauthorizedConnections = new();

        private readonly INetworkServer networkServer;
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private Task networkProcessingTask;

        public AreaService(
            INetworkServer networkServer,
            IAreaServiceProtocolSerializer messageSerializer
        )
        {
            cancellationTokenSource = new();

            this.networkServer = networkServer;

            Send = new ServerSend(messageSerializer, networkServer);
            Receive = new ServerReceive(messageSerializer)
            {
                HandleClientHeartBeat = HandleHeartBeat,
                HandleLoginRequest = HandleLoginRequest
            };
        }

        public void StartService(int areaServicePort)
        {
            if (networkProcessingTask != null)
            {
                throw new InvalidOperationException("Area Service is already started.");
            }

            networkProcessingTask = networkServer.RunServerAsync(
                areaServicePort,
                this,
                cancellationTokenSource.Token
            );
        }

        public Task StopService()
        {
            if (networkProcessingTask == null)
            {
                throw new InvalidOperationException(
                    $"network processing task is not running. Did you forget to call {nameof(StartService)}()?"
                );
            }

            cancellationTokenSource.Cancel();
            return networkProcessingTask;
        }

        public void HandleConnectionEstablished(NetworkId networkId)
        {
            unauthorizedConnections.Add(networkId);
        }

        public void HandleDisconnect(NetworkId networkId)
        {
            unauthorizedConnections.Remove(networkId);
            authorizedConnections.Remove(networkId);
        }

        public void HandleInboundData(NetworkId networkId, ReadOnlySpan<byte> data)
        {
            try
            {
                Receive.HandleInboundData(networkId, data);
            }
            catch (Exception exception)
            {
                Log.Error(
                    "Failed to handle inbound data from networkId {NetworkId}. {Exception}",
                    networkId.ToString(),
                    exception
                );
            }
        }
    }
}
