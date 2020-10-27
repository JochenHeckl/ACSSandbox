using System;
using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal interface IServerRuntimeData
	{
		Guid WorldId { get; set; }
		float ServerIntegrationTimeSec { get; set; }

		IList<int> ConnectedClients { get; }
		IList<AuthenticatedClient> AuthenticatedClients { get; }
	}
}