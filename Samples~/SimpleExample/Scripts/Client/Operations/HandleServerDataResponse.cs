using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ServerToClient;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal partial class ClientOperations : IClientOperations
	{
		public void HandleServerDataResponse( ServerDataResponse message )
		{
			runtimeData.ServerData = new ServerData()
			{
				UptimeSec = message.UptimeSec,
				LoggedInUserCount = message.LoggedInUserCount,
				WorldId = message.WorldId,
			};
		}
	}
}