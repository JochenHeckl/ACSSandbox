using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ACSSandbox.Common.Network;
using UnityEngine;

namespace ACSSandbox.Common
{
    public partial class NetworkSimulator : MonoBehaviour, INetworkClient, INetworkServer
    {
        public IEnumerable<NetworkId> Connections { get; private set; }

        private ConcurrentQueue<byte[]> clientMessages;
        private ConcurrentQueue<byte[]> serverMessages;

        void Awake()
        {
            clientMessages = new();
            serverMessages = new();
        }
    }
}
