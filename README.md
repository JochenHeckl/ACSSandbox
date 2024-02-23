# Authoritative Client Server Sandbox


## Things to investigate
- Investigate code generation to implement Send And Receive
- Investigate contained data types to reduce boilerplate in Receive code.
  For example:
  ```csharp
  public struct ClientHeartBeatData
  {
    public MessageTypeId MessageTypeId => MessageTypeId.ClientHeartBeat;
    public float clientTimeSec;
  }
  
  [MemoryPackable]
  public partial struct ClientHeartBeat : IMessage
  {
    public ClientHeartBeatData data;
  }```
- Investigate code generation to implement MemoryPackable from plain structs.