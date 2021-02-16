using System;
using System.Collections.Generic;

using de.JochenHeckl.Unity.ACSSandbox.Example.Common;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	internal class ServerRuntimeData : IServerRuntimeData
	{
		public Guid WorldId { get; set; }
		public World World { get; set; }

		public Transform WorldRoot { get; set; }

		public float ServerIntegrationTimeSec { get; set; }

		public IList<int> ConnectedClients { get; set; } = new List<int>();
		public IList<AuthenticatedClient> AuthenticatedClients { get; set; } = new List<AuthenticatedClient>();
		public IDictionary<long, (IServerUnitData unitData, ServerUnitView unitView)> Units { get; set; } =
			new Dictionary<long, (IServerUnitData unitData, ServerUnitView unitView)>();
		public int NextUnitId { get; set; } = 0;
	}
}