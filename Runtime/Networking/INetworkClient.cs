using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox
{
	public interface INetworkClient
	{
		bool IsConnected { get; }

		void Connect( string serverAddress, int serverPort );
		void ResetConnection();

		void UpdateProcessing();

		void Send( byte[] message );
		IEnumerable<byte[]> FecthInboundMessages();
	}
}