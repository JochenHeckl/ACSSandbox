using System;

namespace ACSSandbox.AreaServiceProtocol
{
    public interface IAreaServiceProtocolSerializer
    {
        public ReadOnlySpan<byte> Serialize<MessageType>(MessageType message)
            where MessageType : IMessage;
        public (MessageTypeId typeId, IMessage message) Deserialize(ReadOnlySpan<byte> messageRaw);
    }
}
