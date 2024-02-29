using System;
using System.IO;
using ACSSandbox.AreaServiceProtocol;
using ACSSandbox.Client.System;
using ACSSandbox.Common.Network;
using ACSSandbox.Common.Network.Ruffles;
using ACSSandbox.Common.Networking.NetworkSimulator;
using IoCLight;
using Unity.Logging;
using UnityEngine;

namespace ACSSandbox.Client
{
    public class BootstrapClient : BootstrapBase
    {
        public string clientId = Guid.NewGuid().ToString("N");
        public string areaServerHostname = "::";
        public int areaServerPort = 1337;

        public bool useNetworkSimulator;
        public NetworkSimulator networkSimulator;

        private IAreaServiceConnection areaServerConnection;
        private IClientSystem[] clientSystems;

        private static string ClientConfigFilePath => "ACSSandbox.Client.json";

        public override void Compose()
        {
            var configuration = LoadClientConfiguration();
            Container.RegisterInstance(configuration);

            Container.Register<ClientRuntimeData>().SingleInstance();

            if (useNetworkSimulator)
            {
                Container.RegisterInstance(networkSimulator).As<INetworkClient>();
            }
            else
            {
                Container.Register<NetworkClientRuffles>().As<INetworkClient>();
            }

            Container.Register<ProtocolSerializerMemoryPack>().As<IAreaServiceProtocolSerializer>();

            Container
                .Register<AreaServiceConnection>()
                .As<IAreaServiceConnection>()
                .SingleInstance();

            Container.Register<HeartBeatSystem>().As<IClientSystem>().SingleInstance();
        }

        public void Start()
        {
            Log.Info("Client starting.");

            areaServerConnection = Container.Resolve<IAreaServiceConnection>();
            areaServerConnection.Start(areaServerHostname, areaServerPort, clientId);

            clientSystems = Container.ResolveAll<IClientSystem>();

            foreach (var system in clientSystems)
            {
                system.Start();
            }

            // for simplicity reasons we will just generate a pseudo secret
            // and request a login with it. The server will always accept us.
            
            var secret = Guid.NewGuid().ToString("N");
            areaServerConnection.Send.LoginRequest(secret);
        }

        public void FixedUpdate()
        {
            foreach (var system in clientSystems)
            {
                system.Update();
            }
        }

        public async void OnApplicationQuit()
        {
            Log.Info("Client shutting down.");

            foreach (var system in clientSystems)
            {
                system.Stop();
            }

            await areaServerConnection.Stop();
            areaServerConnection = null;
        }

        private static ClientConfiguration LoadClientConfiguration()
        {
            if (!File.Exists(ClientConfigFilePath))
            {
                ClientConfiguration defaultConfig = new();
                var jsonData = JsonUtility.ToJson(defaultConfig, true);

                File.WriteAllText(ClientConfigFilePath, jsonData);
            }

            var configurationJson = File.ReadAllText(ClientConfigFilePath);
            var configuration = JsonUtility.FromJson<ClientConfiguration>(configurationJson);

            return configuration;
        }
    }
}
