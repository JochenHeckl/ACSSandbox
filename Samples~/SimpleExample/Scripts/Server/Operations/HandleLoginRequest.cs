using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Example.Common;
using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ServerToClient;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	internal partial class ServerOperations : IServerOperations
	{
		private void HandleLoginRequest( int clientId, LoginRequest message )
		{
			if ( !ValidateUser( message.UserId, message.LoginToken ) )
			{
				Send( clientId, new LoginResponse()
				{
					LoginResult = LoginResult.AuthenticationError
				} );

				return;
			}

			var authenticatedClient = runtimeData.AuthenticatedClients.FirstOrDefault( x => x.UserId == message.UserId );

			if ( authenticatedClient == null )
			{
				// this is a regular login

				authenticatedClient = new AuthenticatedClient()
				{
					ClientConnectionId = clientId,
					UserId = message.UserId,
					LoginToken = message.LoginToken
				};

				runtimeData.AuthenticatedClients.Add( authenticatedClient );
			}
			else
			{
				// this user is logged in already
				// so we terminate the old client if the old client should be different

				if ( authenticatedClient.ClientConnectionId != clientId )
				{
					Send( authenticatedClient.ClientConnectionId, new ForcedLogout()
					{
						Reason = ForcedLogoutReason.ConnectionTakeOver
					} );
				}

				authenticatedClient.ClientConnectionId = clientId;
				authenticatedClient.LoginToken = message.LoginToken;
			}

			Send( clientId, new LoginResponse()
			{
				LoginResult = LoginResult.OK
			} );

			ProcessClientLogin( authenticatedClient );
		}

		private bool ValidateUser( string userId, string loginToken )
		{
			// for now just accept anyone.
			return true;
		}

		private void ProcessClientLogin( AuthenticatedClient authenticatedClient )
		{
		}
	}
}
