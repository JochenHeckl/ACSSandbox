using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ACSSandbox.Common.Network
{
    public interface INetworkServer
    {
        public IEnumerable<NetworkId> Connections { get; }

        Task RunServerAsync(
            int servicePort,
            INetworkServerEventProcessor eventProcessor,
            CancellationToken cancellationToken
        );

        void Send(NetworkId networkId, ReadOnlySpan<byte> data, TransportChannel channel);
        void Send(NetworkId[] networkIds, ReadOnlySpan<byte> data, TransportChannel channel);
        void Broadcast(ReadOnlySpan<byte> data, TransportChannel channel);
    }
}
