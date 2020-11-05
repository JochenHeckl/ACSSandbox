using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal partial class StartupServer : IContext
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

			var loggedInUser = runtimeData.AuthenticatedClients.FirstOrDefault( x => x.UserId == message.UserId );

			if ( loggedInUser == null )
			{
				runtimeData.AuthenticatedClients.Add( new AuthenticatedClient()
				{
					ClientConnectionId = clientId,
					UserId = message.UserId,
					LoginToken = message.LoginToken
				} );

				Send( clientId, new LoginResponse()
				{
					LoginResult = LoginResult.OK
				} );

				return;
			}

			if ( loggedInUser.ClientConnectionId != clientId )
			{
				// this user is logged in already
				// so we terminate the other client

				Send( loggedInUser.ClientConnectionId, new ForcedLogout()
				{
					Reason = ForcedLogoutReason.ConnectionTakeOver
				} );

				Send( clientId, new LoginResponse()
				{
					LoginResult = LoginResult.OK
				} );

				loggedInUser.ClientConnectionId = clientId;
				loggedInUser.LoginToken = message.LoginToken;
			}
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

		private bool ValidateUser( string userId, string loginToken )
		{
			// for now just accept anyone.
			return true;
		}
	}
}
