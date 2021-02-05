using System.Linq;
using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class ServerOperations : IServerOperations
	{
		private IServerRuntimeData runtimeData;
		private INetworkServer networkServer;
		private IMessageSerializer messageSerializer;

		public ServerOperations(
			IServerRuntimeData runtimeDataIn,
			INetworkServer networkServerIn,
			IMessageSerializer messageSerializerIn )
		{
			runtimeData = runtimeDataIn;
			networkServer = networkServerIn;
			messageSerializer = messageSerializerIn;
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
