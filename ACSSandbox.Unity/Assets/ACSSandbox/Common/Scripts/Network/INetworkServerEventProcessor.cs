using System;

namespace ACSSandbox.Common.Network
{
    public interface INetworkServerEventProcessor
    {
        void HandleConnectionEstablished(NetworkId networkId);

        void HandleDisconnect(NetworkId networkId);

        void HandleInboundData(NetworkId networkId, ReadOnlySpan<byte> inboundData);
    }
}
