using System;
using System.Threading;
using System.Threading.Tasks;
using ACSSandbox.Common.Network;
using Unity.Logging;
using UnityEngine;

namespace ACSSandbox.Server
{
    public partial class ServerOperations : INetworkServerEventProcessor
    {
        public void StartupNetworkServer(CancellationToken cancellationToken)
        {
            Log.Info("Starting network server.");
        }

        public void ShutdownNetworkServer()
        {
            Log.Info("Shutting down network server.");
        }

        public void HandleConnectionEstablished(NetworkId networkId)
        {
            Log.Info("Client connected: {NetworkId}", networkId.ToString());

            runtimeData.NetworkServerData.UnauthorizedConnections.Add(networkId);
        }

        public void HandleDisconnect(NetworkId networkId)
        {
            Log.Info("Client disconnected: {NetworkId}", networkId.ToString());

            runtimeData.NetworkServerData.UnauthorizedConnections.Remove(networkId);
            runtimeData.NetworkServerData.AuthorizedConnections.Remove(networkId);
        }

        public void HandleInboundData(NetworkId networkId, ReadOnlySpan<byte> inboundData)
        {
            throw new NotImplementedException();
        }
    }
}
