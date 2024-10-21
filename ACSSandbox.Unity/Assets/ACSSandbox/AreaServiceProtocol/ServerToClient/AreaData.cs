using MemoryPack;

namespace ACSSandbox.AreaServiceProtocol.ServerToClient
{
    [MemoryPackable]
    public partial struct AreaData : IMessage
    {
        public readonly MessageTypeId MessageTypeId => MessageTypeId.AreaData;
        
        public string areaId;
        public int numberOfConnectedClients; 
    }
}
