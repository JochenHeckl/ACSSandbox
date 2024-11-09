using System;
using ACSSandbox.Client.System;
using ACSSandbox.Common;
using ACSSandbox.Common.Network;
using JH.DataBinding;
using UnityEngine.AddressableAssets;

namespace ACSSandbox.Client
{
    public partial class ClientOperations : IClientSystem, INetworkClientEventProcessor
    {
        private readonly ClientRuntimeData runtimeData;
        private readonly INetworkClient networkClient;

        public ClientOperations(ClientRuntimeData runtimeData, INetworkClient networkClient)
        {
            this.runtimeData = runtimeData;
            this.networkClient = networkClient;
        }

        public void ConnectToHost(string hostname, int servicePort)
        {
            networkClient.StartClient(hostname, servicePort, this);
        }

        public void Start()
        {
            // networkClient will be started after host was chosen
        }

        public void Stop()
        {
            networkClient.StopClient();
        }

        public void Update()
        {
            networkClient.ProcessEvents();
        }

        public void FixedUpdate() { }

        public void HandleConnect()
        {
            runtimeData.ConnectedToAreaServer = true;
        }

        public void HandleDisconnect()
        {
            runtimeData.ConnectedToAreaServer = false;
        }

        public void HandleInboundData(ReadOnlySpan<byte> inboundData)
        {
            throw new NotImplementedException();
        }
    }
}
