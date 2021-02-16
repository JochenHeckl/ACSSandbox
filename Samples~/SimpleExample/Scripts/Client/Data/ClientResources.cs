using System;
using System.Linq;

using UnityEngine;

using de.JochenHeckl.Unity.ACSSandbox.Example.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	[CreateAssetMenu( fileName = "ClientResources", menuName = "ACS Sandbox/ClientResources", order = 1 )]
	public class ClientResources : ScriptableObject, IClientResources
	{
		public ContextUIView connectToServerView;
		public ContextUIView ConnectToServerView => connectToServerView;

		public ContextUIView enterWorldView;
		public ContextUIView EnterWorldView => enterWorldView;

		
		public Camera lobbyCamera;
		public Camera LobbyCamera => lobbyCamera;

		public WorldCamera worldCamera;
		public WorldCamera WorldCamera => worldCamera;


		public StringResources stringResources;
		public StringResources StringResources => stringResources;

		public World[] worlds;
		public ClientUnitView[] unitPrefabs;

		public World GetWorld( Guid worldId )
		{
			return worlds.FirstOrDefault( x => x.WorldId == worldId );
		}

		public ClientUnitView GetUnitPrefab( UnitTypeId unitTypeId )
		{
			var unitPrefab = unitPrefabs.FirstOrDefault( x => x.UnitTypeId == unitTypeId );

			if (unitPrefab == null )
			{
				throw new InvalidOperationException( $"Unit Type {unitTypeId} was not properly defined." );
			}

			return unitPrefab;
		}
	}
}
