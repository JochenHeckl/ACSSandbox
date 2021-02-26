using System;
using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox
{
	public interface INetworkServer
	{
		int[] ClientIds
		{
			get;
		}

		void StartServer( int serverPortIn, int maxMessageBufferSizeByteIn );
		void StopServer();

		void ProcessNetworkEvents( Action<int> clientConnectedCallback, Action<int> clientDisconnectedCallback );
		IEnumerable<(int clientConnectionId, byte[] message)> FetchInboundMessages();

		void Send( int clientConnectionId, byte[] message );
		void Send( int[] clientConnectionIds, byte[] message );
	}
}