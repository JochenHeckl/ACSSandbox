using System;
using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal partial class SimulateWorld : IContext, IEquatable<SimulateWorld>
	{
		private int nextUnitId;

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

		private void HandleSpawnRequest( int clientId, SpawnRequest message )
		{
			var client = runtimeData.AuthenticatedClients.First( x => x.ClientConnectionId == clientId );

			var controlledUnit = runtimeData.Units.FirstOrDefault( x => x.ControllingUserId == client.UserId );

			var serverGameObject = UnityEngine.Object.Instantiate<ServerUnitView>(
				resources.GetUnitPrefab( UnitTypeId.SampleUnitType ),
				message.SpawnLocation,
				Quaternion.identity,
				runtimeData.World.unitRoot );

			if ( controlledUnit == null )
			{
				runtimeData.Units.Add( new ServerUnitData()
				{
					UnitId = MakeUnitId(),
					UnityTypeId = UnitTypeId.SampleUnitType,
					ControllingUserId = client.UserId,
					Position = serverGameObject.transform.position,
					Rotation = serverGameObject.transform.rotation,
				} );
			}
		}

		private long MakeUnitId()
		{
			return nextUnitId++;
		}

		private bool ValidateUser( string userId, string loginToken )
		{
			// for now just accept anyone.
			return true;
		}

		private void ProcessClientLogin( AuthenticatedClient authenticatedClient )
		{
		}

		public bool Equals( SimulateWorld other )
		{
			throw new NotImplementedException();
		}

		public void ActivateContext( IContextContainer contextContainer )
		{
			
		}

		public void DeactivateContext( IContextContainer contextContainer )
		{
			
		}
	}
}
