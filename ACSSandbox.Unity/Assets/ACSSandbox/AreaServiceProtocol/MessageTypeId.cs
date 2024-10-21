namespace ACSSandbox.AreaServiceProtocol
{
    public enum MessageTypeId : byte
    {
        ClientHeartBeat,
        ServerHeartBeat,

        LoginRequest,
        LoginResult,
        
        AreaDataRequest,
        AreaData,
    }
}
