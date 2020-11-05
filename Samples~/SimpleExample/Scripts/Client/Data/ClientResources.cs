using System;
using System.Linq;

using UnityEngine;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
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

		public Camera worldCamera;
		public Camera WorldCamera => worldCamera;


		public StringResources stringResources;
		public StringResources StringResources => stringResources;

		public World[] worlds;

		public World GetWorld( Guid worldId )
		{
			return worlds.FirstOrDefault( x => x.WorldId == worldId );
		}
	}
}
