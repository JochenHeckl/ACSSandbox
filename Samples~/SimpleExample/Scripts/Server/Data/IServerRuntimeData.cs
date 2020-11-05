using System;
using System.Collections.Generic;

using de.JochenHeckl.Unity.ACSSandbox.Common;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal interface IServerRuntimeData
	{
		Guid WorldId { get; set; }
		World World { get; set; }

		Transform WorldRoot { get; }

		float ServerIntegrationTimeSec { get; set; }

		IList<int> ConnectedClients { get; }
		IList<AuthenticatedClient> AuthenticatedClients { get; }
	}
}