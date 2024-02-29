using System.Collections.Concurrent;
using System.Collections.Generic;
using ACSSandbox.Common.Network;
using UnityEngine;

namespace ACSSandbox.Common.Networking.NetworkSimulator
{
    public partial class NetworkSimulator : MonoBehaviour
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
