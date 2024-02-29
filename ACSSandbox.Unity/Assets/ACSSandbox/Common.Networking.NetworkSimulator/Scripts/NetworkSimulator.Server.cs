using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ACSSandbox.Common.Network;

namespace ACSSandbox.Common.Networking.NetworkSimulator
{
    public partial class NetworkSimulator : INetworkServer
    {
        public async Task RunServerAsync(
            int servicePort,
            INetworkServerEventProcessor eventProcessor,
            CancellationToken cancellationToken
        )
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Yield();

                while (clientMessages?.TryDequeue(out var message) ?? false)
                {
                    eventProcessor.HandleInboundData(Connections.Single(), message);
                }
            }
        }

        public void Send(NetworkId networkId, ReadOnlySpan<byte> data, TransportChannel channel)
        {
            if (!Connections.Contains(networkId))
            {
                throw new InvalidOperationException("NetworkSimulator setup is corrupted");
            }

            serverMessages.Enqueue(data.ToArray());
        }

        public void Send(NetworkId[] networkIds, ReadOnlySpan<byte> data, TransportChannel channel)
        {
            if (Connections != networkIds)
            {
                throw new InvalidOperationException("NetworkSimulator setup is corrupted");
            }

            serverMessages.Enqueue(data.ToArray());
        }

        public void Broadcast(ReadOnlySpan<byte> data, TransportChannel channel)
        {
            serverMessages.Enqueue(data.ToArray());
        }
    }
}
