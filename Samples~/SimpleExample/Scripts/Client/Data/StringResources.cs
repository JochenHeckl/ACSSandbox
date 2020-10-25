using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	[Serializable]
	public class StringResources
	{
		public string wellKnownHostsLabel;
		public string WellKnownHostsLabel => wellKnownHostsLabel;

		public string serverAddressLabel;
		public string ServerAddressLabel => serverAddressLabel;

		public string serverPortLabel;
		public string ServerPortLabel => serverPortLabel;

		public string loginLabel;
		public string LoginLabel => loginLabel;
	}
}