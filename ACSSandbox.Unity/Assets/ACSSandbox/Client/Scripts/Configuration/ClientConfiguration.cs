using System;

namespace ACSSandbox.Client
{
    [Serializable]
    public class ClientConfiguration
    {
        public float clientHeartBeatIntervalSec = 2f;
        public int areaServerPort = 1337;
        public string clientId = Guid.NewGuid().ToString("N");
    }
}
