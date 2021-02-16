using System;
using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal partial class SimulateWorld : IState, IEquatable<SimulateWorld>
	{
		private int nextUnitId;

		private void HandleLoginRequest( int clientId, LoginRequest message )
		{
			if ( !ValidateUser( message.UserId, message.LoginToken ) )
			{
				operations.Send( clientId, new LoginResponse()
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
					operations.Send( authenticatedClient.ClientConnectionId, new ForcedLogout()
					{
						Reason = ForcedLogoutReason.ConnectionTakeOver
					} );
				}

				authenticatedClient.ClientConnectionId = clientId;
				authenticatedClient.LoginToken = message.LoginToken;
			}

			operations.Send( clientId, new LoginResponse()
			{
				LoginResult = LoginResult.OK
			} );

			ProcessClientLogin( authenticatedClient );
		}

		private void HandleSpawnRequest( int clientId, SpawnRequest message )
		{
			var client = runtimeData.AuthenticatedClients.First( x => x.ClientConnectionId == clientId );

			if( client == null )
			{
				operations.Send( clientId, new SpawnResponse()
				{
					Result = SpawnRequestResult.AuthenticationFailure,
					ControlledUnitId = -1
				} );
			}

			var controlledUnit = runtimeData.Units.Values
				.Select( x => x.unitData )
				.FirstOrDefault( x => x.ControllingUserId == client.UserId );

			if ( controlledUnit == null )
			{
				controlledUnit = new ServerUnitData()
				{
					UnitId = MakeUnitId(),
					UnityTypeId = UnitTypeId.SampleUnitType,
					ControllingUserId = client.UserId,
					Position = message.SpawnLocation,
					Rotation = Quaternion.identity,
					MaxSpeedMetersPerSec = configuration.MaxUnitSpeed
				};

				var serverUnitView = UnityEngine.Object.Instantiate(
				resources.GetUnitPrefab( UnitTypeId.SampleUnitType ),
				message.SpawnLocation,
				Quaternion.identity,
				runtimeData.World.unitRoot );

				serverUnitView.DataSource = new ServerUnitViewModel()
				{
					UnitData = controlledUnit
				};

				controlledUnit.Destination = controlledUnit.Position;
				runtimeData.Units[ controlledUnit.UnitId ] = ( controlledUnit, serverUnitView );
			}

			operations.Send( clientId, new SpawnResponse()
			{
				Result = SpawnRequestResult.Success,
				ControlledUnitId = controlledUnit.UnitId
			} );
		}

		private void HandleNavigateToPositionRequest( int clientConnectionId, NavigateToPositionRequest message )
		{
			var user = operations.GetAuthenticatedClient( clientConnectionId );

			var controlledUnit = operations.GetControlledUnit( user.UserId );

			if ( controlledUnit != null )
			{
				controlledUnit.Destination = message.Destination;

				operations.Send( clientConnectionId, new NavigateToPositionResponse()
				{
					Result = NavigateToPositionResult.Success
				} ) ;

				return;
			}

			operations.Send( clientConnectionId, new NavigateToPositionResponse()
			{
				Result = NavigateToPositionResult.InvalidOperation
			} );
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

		public void ActivateState( IStateMachine contextContainer )
		{
			
		}

		public void DeactivateState( IStateMachine contextContainer )
		{
			
		}
	}
}
