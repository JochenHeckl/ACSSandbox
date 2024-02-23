using System;
using System.Threading;
using System.Threading.Tasks;

namespace ACSSandbox.Common.Network
{
    public interface INetworkClient
    {
        void Send(ReadOnlySpan<byte> data, TransportChannel channel);

        Task RunClientAsync(
            string host,
            int servicePort,
            INetworkClientEventProcessor eventProcessor,
            CancellationToken cancellationToken
        );
    }
}
