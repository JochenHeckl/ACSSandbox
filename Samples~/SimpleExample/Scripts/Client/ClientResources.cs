using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	[CreateAssetMenu( fileName = "ClientResources", menuName = "ACS Sandbox/ClientResources", order = 1 )]
	public class ClientResources : ScriptableObject, IClientResources
	{
		public LoginView loginView;
		public LoginView LoginView => loginView;

		public StringResources stringResources;
		public StringResources StringResources => stringResources;
	}
}
