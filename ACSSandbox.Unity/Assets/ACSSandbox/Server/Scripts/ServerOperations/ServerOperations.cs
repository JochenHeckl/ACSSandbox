using System;
using ACSSandbox.Common.Network;
using Unity.Logging;

namespace ACSSandbox.Server
{
    public partial class ServerOperations : IServerSystem
    {
        private readonly INetworkServer networkServer;
        private readonly ServerRuntimeData runtimeData;

        public ServerOperations(ServerRuntimeData runtimeData, INetworkServer networkServer)
        {
            this.runtimeData = runtimeData;
            this.networkServer = networkServer;
        }

        public void Start()
        {
            var config = runtimeData.Configuration;
            networkServer.StartServer(config.areaServerPort, this);
        }

        public void Stop()
        {
            networkServer.StopServer();
        }

        public void FixedUpdate()
        {
            networkServer.ProcessEvents();
        }
    }
}
