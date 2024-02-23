using System.Threading.Tasks;
using ACSSandbox.AreaServiceProtocol;

namespace ACSSandbox.Client
{
    public enum ConnectionStatus
    {
        Disconnected,
        Connected,
        Authenticated
    }

    public interface IAreaServiceConnection
    {
        public ConnectionStatus ConnectionStatus { get; }

        ClientSend Send { get; }
        ClientReceive Receive { get; }

        // for simplicity we just use a clientId for authentication.
        void Start(string hostname, int port, string clientId);
        Task Stop();
    }
}
