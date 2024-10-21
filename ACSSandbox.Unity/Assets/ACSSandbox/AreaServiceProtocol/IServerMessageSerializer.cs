using System;
using ACSSandbox.Common.Network;

namespace ACSSandbox.AreaServiceProtocol
{
    public interface IServerMessageSerializer<ClientIdType>
    {
        ReadOnlySpan<byte> Serialize<MessageType>(MessageType message)
            where MessageType : IMessage;
        void DeserializedDispatch(ReadOnlySpan<byte> data);

        void RegisterServerMessageDispatch<MessageType>(
            MessageTypeId messageTypeId,
            Action<MessageType> dispatch
        )
            where MessageType : IMessage;
        void RegisterClientMessageDispatch<Message>(
            MessageTypeId messageTypeId,
            Action<NetworkId, Message> handler
        )
            where Message : IMessage;
        void DeserializedDispatch(ClientIdType networkId, ReadOnlySpan<byte> inboundData);
    }
}
