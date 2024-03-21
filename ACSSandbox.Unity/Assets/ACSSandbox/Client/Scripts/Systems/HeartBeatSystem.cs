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
        private readonly ClientConfiguration configuration;
        private readonly IAreaServiceConnection areaServiceConnection;

        private float nextHeartBeatSec;
        private bool isStarted;

        public HeartBeatSystem(
            ClientRuntimeData runtimeData,
            ClientConfiguration configuration,
            IAreaServiceConnection areaServiceConnection
        )
        {
            this.runtimeData = runtimeData;
            this.configuration = configuration;
            this.configuration = configuration;
            this.areaServiceConnection = areaServiceConnection;

            areaServiceConnection.Receive.HandleServerHeartBeat = HandleServerHeartBeat;

            isStarted = false;
        }

        public HeartBeatSystem(bool isStarted)
        {
            this.isStarted = isStarted;
        }

        public void Start()
        {
            nextHeartBeatSec = Time.time;
            isStarted = true;
        }

        public void Update()
        {
            if (!isStarted || (Time.time < nextHeartBeatSec))
            {
                return;
            }

            nextHeartBeatSec = Time.time + configuration.HeartBeatIntervalSec;
            areaServiceConnection.Send.Send(
                new ClientHeartBeat() { clientTimeSec = runtimeData.ServerTimeSec },
                TransportChannel.Unreliable
            );
        }

        public void Stop()
        {
            isStarted = false;
        }

        private void HandleServerHeartBeat(ServerHeartBeat message)
        {
            Log.Debug("Client received server time {ServerTime:0.00}", message.serverTimeSec);
            runtimeData.ServerTimeSec = message.serverTimeSec;
        }
    }
}
