using System;
using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal partial class ConnectToServer : IContext
	{
		public void HandleUnitSync( UnitSync message )
		{
			// TODO: optimize for efficiency
			// reuse Units that are removed for new ones
			
			var localUnitIds = runtimeData.Units.Keys;
			var remoteUnitIds = message.Units.Select( x => x.UnitId ).ToArray();

			var unitsToUpdateIds = localUnitIds.Intersect( remoteUnitIds );
			var unitsToDelete = localUnitIds.Except( unitsToUpdateIds );
			var unitsToCreate = remoteUnitIds.Except( unitsToUpdateIds );

			foreach( var unitId in unitsToDelete )
			{
				runtimeData.Units[unitId].unitView.Remove();
				runtimeData.Units.Remove( unitId );
			}

			foreach( var remoteUnit in message.Units )
			{
				if( unitsToCreate.Contains( remoteUnit.UnitId ) )
				{
					var clientUnitData = new ClientUnitData()
					{
						UnitId = remoteUnit.UnitId,
						Position = remoteUnit.Position,
						Rotation = remoteUnit.Rotation,
						ControllingUserId = remoteUnit.ControllingUserId,
						UnityTypeId = remoteUnit.UnitTypeId
					};

					var unitView = resources.GetUnitPrefab( remoteUnit.UnitTypeId );

					UnityEngine.Object.Instantiate<ClientUnitView>(unitView, runtimeData.World.UnitRoot);

					unitView.DataSource = new ClientUnitViewModel()
					{
						ClientUnitData = clientUnitData
					};

					runtimeData.Units[remoteUnit.UnitId] = (clientUnitData, unitView);
				}
				else
				{
					var existingUnitData = runtimeData.Units[remoteUnit.UnitId].unitData;

					existingUnitData.Position = remoteUnit.Position;
					existingUnitData.Rotation = remoteUnit.Rotation;
				}
			}
		}
	}
}
