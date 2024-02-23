using System;
using ACSSandbox.Common.Network;

namespace ACSSandbox.Client
{
    [Serializable]
    public class ClientConfiguration
    {
        public AreaServiceConnectionConfiguration areaServiceConnectionConfiguration = new();

        public float HeartBeatIntervalSec { get; set; } = 2f;
    }
}
