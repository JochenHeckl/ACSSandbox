using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	internal class SimulateWorld : IState
	{
		private readonly ServerConfiguration configuration;
		private readonly IServerOperations operations;
		private readonly IServerResources resources;
		private readonly IServerRuntimeData runtimeData;
		private readonly INetworkServer networkServer;

		private float nextUnitDataSyncTimeSec;

		public SimulateWorld(
			ServerConfiguration configurationIn,
			IServerOperations operationsIn,
			IServerResources resourcesIn,
			IServerRuntimeData runtimeDataIn,
			INetworkServer networkServerIn )
		{
			configuration = configurationIn;
			operations = operationsIn;
			resources = resourcesIn;
			runtimeData = runtimeDataIn;
			networkServer = networkServerIn;
		}

		public void ActivateState( IStateMachine stateMachine )
		{
		}

		public void DeactivateState( IStateMachine stateMachine )
		{
		}

		public void EnterState( IStateMachine contextContainer )
		{
			nextUnitDataSyncTimeSec = Time.realtimeSinceStartup;
		}

		public void LeaveState( IStateMachine contextContainer )
		{
		}

		public void UpdateState( IStateMachine contextContainer, float deltaTimeSec )
		{
			if ( nextUnitDataSyncTimeSec < Time.realtimeSinceStartup )
			{
				nextUnitDataSyncTimeSec += configuration.UnitDataSyncIntervalSec;

				SyncUnits();
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