using System.Linq;
using System.Security.Cryptography.X509Certificates;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal partial class SimulateWorld : IContext
	{
		private readonly ServerConfiguration configuration;
		private readonly IServerResources resources;
		private readonly IServerRuntimeData runtimeData;
		private readonly IMessageDispatcher messageDispatcher;
		private readonly INetworkServer networkServer;
		private readonly IMessageSerializer messageSerializer;

		private float targetIntegrationTimeSec;
		private float nextUnitDataSyncTimeSec;

		public SimulateWorld(
			ServerConfiguration configurationIn,
			IServerResources resourcesIn,
			IServerRuntimeData runtimeDataIn,
			INetworkServer networkServerIn,
			IMessageSerializer messageSerializerIn,
			IMessageDispatcher messageDispatcherIn )
		{
			configuration = configurationIn;
			resources = resourcesIn;
			runtimeData = runtimeDataIn;
			networkServer = networkServerIn;
			messageSerializer = messageSerializerIn;
			messageDispatcher = messageDispatcherIn;
		}

		public void EnterContext( IContextContainer contextContainer )
		{
			messageDispatcher.RegisterHandler<LoginRequest>( HandleLoginRequest );
			messageDispatcher.RegisterHandler<SpawnRequest>( HandleSpawnRequest );

			targetIntegrationTimeSec = runtimeData.ServerIntegrationTimeSec;
			nextUnitDataSyncTimeSec = runtimeData.ServerIntegrationTimeSec;
			nextUnitDataSyncTimeSec = runtimeData.ServerIntegrationTimeSec;
		}

		public void LeaveContext( IContextContainer contextContainer )
		{
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

				SimulateUnits();
			}

			if ( runtimeData.ServerIntegrationTimeSec > nextUnitDataSyncTimeSec )
			{
				nextUnitDataSyncTimeSec += configuration.UnitDataSyncIntervalSec;

				SyncUnits();
			}

		}

		private void SimulateUnits()
		{

		}

		private void SyncUnits()
		{

			var authenticatedClientIds = runtimeData.AuthenticatedClients
				.Select( x => x.ClientConnectionId )
				.ToArray();

			var units = runtimeData.Units
				.Select( x => new UnitSync.UnitData()
				{
					UnitId = x.UnitId,
					Position = x.Position,
					Rotation = x.Rotation,
					UnitTypeId = x.UnityTypeId,
					ControllingUserId = x.ControllingUserId
				} )
				.ToArray();

			Send( authenticatedClientIds,
				new UnitSync()
				{
					ServerIntegrationTimeSec = runtimeData.ServerIntegrationTimeSec,
					Units = units
				} );

		}

		private void Send( int clientId, object message )
		{
			networkServer.Send( clientId, messageSerializer.Serialize( message ) );
		}
		private void Send( int[] clientIds, object message )
		{
			networkServer.Send( clientIds, messageSerializer.Serialize( message ) );
		}
	}
}