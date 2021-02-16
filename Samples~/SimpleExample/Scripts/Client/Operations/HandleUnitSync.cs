using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal partial class ClientOperations : IClientOperations
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
				(ClientUnitData unitData, ClientUnitView unitView) localUnit;

				if( unitsToCreate.Contains( remoteUnit.UnitId ) )
				{
					var localUnitData = new ClientUnitData()
					{
						UnitId = remoteUnit.UnitId,
						Position = remoteUnit.Position,
						Rotation = remoteUnit.Rotation,
						ControllingUserId = remoteUnit.ControllingUserId,
						UnityTypeId = remoteUnit.UnitTypeId
					};

					var unitPrefab = resources.GetUnitPrefab( remoteUnit.UnitTypeId );
					var localUnitView = UnityEngine.Object.Instantiate<ClientUnitView>( unitPrefab, runtimeData.World.UnitRoot);

					localUnitView.DataSource = new ClientUnitViewModel()
					{
						UnitData = localUnitData
					};

					localUnit = (localUnitData, localUnitView);
					runtimeData.Units[remoteUnit.UnitId] = localUnit;
				}
				else
				{
					localUnit = runtimeData.Units[remoteUnit.UnitId];

					localUnit.unitData.Position = remoteUnit.Position;
					localUnit.unitData.Rotation = remoteUnit.Rotation;
					localUnit.unitData.ControllingUserId = remoteUnit.ControllingUserId;

				}

				if( localUnit.unitData.UnitId == runtimeData.ControlledUnitId )
				{
					runtimeData.WorldCamera.TrackGameObjects( new Transform[] { localUnit.unitView.transform } );
				}
			}
		}
	}
}
