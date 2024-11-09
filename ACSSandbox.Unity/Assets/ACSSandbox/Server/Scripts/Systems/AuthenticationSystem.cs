using System.Collections.Generic;
using ACSSandbox.AreaServiceProtocol;
using ACSSandbox.AreaServiceProtocol.ClientToServer;
using ACSSandbox.AreaServiceProtocol.ServerToClient;
using ACSSandbox.Common.Network;
using Unity.Logging;

namespace ACSSandbox.Server
{
    public class AuthenticationSystem : IServerSystem
    {
        private readonly ServerRuntimeData runtimeData;
        private readonly INetworkServer networkServer;

        private ServerSend Send;
        private ServerReceive Receive;

        public AuthenticationSystem(ServerRuntimeData runtimeData, INetworkServer networkServer)
        {
            this.runtimeData = runtimeData;
            this.networkServer = networkServer;
        }

        public void Start()
        {
            // Send = new ServerSend(networkServer);
            // Receive = new ServerReceive();

            // Receive.RegisterMessageHandler<LoginRequest>(
            //     MessageTypeId.LoginRequest,
            //     HandleLoginRequest
            // );
        }

        public void FixedUpdate() { }

        public void Stop()
        {
            // Send = null;
            // Receive = null;
        }

        private void HandleLoginRequest(NetworkId networkId, LoginRequest message)
        {
            var loginResult = ValidateLogin(message.secret);

            if (loginResult == ValidateLoginResult.AccessGranted)
            {
                runtimeData.NetworkServerData.UnauthorizedConnections.Remove(networkId);
                runtimeData.NetworkServerData.AuthorizedConnections.Add(networkId);

                Log.Info("Accepting LoginRequest from client {ClientId}", networkId.ToString());
            }

            Send.Reliable(networkId, new LoginResult() { result = loginResult });
        }

        private static ValidateLoginResult ValidateLogin(string _)
        {
            // we just accept any connection for now
            // this should become some kind of jwt validation
            return ValidateLoginResult.AccessGranted;
        }
    }
}
