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

        public void Send<MessageType>( MessageType message, TransportChannel channel ) where MessageType : IMessage
        {
            networkClient.Send( serializer.Serialize(message), channel);
        }
    }
}
