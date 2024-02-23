using MemoryPack;

namespace ACSSandbox.AreaServiceProtocol.ClientToServer
{
    [MemoryPackable]
    public partial struct LoginRequest : IMessage
    {
        public MessageTypeId MessageTypeId => MessageTypeId.LoginRequest;

        public string secret;
    }
}
