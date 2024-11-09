using ACSSandbox.AreaServiceProtocol;
using ACSSandbox.AreaServiceProtocol.ServerToClient;
using ACSSandbox.Common.Network;
using UnityEngine;

namespace ACSSandbox.Server
{
    public class ServerHearBeatSystem : IServerSystem
    {
        private float nextHearBeatSec = Time.time;
        private readonly ServerRuntimeData runtimeData;
        private readonly INetworkServer networkServer;
        private ServerSend serverSend;

        public ServerHearBeatSystem(ServerRuntimeData runtimeData, INetworkServer networkServer)
        {
            this.runtimeData = runtimeData;
            this.networkServer = networkServer;
        }

        public void Start()
        {
            serverSend = new ServerSend(networkServer);
        }

        public void FixedUpdate()
        {
            if (Time.time < nextHearBeatSec)
            {
                return;
            }

            nextHearBeatSec += runtimeData.Configuration.serverHeartBeatIntervalSec;

            serverSend.Broadcast(
                new ServerHeartBeat() { serverTimeSec = Time.time },
                TransportChannel.Unreliable
            );
        }

        public void Stop()
        {
            serverSend = null;
        }
    }
}
