using ACSSandbox.AreaServiceProtocol.ServerToClient;
using ACSSandbox.Common.Network;

namespace ACSSandbox.AreaServiceProtocol
{
    public class ServerSend
    {
        private readonly ProtocolSerializerMemoryPack<NetworkId> serializer = new();
        private readonly INetworkServer networkServer;

        public ServerSend(INetworkServer networkServer)
        {
            this.networkServer = networkServer;
        }

        public void Send<MessageType>( NetworkId destination, MessageType message, TransportChannel channel ) where MessageType : IMessage
        {
            networkServer.Send( destination, serializer.Serialize(message), channel);
        }

        public void Send<MessageType>( NetworkId[] destinations, MessageType message, TransportChannel channel ) where MessageType : IMessage
        {
            networkServer.Send( destinations, serializer.Serialize(message), channel);
        }

        public void Broadcast<MessageType>( MessageType message, TransportChannel channel ) where MessageType : IMessage
        {
            networkServer.Broadcast( serializer.Serialize(message), channel);
        }
    }
}
