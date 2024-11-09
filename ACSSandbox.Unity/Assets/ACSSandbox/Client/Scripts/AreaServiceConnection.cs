// using System;
// using System.Linq;
// using System.Net;
// using System.Net.Sockets;
// using System.Threading;
// using System.Threading.Tasks;
// using ACSSandbox.Common.Network;
// using Unity.Logging;

// namespace ACSSandbox.Client
// {
//     public class AreaServiceConnection : IAreaServiceConnection, INetworkClientEventProcessor
//     {
//         public delegate void ConnectionStatusChangedDelegate(
//             AreaServiceConnectionStatus areaServiceConnectionStatus
//         );
//         public event ConnectionStatusChangedDelegate ConnectionStatusChanged;

//         public AreaServiceConnectionStatus AreaServiceConnectionStatus { get; private set; }
//         public ClientSend Send { get; private set; }
//         public ClientReceive Receive { get; private set; }

//         private readonly INetworkClient networkClient;
//         private readonly CancellationTokenSource cancellationTokenSource;
//         private Task networkProcessingTask;

//         private ClientRuntimeData clientRuntimeData;

//         public AreaServiceConnection(
//             INetworkClient networkClient,
//             IClientMessageSerializer<NetworkId> serializer,
//             ClientRuntimeData clientRuntimeData
//         )
//         {
//             cancellationTokenSource = new();

//             this.networkClient = networkClient;

//             Send = new ClientSend(networkClient, serializer);
//             Receive = new ClientReceive();

//             this.clientRuntimeData = clientRuntimeData;
//         }

//         public void Start(string hostname, int servicePort, string clientId)
//         {
//             var ipv6Address = Dns.GetHostAddresses(hostname)
//                 .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetworkV6);
//             var ipAddress = Dns.GetHostAddresses(hostname)
//                 .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

//             var address = ipv6Address ?? ipAddress;

//             if (address == default)
//             {
//                 throw new InvalidOperationException($"hostname {hostname} can not be resolved.");
//             }

//             if (networkProcessingTask != null)
//             {
//                 throw new InvalidOperationException("Area Service Connection is already started.");
//             }

//             networkProcessingTask = networkClient.RunClientAsync(
//                 hostname,
//                 servicePort,
//                 this,
//                 cancellationTokenSource.Token
//             );
//         }

//         public Task Stop()
//         {
//             if (networkProcessingTask == null)
//             {
//                 throw new InvalidOperationException(
//                     $"network processing task is not running. Did you forget to call {nameof(Start)}()?"
//                 );
//             }

//             cancellationTokenSource.Cancel();
//             return networkProcessingTask;
//         }

//         public void HandleConnect()
//         {
//             AreaServiceConnectionStatus = AreaServiceConnectionStatus.Connected;
//             ConnectionStatusChanged?.Invoke(AreaServiceConnectionStatus);
//         }

//         public void HandleDisconnect()
//         {
//             AreaServiceConnectionStatus = AreaServiceConnectionStatus.Disconnected;
//             ConnectionStatusChanged?.Invoke(AreaServiceConnectionStatus);
//         }

//         public void HandleInboundData(ReadOnlySpan<byte> inboundData)
//         {
//             if (AreaServiceConnectionStatus == AreaServiceConnectionStatus.Disconnected)
//             {
//                 AreaServiceConnectionStatus = AreaServiceConnectionStatus.Connected;
//             }

//             try
//             {
//                 Receive.HandleInboundData(inboundData);
//             }
//             catch (Exception exception)
//             {
//                 Log.Error("Failed to handle inbound data. {Exception}", exception);
//             }
//         }

//         private void HandleLoginResult(LoginResult message)
//         {
//             Log.Info("Login result received: {result}", message.result);

//             if (message.result == AreaServiceProtocol.ValidateLoginResult.AccessGranted)
//             {
//                 AreaServiceConnectionStatus = AreaServiceConnectionStatus.Authenticated;
//                 ConnectionStatusChanged?.Invoke(AreaServiceConnectionStatus);
//             }
//         }
//     }
// }
