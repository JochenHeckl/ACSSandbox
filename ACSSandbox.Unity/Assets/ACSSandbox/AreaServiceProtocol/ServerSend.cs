using ACSSandbox.AreaServiceProtocol.ServerToClient;
using ACSSandbox.Common.Network;

namespace ACSSandbox.AreaServiceProtocol
{
    public class ServerSend
    {
        private readonly INetworkServer networkServer;
        private readonly IServerMessageSerializer<NetworkId> serializer;

        public ServerSend(INetworkServer networkServer)
        {
            this.networkServer = networkServer;
            this.serializer = new ProtocolSerializerMemoryPack<NetworkId>(false);
        }

        public void Send<MessageType>(
            NetworkId destination,
            MessageType message,
            TransportChannel channel
        )
            where MessageType : IMessage
        {
            networkServer.Send(destination, serializer.Serialize(message), channel);
        }

        public void Send<MessageType>(
            NetworkId[] destinations,
            MessageType message,
            TransportChannel channel
        )
            where MessageType : IMessage
        {
            networkServer.Send(destinations, serializer.Serialize(message), channel);
        }

        public void Broadcast<MessageType>(MessageType message, TransportChannel channel)
            where MessageType : IMessage
        {
            networkServer.Broadcast(serializer.Serialize(message), channel);
        }

        public void Unreliable<MessageType>(NetworkId destination, MessageType message)
            where MessageType : IMessage
        {
            networkServer.Send(
                destination,
                serializer.Serialize(message),
                TransportChannel.Unreliable
            );
        }

        public void Reliable<MessageType>(NetworkId destination, MessageType message)
            where MessageType : IMessage
        {
            networkServer.Send(
                destination,
                serializer.Serialize(message),
                TransportChannel.Reliable
            );
        }

        public void ReliableInOrder<MessageType>(NetworkId destination, MessageType message)
            where MessageType : IMessage
        {
            networkServer.Send(
                destination,
                serializer.Serialize(message),
                TransportChannel.ReliableInOrder
            );
        }
    }
}
