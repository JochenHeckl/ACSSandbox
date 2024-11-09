using UnityEngine;

namespace ACSSandbox.Server
{
    public class ServerRuntimeData
    {
        public ServerConfiguration Configuration { get; set; }
        public Transform WorldRootTransform { get; set; }

        public NetworkServerData NetworkServerData { get; } = new();
    }
}
