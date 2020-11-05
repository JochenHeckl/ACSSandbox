using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	class ServerData
	{
		public float UptimeSec { get; set; }

		public int LoggedInUserCount { get; set; }
		public Guid WorldId { get; internal set; }
	}
}
