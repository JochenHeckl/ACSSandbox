using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Protocol
{
	public class ServerDataResponse
	{
		public float UptimeSec { get; set; }
		public int LoggedInUserCount { get; set; }
		public Guid WorldId { get; set; }
	}
}
