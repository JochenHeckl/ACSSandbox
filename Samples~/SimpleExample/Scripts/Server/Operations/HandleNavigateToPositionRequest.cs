
using de.JochenHeckl.Unity.ACSSandbox.Example.Common;
using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ServerToClient;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	internal partial class ServerOperations : IServerOperations
	{
		private void HandleNavigateToPositionRequest( int clientConnectionId, NavigateToPositionRequest message )
		{
			var user = GetAuthenticatedClient( clientConnectionId );

			var controlledUnit = GetControlledUnit( user.UserId );

			if ( controlledUnit != null )
			{
				controlledUnit.Destination = message.Destination;

				Send( clientConnectionId, new NavigateToPositionResponse()
				{
					Result = NavigateToPositionResult.Success
				} ) ;

				return;
			}

			Send( clientConnectionId, new NavigateToPositionResponse()
			{
				Result = NavigateToPositionResult.InvalidOperation
			} );
		}
	}
}
