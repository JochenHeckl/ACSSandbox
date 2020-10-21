using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal interface INetworkServer : IServerSystem
	{
		int[] ClientConnections
		{
			get;
		}

		void ProcessNetworkEvents();
		IEnumerable<(int clientConnectionId, byte[] message)> FetchInboundMessages();

		void Send( int clientConnectionId, byte[] message );
		void Send( int[] clientConnectionIds, byte[] message );
	}
}