using ACSSandbox.AreaServiceProtocol.ServerToClient;
using ACSSandbox.Common.Network;

namespace ACSSandbox.AreaServiceProtocol
{
    public class ServerSend
    {
        private readonly IAreaServiceProtocolSerializer serializer;
        private readonly INetworkServer networkServer;

        public ServerSend(IAreaServiceProtocolSerializer serializer, INetworkServer networkServer)
        {
            this.serializer = serializer;
            this.networkServer = networkServer;
        }

        public void ServerHeartBeat(float serverTimeSec)
        {
            var messageBytes = serializer.Serialize(
                new ServerHeartBeat()
                {
                    serverTimeSec = serverTimeSec
                }
            );

            networkServer.Broadcast(messageBytes, TransportChannel.Unreliable);
        }

        public void LoginResult(NetworkId networkId, LoginResultType result)
        {
            var messageBytes = serializer.Serialize(new LoginResult() { result = result });

            networkServer.Send(networkId, messageBytes, TransportChannel.ReliableInOrder);
        }
    }
}
