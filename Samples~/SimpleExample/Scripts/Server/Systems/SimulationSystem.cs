using System;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class SimulationSystem : IServerSystem
	{
		private readonly IServerConfiguration configuration;
		private readonly IServerRuntimeData runtimeData;

		private float targetIntegrationTimeSec;
		
		public SimulationSystem( IServerConfiguration configurationIn, IServerRuntimeData runtimeDataIn )
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

		public void Update( float deltaTimeSec )
		{
			var deltaIntegrationTimeSec = deltaTimeSec * configuration.IntegrationTimeLapse; 
			targetIntegrationTimeSec += deltaIntegrationTimeSec;
			
			while ( runtimeData.ServerIntegrationTimeSec < targetIntegrationTimeSec )
			{
				runtimeData.ServerIntegrationTimeSec += configuration.IntegrationTimeStepSec;

				SimulateUnits();
			}
		}

		private void SimulateUnits()
		{
			foreach ( var unit in runtimeData.Units.Values )
			{
				AdvanceUnitTowardsDestination( unit.unitData );
			}
		}

		private void AdvanceUnitTowardsDestination( IServerUnitData unitData )
		{
			var maxDeltaPosition = unitData.MaxSpeedMetersPerSec * configuration.IntegrationTimeStepSec;

			var towardsDestination = unitData.Destination - unitData.Position;
			var distanceToDestination = Vector3.Magnitude( towardsDestination );

			if ( distanceToDestination < maxDeltaPosition )
			{
				unitData.Position = unitData.Destination;
			}
			else
			{
				unitData.Position += towardsDestination * (maxDeltaPosition / distanceToDestination);
			}
		}
	}
}
