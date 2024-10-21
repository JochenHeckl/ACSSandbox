using MemoryPack;

namespace ACSSandbox.AreaServiceProtocol.ClientToServer
{
    [MemoryPackable]
    public partial struct AreaDataRequest : IMessage
    {
        public MessageTypeId MessageTypeId => MessageTypeId.AreaDataRequest;
    }
}
