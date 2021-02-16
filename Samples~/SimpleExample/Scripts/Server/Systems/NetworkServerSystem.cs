using System;
using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	internal class NetworkServerSystem : IServerSystem
    {
		private IServerConfiguration configuration;
		private IServerRuntimeData runtimeData;
		private INetworkServer networkServer;

		public NetworkServerSystem(
			IServerConfiguration configurationIn,
			IServerRuntimeData runtimeDataIn,
			INetworkServer networkServerIn )
		{
			configuration = configurationIn;
			runtimeData = runtimeDataIn;

			networkServer = networkServerIn;
		}

		public void Initialize()
		{
			networkServer.StartServer( configuration.ServerPort, configuration.MaxMessageSizeByte );
		}

		public void Shutdown()
		{
			networkServer.StopServer();
		}

		public void Update( float deltaTime )
		{
			networkServer.ProcessNetworkEvents();
		}
	}
}
