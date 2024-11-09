using System;

namespace ACSSandbox.Server
{
    [Serializable]
    public class ServerConfiguration
    {
        public int areaServerPort = 1337;
        public float serverHeartBeatIntervalSec = 2f;
    }
}
