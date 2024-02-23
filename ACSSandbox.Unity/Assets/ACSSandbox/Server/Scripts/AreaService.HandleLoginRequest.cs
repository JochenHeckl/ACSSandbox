using ACSSandbox.AreaServiceProtocol;
using ACSSandbox.Common.Network;
using ACSSandbox.Common.Profiling;
using Unity.Logging;

namespace ACSSandbox.Server
{
    public partial class AreaService
    {
        private void HandleLoginRequest(NetworkId networkId, string secret)
        {
            var loginResult = ValidateLogin(secret);

            if (loginResult == LoginResultType.AccessGranted)
            {
                unauthorizedConnections.Remove(networkId);
                authorizedConnections.Add(networkId);

                Log.Info("Accepting LoginRequest from client {ClientId}", networkId.ToString());
            }

            Send.LoginResult(networkId, loginResult);
        }

        private static LoginResultType ValidateLogin(string _)
        {
            // we just accept any connection for now
            // this should become some kind of jwt validation
            return LoginResultType.AccessGranted;
        }
    }
}
