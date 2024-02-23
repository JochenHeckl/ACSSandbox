using UnityEngine;
using Unity.Logging;

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
            areaServiceConnection.Send.ClientHeartBeat(runtimeData.ServerTimeSec);
        }

        public void Stop()
        {
            isStarted = false;
        }

        private void HandleServerHeartBeat(float serverTimeSec)
        {
            Log.Debug("Client received server time {ServerTime:0.00}", serverTimeSec);
            runtimeData.ServerTimeSec = serverTimeSec;
        }
    }
}
