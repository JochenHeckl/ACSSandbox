using System;
using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal partial class ServerOperations : IServerOperations
	{
		private void HandleSpawnRequest( int clientId, SpawnRequest message )
		{
			var client = runtimeData.AuthenticatedClients.First( x => x.ClientConnectionId == clientId );

			if( client == null )
			{
				Send( clientId, new SpawnResponse()
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

			Send( clientId, new SpawnResponse()
			{
				Result = SpawnRequestResult.Success,
				ControlledUnitId = controlledUnit.UnitId
			} );
		}

		private long MakeUnitId()
		{
			return runtimeData.NextUnitId++;
		}
	}
}
