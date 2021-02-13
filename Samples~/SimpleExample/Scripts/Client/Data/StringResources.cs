using System;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{


	[Serializable]
	public class StringResources
	{
		[Header( "Common Resources" )]

		[Space(8)]
		[Header("Connect To Server Context")]
		public string wellKnownHostsLabel;
		public string serverAddressLabel;
		public string serverPortLabel;
		public string loginLabel;

		public string WellKnownHostsLabel => wellKnownHostsLabel;
		public string ServerAddressLabel => serverAddressLabel;
		public string ServerPortLabel => serverPortLabel;
		public string LoginLabel => loginLabel;

		[Space( 8 )]
		[Header( "Enter World" )]
		public string acquireServerDataText;
		public string loadingWorldText;

		public string AcquireServerDataText => acquireServerDataText;
		public string LoadingWorldText => loadingWorldText;
	}
}