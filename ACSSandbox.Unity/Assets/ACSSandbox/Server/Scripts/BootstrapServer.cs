using System.Linq;
using System.Threading.Tasks;
using ACSSandbox.Common.Network;
using ACSSandbox.Common.Network.Ruffles;
using JH.AppConfig;
using JH.IoCLight;
using Unity.Logging;
using UnityEngine;

namespace ACSSandbox.Server
{
    public class BootstrapServer : BootstrapBase
    {
        private IServerSystem[] serverSystems;
        public AppConfig<ServerConfiguration> ServerAppConfig =>
            new AppConfig<ServerConfiguration>("ACSSandbox", "Configuration.Server.json");

        public override void Compose()
        {
            Container.RegisterInstance(ServerAppConfig);

            var serverRuntimeData = new ServerRuntimeData
            {
                Configuration = ServerAppConfig.Data,
                WorldRootTransform = transform,
            };

            Container.RegisterInstance(serverRuntimeData).SingleInstance();

            Container.Register<NetworkServerRuffles>().As<INetworkServer>().SingleInstance();

            Container.Register<ServerOperations>().SingleInstance();
        }

        public override void OnDestroy()
        {
            Log.Info("Server shutting down.");

            foreach (var serverSystem in serverSystems)
            {
                serverSystem.Stop();
            }

            var serverOperations = Container.Resolve<ServerOperations>();
            serverOperations.ShutdownNetworkServer();

            base.OnDestroy();
        }

        public void OnApplicationQuit()
        {
            Log.Info("Exiting server process.");

            Application.Quit();
        }

        public void Start()
        {
            Log.Info("Server starting.");

            var serverOperations = Container.Resolve<ServerOperations>();
            serverOperations.StartupNetworkServer(destroyCancellationToken);

            serverSystems = Container.ResolveAll<IServerSystem>().ToArray();

            foreach (var serverSystem in serverSystems)
            {
                serverSystem.Start();
            }
        }

        public void FixedUpdate()
        {
            foreach (var serverSystem in serverSystems)
            {
                serverSystem.FixedUpdate();
            }
        }
    }
}
