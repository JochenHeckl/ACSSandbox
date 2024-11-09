using System.Collections.Concurrent;
using System.Collections.Generic;
using ACSSandbox.Common.Network;
using UnityEngine;

namespace ACSSandbox.Common.Network.NetworkSimulator
{
    public partial class NetworkSimulator : MonoBehaviour
    {
        public IEnumerable<NetworkId> Connections
        {
            get { yield return ClientNetworkId; }
        }
        public NetworkId ClientNetworkId { get; private set; } = NetworkId.Create();

        private ConcurrentQueue<byte[]> clientMessages;
        private ConcurrentQueue<byte[]> serverMessages;

        void Awake()
        {
            clientMessages = new();
            serverMessages = new();
        }

        public void ProcessEvents()
        {
            while (serverMessages?.TryDequeue(out var message) ?? false)
            {
                serverEventProcessor.HandleInboundData(ClientNetworkId, message);
            }

            while (clientMessages?.TryDequeue(out var message) ?? false)
            {
                clientEventProcessor.HandleInboundData(message);
            }
        }
    }
}
