using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ACSSandbox.AreaServiceProtocol;
using ACSSandbox.AreaServiceProtocol.ServerToClient;
using ACSSandbox.Common.Network;
using Unity.Logging;

namespace ACSSandbox.Client
{
    public class AreaServiceConnection : IAreaServiceConnection, INetworkClientEventProcessor
    {
        public ConnectionStatus ConnectionStatus { get; private set; }
        public ClientSend Send { get; private set; }
        public ClientReceive Receive { get; private set; }

        private readonly INetworkClient networkClient;
        private readonly CancellationTokenSource cancellationTokenSource;
        private Task networkProcessingTask;

        public AreaServiceConnection(
            INetworkClient networkClient
        )
        {
            cancellationTokenSource = new();

            this.networkClient = networkClient;

            Send = new ClientSend(networkClient);
            Receive = new ClientReceive() { HandleLoginResult = HandleLoginResult };
        }

        public void Start(string hostname, int servicePort, string clientId)
        {
            var ipv6Address = Dns.GetHostAddresses(hostname)
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetworkV6);
            var ipAddress = Dns.GetHostAddresses(hostname)
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

            var address = ipv6Address ?? ipAddress;

            if (address == default)
            {
                throw new InvalidOperationException($"hostname {hostname} can not be resolved.");
            }

            if (networkProcessingTask != null)
            {
                throw new InvalidOperationException("Area Service Connection is already started.");
            }

            networkProcessingTask = networkClient.RunClientAsync(
                hostname,
                servicePort,
                this,
                cancellationTokenSource.Token
            );
        }

        public Task Stop()
        {
            if (networkProcessingTask == null)
            {
                throw new InvalidOperationException(
                    $"network processing task is not running. Did you forget to call {nameof(Start)}()?"
                );
            }

            cancellationTokenSource.Cancel();
            return networkProcessingTask;
        }

        public void HandleConnect()
        {
            ConnectionStatus = ConnectionStatus.Connected;
        }

        public void HandleDisconnect()
        {
            ConnectionStatus = ConnectionStatus.Disconnected;
        }

        public void HandleInboundData(ReadOnlySpan<byte> inboundData)
        {
            ConnectionStatus = ConnectionStatus.Connected;

            try
            {
                Receive.HandleInboundData(inboundData);
            }
            catch (Exception exception)
            {
                Log.Error("Failed to handle inbound data. {Exception}", exception);
            }
        }

        private void HandleLoginResult(LoginResult message)
        {
            Log.Info("Login result received: {result}", message.result);

            if (message.result == LoginResultType.AccessGranted)
            {
                ConnectionStatus = ConnectionStatus.Authenticated;
            }
        }
    }
}
