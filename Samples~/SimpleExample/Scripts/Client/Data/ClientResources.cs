using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	[CreateAssetMenu( fileName = "ClientResources", menuName = "ACS Sandbox/ClientResources", order = 1 )]
	public class ClientResources : ScriptableObject, IClientResources
	{
		public ConnectToServerView loginView;
		public ConnectToServerView ConnectToServerView => loginView;

		public StringResources stringResources;
		public StringResources StringResources => stringResources;
	}
}
