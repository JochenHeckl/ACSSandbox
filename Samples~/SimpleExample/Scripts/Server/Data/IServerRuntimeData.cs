using System;
using System.Collections.Generic;

using de.JochenHeckl.Unity.ACSSandbox.Common;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal interface IServerRuntimeData
	{
		float ServerIntegrationTimeSec { get; set; }
		
		IList<int> ConnectedClients { get; }
		IList<AuthenticatedClient> AuthenticatedClients { get; }

		Guid WorldId { get; set; }
		Transform WorldRoot { get; }
		World World { get; set; }

		IDictionary<long, (IServerUnitData unitData, ServerUnitView unitView)> Units { get; set; }
	}
}