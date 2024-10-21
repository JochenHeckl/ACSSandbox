using System;

namespace ACSSandbox.AreaServiceProtocol
{
    public interface IClientMessageSerializer<ClientIdType>
    {
        ReadOnlySpan<byte> Serialize<MessageType>(MessageType message)
            where MessageType : IMessage;

        void DeserializedDispatch(ClientIdType sourceId, ReadOnlySpan<byte> data);
        void RegisterClientMessageDispatch<MessageType>(
            MessageTypeId messageTypeId,
            Action<ClientIdType, MessageType> dispatch
        )
            where MessageType : IMessage;
        void RegisterServerMessageDispatch<Message>(
            MessageTypeId messageTypeId,
            Action<Message> handler
        )
            where Message : IMessage;
        void DeserializedDispatch(ReadOnlySpan<byte> inboundData);
    }
}
