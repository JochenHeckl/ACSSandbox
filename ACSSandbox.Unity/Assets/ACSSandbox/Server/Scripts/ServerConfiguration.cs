using System;
using ACSSandbox.Common.Network;

namespace ACSSandbox.Server
{
    [Serializable]
    public class ServerConfiguration
    {
        public NetworkServerConfiguration networkServerConfiguration = new();

        public float HeartBeatIntervalSec { get; set; } = 2f;
    }
}
