using System;

namespace ACSSandbox.AreaServiceProtocol
{
    public interface IServerMessageSerializer
    {
        ReadOnlySpan<byte> Serialize<MessageType>(MessageType message) where MessageType : IMessage;
        void DeserializedDispatch( ReadOnlySpan<byte> data );

        void RegisterServerMessageDispatch<MessageType>(MessageTypeId messageTypeId, Action<MessageType> dispatch) where MessageType : IMessage;
    }
}