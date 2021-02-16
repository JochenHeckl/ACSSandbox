using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ServerToClient
{
	public class ServerDataResponse
	{
		public float UptimeSec { get; set; }
		public int LoggedInUserCount { get; set; }
		public Guid WorldId { get; set; }
	}
}
