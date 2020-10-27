using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class SimulationSystem : IServerSystem
	{
		private readonly ServerConfiguration configuration;
		private readonly IServerRuntimeData runtimeData;

		private float timeLapse;

		public SimulationSystem( ServerConfiguration configurationIn, IServerRuntimeData runtimeDataIn )
		{
			configuration = configurationIn;
			runtimeData = runtimeDataIn;

			timeLapse = configuration.DefaultTimeLapse;
		}

		public void Initialize()
		{
			
		}

		public void Shutdown()
		{
			
		}

		public void Update( float deltaTimeSec )
		{
			var deltaIntegrationTime = deltaTimeSec * timeLapse;
			var targetIntegrationTime = runtimeData.ServerIntegrationTimeSec + deltaIntegrationTime;
			var integrationTimeStepSec = configuration.IntegrationTimeStepSec;

			while ( runtimeData.ServerIntegrationTimeSec < targetIntegrationTime )
			{
				UpdateSimulation( integrationTimeStepSec );

				runtimeData.ServerIntegrationTimeSec += integrationTimeStepSec;
			}
		}

		private void UpdateSimulation( float integrationTimeStepSec )
		{
			
		}
	}
}
