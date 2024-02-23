using MemoryPack;

namespace ACSSandbox.AreaServiceProtocol.ClientToServer
{
    [MemoryPackable]
    public partial struct ClientHeartBeat : IMessage
    {
        public MessageTypeId MessageTypeId => MessageTypeId.ClientHeartBeat;

        public float clientTimeSec;
    }
}
