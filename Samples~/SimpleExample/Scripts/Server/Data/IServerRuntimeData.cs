using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal interface IServerRuntimeData
	{
		float ServerIntegrationTimeSec { get; set; }

		IList<int> ConnectedClients { get; }
		IList<AuthenticatedClient> AuthenticatedClients { get; }
	}
}