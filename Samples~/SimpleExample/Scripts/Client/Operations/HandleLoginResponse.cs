using de.JochenHeckl.Unity.ACSSandbox.Example.Common;
using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ServerToClient;

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