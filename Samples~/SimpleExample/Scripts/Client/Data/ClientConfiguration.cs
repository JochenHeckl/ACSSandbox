namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	public class NetworkConfiguration
	{
		public bool AutoConnect { get; set; }
		public string AutoConnectServerAddress { get; set; }
		public int AutoConnectServerPort { get; set; }
		public float NetworkConnectionRetryIntervalSec { get; set; }
	}

	public class ClientConfiguration
	{
		public class ServerConnectionData
		{
			public string DisplayName { get; set; }
			public string ServerAddress { get; set; }
			public int ServerPort { get; set; }
		}

		public int FPSLimit { get; set; }

		public NetworkConfiguration NetworkConfiguration { get; set; }
		public ServerConnectionData[] WellKnownServers { get; set; }

		public float NetworkConnectionPingIntervalSec { get; set; }
		public float NetworkConnectionTimeoutSec { get; set; }
	}
}