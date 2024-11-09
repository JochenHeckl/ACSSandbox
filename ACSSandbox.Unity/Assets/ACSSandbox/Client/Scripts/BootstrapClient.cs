using System;
using ACSSandbox.Client.System;
using ACSSandbox.Common.Network;
using ACSSandbox.Common.Network.Ruffles;
using JH.AppConfig;
using JH.IoCLight;
using Unity.Logging;
using UnityEngine;

namespace ACSSandbox.Client
{
    // Last thing we do is to initialize the client
    [DefaultExecutionOrder(10000)]
    public class BootstrapClient : BootstrapBase
    {
        [SerializeField]
        private ClientResources clientResources;

        private IClientSystem[] clientSystems = Array.Empty<IClientSystem>();

        public AppConfig<ClientConfiguration> ClientAppConfig =>
            new AppConfig<ClientConfiguration>("ACSSandbox", "Configuration.Client.json");

        public override void Compose()
        {
            Container.RegisterInstance(ClientAppConfig);

            var clientRuntimeData = new ClientRuntimeData
            {
                Configuration = ClientAppConfig.Data,
                ClientResources = clientResources,
            };

            Container.RegisterInstance(clientRuntimeData).SingleInstance();

            Container.Register<NetworkClientRuffles>().As<INetworkClient>().SingleInstance();

            Container.Register<ClientOperations>();
        }

        public override void OnDestroy()
        {
            ClientAppConfig.Save();

            Log.Info("Client shutting down.");

            foreach (var system in clientSystems)
            {
                system.Stop();
            }

            base.OnDestroy();
        }

        public void Start()
        {
            Log.Info("Client starting.");

            // areaServerConnection = Container.Resolve<IAreaServiceConnection>();
            // areaServerConnection.Start(areaServerHostname, areaServerPort, clientId);

            clientSystems = Container.ResolveAll<IClientSystem>();

            foreach (var system in clientSystems)
            {
                system.Start();
            }

            // // for simplicity reasons we will just generate a pseudo secret
            // // and request a login with it. The server will always accept us.

            // var secret = Guid.NewGuid().ToString("N");
            // areaServerConnection.Send.ReliableInOrder(new LoginRequest() { secret = secret });

#if UNITY_EDITOR
            var runtimeData = Container.Resolve<ClientRuntimeData>();
            var clientOperations = Container.Resolve<ClientOperations>();

            clientOperations.ConnectToHost("localhost", runtimeData.Configuration.areaServerPort);
#endif
        }

        public void FixedUpdate()
        {
            foreach (var system in clientSystems)
            {
                system.Update();
            }
        }
    }
}
