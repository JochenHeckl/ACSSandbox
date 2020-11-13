using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class StartupServer : IContext
	{
		private ServerConfiguration configuration;
		private IServerResources resources;
		private IServerRuntimeData runtimeData;
		private IMessageDispatcher messageDispatcher;
		private INetworkServer networkServer;
		private IMessageSerializer messageSerializer;

		public StartupServer(
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
			// more or less a placeholder for now.
			// we probably have to check for availability of services, register the server etc.

			runtimeData.WorldId = configuration.ServerWorldId;
			runtimeData.World = UnityEngine.Object.Instantiate(
				resources.GetWorld( configuration.ServerWorldId ),
				runtimeData.WorldRoot );

			runtimeData.World.gameObject.layer = runtimeData.WorldRoot.gameObject.layer;

			messageDispatcher.RegisterHandler<PingRequest>( HandlePingRequest );
			messageDispatcher.RegisterHandler<ServerDataRequest>( HandleGlobalServerDataRequest );
		}

		public void LeaveContext( IContextContainer contextContainer )
		{
			messageDispatcher.UnregisterHandler<PingRequest>( HandlePingRequest );
			messageDispatcher.UnregisterHandler<ServerDataRequest>( HandleGlobalServerDataRequest );
		}

		public void ActivateContext( IContextContainer contextContainer ) { }

		public void DeactivateContext( IContextContainer contextContainer ) { }

		public void Update( IContextContainer contextContainer, float deltaTimeSec )
		{
			contextContainer.PushContext( contextContainer.Resolve<SimulateWorld>() );
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
