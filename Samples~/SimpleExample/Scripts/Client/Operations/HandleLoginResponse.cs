using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal partial class ClientOperations : IClientOperations
	{
		public void HandleLoginResonse( LoginResponse message )
		{
			runtimeData.IsAuthenticated = message.LoginResult == LoginResult.OK;
			Send( new ServerDataRequest() );
		}
	}
}