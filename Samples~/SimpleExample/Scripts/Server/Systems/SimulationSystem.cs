using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class SimulationSystem : IServerSystem
	{
		private readonly ServerConfiguration configuration;
		private readonly IServerRuntimeData runtimeData;

		public SimulationSystem( ServerConfiguration configurationIn, IServerRuntimeData runtimeDataIn )
		{
			configuration = configurationIn;
			runtimeData = runtimeDataIn;
		}

		public void Initialize()
		{
			
		}

		public void Shutdown()
		{
			
		}

		public void Update( float deltaTime )
		{
			
		}
	}
}
