using System;
using ACSSandbox.AreaServiceProtocol.ClientToServer;
using ACSSandbox.Common.Network;

namespace ACSSandbox.AreaServiceProtocol
{
    public delegate void ClientHeartBeatHandler(NetworkId networkId, float clientTimeSec);
    public delegate void LoginRequestHandler(NetworkId networkId, string secret);

    public class ServerReceive
    {
        private readonly IServerMessageSerializer<NetworkId> serializer;

        ServerReceive(IServerMessageSerializer<NetworkId> serializer)
        {
            this.serializer = serializer;
        }

        public void RegisterMessageHandler<Message>(
            MessageTypeId messageTypeId,
            Action<NetworkId, Message> handler
        )
            where Message : IMessage
        {
            serializer.RegisterClientMessageDispatch(messageTypeId, handler);
        }

        public void HandleInboundData(NetworkId networkId, ReadOnlySpan<byte> inboundData)
        {
            serializer.DeserializedDispatch(networkId, inboundData);
        }

        private static void DiscardMessage(NetworkId networkId, IMessage message)
        {
            throw new InvalidProgramException(
                $"You received a message of type {message.GetType().Name} from networkId {networkId} but there was no dispatcher registered to handle it."
            );
        }
    }
}
