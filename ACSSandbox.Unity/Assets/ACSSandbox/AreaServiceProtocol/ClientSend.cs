using ACSSandbox.AreaServiceProtocol.ClientToServer;
using ACSSandbox.Common.Network;

namespace ACSSandbox.AreaServiceProtocol
{
    public class ClientSend
    {
        private readonly ProtocolSerializerMemoryPack<NetworkId> serializer = new();
        private readonly INetworkClient networkClient;

        public ClientSend(INetworkClient networkClient)
        {
            this.networkClient = networkClient;
        }

        public void ClientHeartBeat(float clientTimeSec)
        {
            var message = serializer.Serialize(
                new ClientHeartBeat() { clientTimeSec = clientTimeSec }
            );

            networkClient.Send(message, TransportChannel.Unreliable);
        }

        public void LoginRequest(string secret)
        {
            var message = serializer.Serialize(new LoginRequest() { secret = secret });

            networkClient.Send(message, TransportChannel.ReliableInOrder);
        }
    }
}
