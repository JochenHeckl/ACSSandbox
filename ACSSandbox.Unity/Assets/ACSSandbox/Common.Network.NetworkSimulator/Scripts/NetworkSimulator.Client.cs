using System;
using System.Threading;
using System.Threading.Tasks;

namespace ACSSandbox.Common.Network.NetworkSimulator
{
    public partial class NetworkSimulator : INetworkClient
    {
        private INetworkClientEventProcessor clientEventProcessor;

        public void StartClient(
            string host,
            int servicePort,
            INetworkClientEventProcessor eventProcessor
        )
        {
            this.clientEventProcessor = eventProcessor;

            eventProcessor.HandleConnect();
        }

        public void StopClient() { }

        public void Send(ReadOnlySpan<byte> message, TransportChannel channel)
        {
            clientMessages.Enqueue(message.ToArray());
        }
    }
}
