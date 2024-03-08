using ACSSandbox.AreaServiceProtocol.ServerToClient;
using ACSSandbox.Common.Network;

namespace ACSSandbox.AreaServiceProtocol
{
    public class ServerSend
    {
        private readonly ProtocolSerializerMemoryPack<NetworkId> serializer = new();
        private readonly INetworkServer networkServer;

        public ServerSend(INetworkServer networkServer)
        {
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
