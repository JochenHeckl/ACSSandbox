using ACSSandbox.AreaServiceProtocol;
using ACSSandbox.AreaServiceProtocol.ClientToServer;
using ACSSandbox.AreaServiceProtocol.ServerToClient;
using ACSSandbox.Common.Network;
using Unity.Logging;
using UnityEngine;

namespace ACSSandbox.Client.System
{
    public class HeartBeatSystem : IClientSystem
    {
        private readonly ClientRuntimeData runtimeData;
        private readonly INetworkClient networkClient;

        private float nextHeartBeatSec;
        private ClientSend Send;

        public HeartBeatSystem(ClientRuntimeData runtimeData, INetworkClient networkClient)
        {
            this.runtimeData = runtimeData;
            this.networkClient = networkClient;
        }

        public void Start()
        {
            nextHeartBeatSec = Time.time;
        }

        public void Update() { }

        public void FixedUpdate()
        {
            if (Time.time < nextHeartBeatSec)
            {
                return;
            }

            nextHeartBeatSec = Time.time + runtimeData.Configuration.clientHeartBeatIntervalSec;

            Send.Unreliable(new ClientHeartBeat() { clientTimeSec = runtimeData.ServerTimeSec });
        }

        public void Stop() { }

        private void HandleServerHeartBeat(ServerHeartBeat message)
        {
            Log.Debug("Client received server time {ServerTime:0.00}", message.serverTimeSec);
            runtimeData.ServerTimeSec = message.serverTimeSec;
        }
    }
}
