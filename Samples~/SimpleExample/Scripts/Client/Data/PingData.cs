namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	class PingData
	{
		public float LastPingTimeMS { get; set; }
		public float MovingAverage5PingMS { get; set; }
		
		public float ServerIntegrationTimeSec { get; set; }
	}
}
