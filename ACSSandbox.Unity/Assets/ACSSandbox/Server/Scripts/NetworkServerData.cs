using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ACSSandbox.Common.Network;

namespace ACSSandbox.Server
{
    [Serializable]
    public class NetworkServerData
    {
        public HashSet<NetworkId> UnauthorizedConnections { get; } = new();
        public HashSet<NetworkId> AuthorizedConnections { get; } = new();
    }
}
