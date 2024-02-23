using MemoryPack;

namespace ACSSandbox.AreaServiceProtocol.ServerToClient
{
    [MemoryPackable]
    public partial struct ServerHeartBeat : IMessage
    {
        public MessageTypeId MessageTypeId => MessageTypeId.ServerHeartBeat;

        public float serverTimeSec;
    }
}
