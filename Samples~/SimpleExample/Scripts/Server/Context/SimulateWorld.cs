using System.Linq;
using System.Security.Cryptography.X509Certificates;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal partial class SimulateWorld : IContext
	{
		private readonly ServerConfiguration configuration;
		private readonly IServerOperations operations;
		private readonly IServerResources resources;
		private readonly IServerRuntimeData runtimeData;
		private readonly IMessageDispatcher messageDispatcher;
		private readonly INetworkServer networkServer;

		private float targetIntegrationTimeSec;
		private float nextUnitDataSyncTimeSec;

		public SimulateWorld(
			ServerConfiguration configurationIn,
			IServerOperations operationsIn,
			IServerResources resourcesIn,
			IServerRuntimeData runtimeDataIn,
			INetworkServer networkServerIn,
			IMessageDispatcher messageDispatcherIn )
		{
			configuration = configurationIn;
			operations = operationsIn;
			resources = resourcesIn;
			runtimeData = runtimeDataIn;
			networkServer = networkServerIn;
			messageDispatcher = messageDispatcherIn;
		}

		public void EnterContext( IContextContainer contextContainer )
		{
			messageDispatcher.RegisterHandler<LoginRequest>( HandleLoginRequest );
			messageDispatcher.RegisterHandler<SpawnRequest>( HandleSpawnRequest );
			messageDispatcher.RegisterHandler<NavigateToPositionRequest>( HandleNavigateToPositionRequest );

			targetIntegrationTimeSec = runtimeData.ServerIntegrationTimeSec;
			nextUnitDataSyncTimeSec = runtimeData.ServerIntegrationTimeSec;
			nextUnitDataSyncTimeSec = runtimeData.ServerIntegrationTimeSec;
		}

		public void LeaveContext( IContextContainer contextContainer )
		{
			messageDispatcher.UnregisterHandler<NavigateToPositionRequest>( HandleNavigateToPositionRequest );
			messageDispatcher.UnregisterHandler<SpawnRequest>( HandleSpawnRequest );
			messageDispatcher.UnregisterHandler<LoginRequest>( HandleLoginRequest );
		}

		public void Update( IContextContainer contextContainer, float deltaTimeSec )
		{
			// test for conditions to shut down the server
			targetIntegrationTimeSec += deltaTimeSec;

			while ( runtimeData.ServerIntegrationTimeSec < targetIntegrationTimeSec )
			{
				runtimeData.ServerIntegrationTimeSec += configuration.IntegrationTimeStepSec;

				SimulateUnits( runtimeData.ServerIntegrationTimeSec, configuration.IntegrationTimeStepSec );
			}

			if ( runtimeData.ServerIntegrationTimeSec > nextUnitDataSyncTimeSec )
			{
				nextUnitDataSyncTimeSec += configuration.UnitDataSyncIntervalSec;

				SyncUnits();
			}

		}

		private void SimulateUnits( float serverTimeSec, float deltaTimeSec )
		{
			foreach( var unit in runtimeData.Units.Values )
			{
				AdvanceTowardsDestination( unit.unitData, deltaTimeSec );
			}
		}

		private void AdvanceTowardsDestination( IServerUnitData unitData, float deltaTimeSec )
		{
			var maxDeltaPosition = unitData.MaxSpeedMetersPerSec * deltaTimeSec;

			var towardsDestination = unitData.Destination - unitData.Position;
			var distanceToDestination = Vector3.Magnitude( towardsDestination );

			if( distanceToDestination < maxDeltaPosition )
			{
				unitData.Position = unitData.Destination;
			}
			else
			{
				unitData.Position += towardsDestination * (maxDeltaPosition / distanceToDestination);
			}
		}

		private void SyncUnits()
		{

			var authenticatedClientIds = runtimeData.AuthenticatedClients
				.Select( x => x.ClientConnectionId )
				.ToArray();

			var units = runtimeData.Units.Values
				.Select( x => new UnitSync.UnitData()
				{
					UnitId = x.unitData.UnitId,
					Position = x.unitData.Position,
					Rotation = x.unitData.Rotation,
					UnitTypeId = x.unitData.UnityTypeId,
					ControllingUserId = x.unitData.ControllingUserId
				} )
				.ToArray();

			operations.Send( authenticatedClientIds,
				new UnitSync()
				{
					ServerIntegrationTimeSec = runtimeData.ServerIntegrationTimeSec,
					Units = units
				} );

		}
	}
}