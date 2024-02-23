using System;

namespace ACSSandbox.Common.Network
{
    public interface INetworkClientEventProcessor
    {
        void HandleConnect();
        void HandleDisconnect();

        void HandleInboundData(ReadOnlySpan<byte> inboundData);
    }
}
