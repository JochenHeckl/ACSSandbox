using MemoryPack;

namespace ACSSandbox.AreaServiceProtocol.ServerToClient
{
    [MemoryPackable]
    public partial struct LoginResult : IMessage
    {
        public readonly MessageTypeId MessageTypeId => MessageTypeId.LoginResult;

        public ValidateLoginResult result;
        public string areaId;
    }
}
