namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal interface IServerOperations
	{
		public AuthenticatedClient GetAuthenticatedClient( int clientConnectionId );
		public IServerUnitData GetControlledUnit( string userId );

		void Send( int clientId, object message );
		void Send( int[] clientIds, object message );
	}
}
