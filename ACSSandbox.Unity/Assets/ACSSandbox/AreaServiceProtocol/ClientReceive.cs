using System;
using ACSSandbox.AreaServiceProtocol.ServerToClient;
using ACSSandbox.Common.Network;

namespace ACSSandbox.AreaServiceProtocol
{
    public delegate void ServerHeartBeatHandler(float serverTimeSec);
    public delegate void LoginResultHandler(LoginResultType result);

    public class ClientReceive
    {
        private readonly ProtocolSerializerMemoryPack<NetworkId> serializer = new();
        
        public ServerHeartBeatHandler HandleServerHeartBeat
        {
            set => serializer.RegisterServerMessageDispatch<ServerHeartBeat>(
                MessageTypeId.ServerHeartBeat,
                (x) => value(x.serverTimeSec) );
        }
        public LoginResultHandler HandleLoginResult
        {
            set => serializer.RegisterServerMessageDispatch<LoginResult>( 
                MessageTypeId.LoginResult,
                (x) => value(x.result) );
        }

        public void HandleInboundData(ReadOnlySpan<byte> inboundData)
        {
            serializer.DeserializedDispatch(inboundData);
        }
    }
}
