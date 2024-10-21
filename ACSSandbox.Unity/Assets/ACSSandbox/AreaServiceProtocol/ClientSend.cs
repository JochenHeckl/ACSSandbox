using ACSSandbox.AreaServiceProtocol.ClientToServer;
using ACSSandbox.Common.Network;

namespace ACSSandbox.AreaServiceProtocol
{
    public class ClientSend
    {
        private readonly IClientMessageSerializer<NetworkId> serializer;
        private readonly INetworkClient networkClient;

        public ClientSend(
            INetworkClient networkClient,
            IClientMessageSerializer<NetworkId> serializer
        )
        {
            this.serializer = serializer;
            this.networkClient = networkClient;
        }

        public void Unreliable<MessageType>(MessageType message)
            where MessageType : IMessage
        {
            networkClient.Send(serializer.Serialize(message), TransportChannel.Unreliable);
        }

        public void Reliable<MessageType>(MessageType message)
            where MessageType : IMessage
        {
            networkClient.Send(serializer.Serialize(message), TransportChannel.Reliable);
        }

        public void ReliableInOrder<MessageType>(MessageType message)
            where MessageType : IMessage
        {
            networkClient.Send(serializer.Serialize(message), TransportChannel.ReliableInOrder);
        }
    }
}
