using System;
using System.Linq;


using UnityEngine;
using UnityEngine.Serialization;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	[CreateAssetMenu( fileName = "ServerResources", menuName = "ACS Sandbox/ServerResources", order = 1 )]
	public class ServerResources : ScriptableObject, IServerResources
	{
		public World[] worlds;

		public ServerUnitView[] unitPrefabs;

		public World GetWorld( Guid worldId )
		{
			return worlds.FirstOrDefault( x => x.WorldId == worldId );
		}
		public ServerUnitView GetUnitPrefab( UnitTypeId unitTypeId )
		{
			var unitPrefab = unitPrefabs.First( x => x.UnitTypeId == unitTypeId );

			if( unitPrefab == null )
			{
				throw new InvalidOperationException($"Unit Type {unitTypeId} was not properly defined.");
			}

			return unitPrefab;
		}
	}
}
