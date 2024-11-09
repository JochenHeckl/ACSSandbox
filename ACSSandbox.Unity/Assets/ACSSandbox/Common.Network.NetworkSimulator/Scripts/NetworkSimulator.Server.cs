using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ACSSandbox.Common.Network.NetworkSimulator
{
    public partial class NetworkSimulator : INetworkServer
    {
        private INetworkServerEventProcessor serverEventProcessor;

        public void StartServer(int servicePort, INetworkServerEventProcessor eventProcessor)
        {
            this.serverEventProcessor = eventProcessor;
        }

        public void StopServer() { }

        public void Send(NetworkId networkId, ReadOnlySpan<byte> data, TransportChannel channel)
        {
            serverMessages.Enqueue(data.ToArray());
        }

        public void Send(NetworkId[] networkIds, ReadOnlySpan<byte> data, TransportChannel channel)
        {
            serverMessages.Enqueue(data.ToArray());
        }

        public void Broadcast(ReadOnlySpan<byte> data, TransportChannel channel)
        {
            serverMessages.Enqueue(data.ToArray());
        }
    }
}
