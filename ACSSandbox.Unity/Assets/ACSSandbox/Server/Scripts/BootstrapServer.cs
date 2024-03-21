using System.IO;
using ACSSandbox.AreaServiceProtocol.ServerToClient;
using ACSSandbox.Common.Network;
using ACSSandbox.Common.Network.Ruffles;
using ACSSandbox.Common.Networking.NetworkSimulator;
using IoCLight;
using Unity.Logging;
using UnityEngine;

namespace ACSSandbox.Server
{
    public class BootstrapServer : BootstrapBase
    {
        public int targetFrameRate = 1;
        public int areaServicePort = 1337;
        private static string ServerConfigFilePath => "ACSSandbox.Server.json";

        public bool useNetworkSimulator;
        public NetworkSimulator networkSimulator;

        private IAreaService areaService;
        private ServerConfiguration configuration;
        private float nextHearBeatSec;

        public override void Compose()
        {
            configuration = LoadServerConfiguration();
            Container.RegisterInstance(configuration.networkServerConfiguration);

            if (useNetworkSimulator)
            {
                Container.RegisterInstance(networkSimulator).As<INetworkServer>();
            }
            else
            {
                Container.Register<NetworkServerRuffles>().As<INetworkServer>().SingleInstance();
            }

            Container.Register<AreaService>().As<IAreaService>().SingleInstance();
        }

        private void Start()
        {
            Log.Info("Area Server starting.");

            Application.targetFrameRate = targetFrameRate;

            areaService = Container.Resolve<IAreaService>();
            areaService.StartService(areaServicePort);

            nextHearBeatSec = Time.time;
        }

        public void FixedUpdate()
        {
            if (Time.time < nextHearBeatSec)
            {
                return;
            }

            nextHearBeatSec += configuration.HeartBeatIntervalSec;
            areaService.Send.Broadcast(
                new ServerHeartBeat() { serverTimeSec = Time.time },
                TransportChannel.Unreliable
            );
        }

        public async void OnApplicationQuit()
        {
            Log.Info("Area Server shutting down.");

            await areaService.StopService();
            areaService = null;
        }

        private static ServerConfiguration LoadServerConfiguration()
        {
            if (!File.Exists(ServerConfigFilePath))
            {
                ServerConfiguration defaultConfig = new();
                var jsonData = JsonUtility.ToJson(defaultConfig, true);

                File.WriteAllText(ServerConfigFilePath, jsonData);
            }

            var configurationJson = File.ReadAllText(ServerConfigFilePath);
            var configuration = JsonUtility.FromJson<ServerConfiguration>(configurationJson);

            return configuration;
        }
    }
}
