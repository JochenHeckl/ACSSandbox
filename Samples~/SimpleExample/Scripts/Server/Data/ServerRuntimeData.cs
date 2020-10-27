using System;
using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class ServerRuntimeData : IServerRuntimeData
	{
		public Guid WorldId { get; set; }
		public float ServerIntegrationTimeSec { get; set; }

		public IList<int> ConnectedClients { get; set; } = new List<int>();
		public IList<AuthenticatedClient> AuthenticatedClients { get; set; } = new List<AuthenticatedClient>();
	}
}