// using System.Threading.Tasks;
// using ACSSandbox.AreaServiceProtocol;

// namespace ACSSandbox.Client
// {
//     public enum AreaServiceConnectionStatus
//     {
//         Disconnected,
//         Connected,
//         Authenticated
//     }

//     public interface IAreaServiceConnection
//     {
//         public AreaServiceConnectionStatus AreaServiceConnectionStatus { get; }

//         ClientSend Send { get; }
//         ClientReceive Receive { get; }

//         // for simplicity we just use a clientId for authentication.
//         void Start(string hostname, int port, string clientId);
//         Task Stop();
//     }
// }
