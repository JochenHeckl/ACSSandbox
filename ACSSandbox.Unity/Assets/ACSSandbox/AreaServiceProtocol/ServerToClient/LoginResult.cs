using MemoryPack;

namespace ACSSandbox.AreaServiceProtocol.ServerToClient
{
    [MemoryPackable]
    public partial struct LoginResult : IMessage
    {
        public MessageTypeId MessageTypeId => MessageTypeId.LoginResult;
        public LoginResultType result;
    }
}
