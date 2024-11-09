using System;
using System.Threading;
using System.Threading.Tasks;

namespace ACSSandbox.Common.Network
{
    public interface INetworkClient
    {
        void StartClient(string host, int servicePort, INetworkClientEventProcessor eventProcessor);
        void ProcessEvents();
        void StopClient();

        void Send(ReadOnlySpan<byte> data, TransportChannel channel);
    }
}
