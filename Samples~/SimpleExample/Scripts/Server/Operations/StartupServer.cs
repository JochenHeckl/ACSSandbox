using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class StartupServer : IState
	{
		private ServerConfiguration configuration;
		private IServerResources resources;
		private IServerRuntimeData runtimeData;
		private IAddressableMessageDispatcher<int> messageDispatcher;
		private INetworkServer networkServer;
		private IMessageSerializer messageSerializer;

		public StartupServer(
			ServerConfiguration configurationIn,
			IServerResources resourcesIn,
			IServerRuntimeData runtimeDataIn,
			INetworkServer networkServerIn,
			IMessageSerializer messageSerializerIn,
			IAddressableMessageDispatcher<int> messageDispatcherIn )
		{
			configuration = configurationIn;
			resources = resourcesIn;
			runtimeData = runtimeDataIn;
			networkServer = networkServerIn;
			messageSerializer = messageSerializerIn;
			messageDispatcher = messageDispatcherIn;
		}

		public void EnterState( IStateMachine contextContainer )
		{
			// more or less a placeholder for now.
			// we probably have to check for availability of services, register the server etc.

			runtimeData.WorldId = configuration.ServerWorldId;
			runtimeData.World = Object.Instantiate( resources.GetWorld( configuration.ServerWorldId ), runtimeData.WorldRoot );
			runtimeData.World.gameObject.RecursiveMoveToLayer( runtimeData.WorldRoot.gameObject.layer );

			runtimeData.World.gameObject.layer = runtimeData.WorldRoot.gameObject.layer;

			messageDispatcher.RegisterHandler<PingRequest>( HandlePingRequest );
			messageDispatcher.RegisterHandler<ServerDataRequest>( HandleGlobalServerDataRequest );
		}

		public void LeaveState( IStateMachine contextContainer )
		{
			messageDispatcher.DeregisterHandler<PingRequest>( HandlePingRequest );
			messageDispatcher.DeregisterHandler<ServerDataRequest>( HandleGlobalServerDataRequest );
		}

		public void ActivateState( IStateMachine contextContainer ) { }

		public void DeactivateState( IStateMachine contextContainer ) { }

		public void UpdateState( IStateMachine contextContainer, float deltaTimeSec )
		{
			contextContainer.PushState( contextContainer.StateResolver.Resolve<SimulateWorld>() );
		}

		private void HandlePingRequest( int clientId, PingRequest message )
		{
			Send( clientId, new PingResponse()
			{
				PingRequestTimeSec = message.PingRequestTimeSec
			} );
		}
		private void HandleGlobalServerDataRequest( int clientId, ServerDataRequest message )
		{
			Send( clientId, new ServerDataResponse()
			{
				UptimeSec = Time.realtimeSinceStartup,
				LoggedInUserCount = runtimeData.AuthenticatedClients.Count,
				WorldId = runtimeData.WorldId
			} );
		}

		private void Send( int clientId, object message )
		{
			networkServer.Send( clientId, messageSerializer.Serialize( message ) );
		}
	}
}
