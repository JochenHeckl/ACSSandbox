using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal interface IServerRuntimeData
	{
		IList<int> ConnectedClients { get; }
		IList<AuthenticatedClient> AuthenticatedClients { get; }
	}
}