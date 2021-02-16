using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ClientToServer;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	internal partial class ServerOperations : IServerOperations
	{
		private readonly IServerConfiguration configuration;
		private readonly IServerRuntimeData runtimeData;
		private readonly IServerResources resources;
		private readonly INetworkServer networkServer;
		private readonly IMessageSerializer messageSerializer;
		private readonly IAddressableMessageDispatcher<int> messageDispatcher;

		public ServerOperations(
			IServerConfiguration configurationIn,
			IServerRuntimeData runtimeDataIn,
			IServerResources resourcesIn,
			INetworkServer networkServerIn,
			IMessageSerializer messageSerializerIn
			,
			IAddressableMessageDispatcher<int> messageDispatcherIn )
		{
			configuration = configurationIn;
			runtimeData = runtimeDataIn;
			resources = resourcesIn;
			networkServer = networkServerIn;
			messageSerializer = messageSerializerIn;
			messageDispatcher = messageDispatcherIn;

			messageDispatcher.RegisterHandler<LoginRequest>( HandleLoginRequest );
			messageDispatcher.RegisterHandler<SpawnRequest>( HandleSpawnRequest );
			messageDispatcher.RegisterHandler<NavigateToPositionRequest>( HandleNavigateToPositionRequest );
		}

		public AuthenticatedClient GetAuthenticatedClient( int clientConnectionId )
		{
			return runtimeData.AuthenticatedClients.FirstOrDefault( x => x.ClientConnectionId == clientConnectionId );
		}

		public IServerUnitData GetControlledUnit( string userId )
		{
			return runtimeData
				.Units
				.Select( x=> x.Value.unitData )
				.FirstOrDefault( x => x.ControllingUserId == userId );
		}

		public void Send( int clientConnectionId, object message )
		{
			var messageRaw = messageSerializer.Serialize( message );

			networkServer.Send( clientConnectionId, messageRaw );
		}

		public void Send( int[] clientConnectionIds, object message )
		{
			var messageRaw = messageSerializer.Serialize( message );

			networkServer.Send( clientConnectionIds, messageRaw );
		}
	}
}
