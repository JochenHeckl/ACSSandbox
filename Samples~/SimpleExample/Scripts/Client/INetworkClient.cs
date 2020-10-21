using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal interface INetworkClient : IClientSystem
	{
		bool IsConnected { get; }

		void Connect( string serverAddress, int serverPort );
		void ResetConnection();
		void Send( byte[] message );
		IEnumerable<byte[]> FecthInboundMessages();
	}
}