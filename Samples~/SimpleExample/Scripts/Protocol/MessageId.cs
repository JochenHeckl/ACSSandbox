using System;

using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

namespace de.JochenHeckl.Unity.ACSSandbox.Protocol
{
	public static class MessageIds
	{
		public static (byte messageId, Type messageType)[] ClientToServerMessageIds => new (byte messageId, Type messageType)[]
			{
				(1, typeof( PingRequest )),
				(3, typeof( LoginRequest )),
				(5, typeof( ServerDataRequest )),
				(8, typeof( SpawnRequest )),
				(11, typeof( NavigateToPositionRequest ))
			};

		public static (byte messageId, Type messageType)[] ServerToClientMessageIds => new (byte messageId, Type messageType)[]
			{
				(2, typeof( PingResponse )),
				(4, typeof( LoginResponse )),
				(6, typeof( ServerDataResponse )),
				(7, typeof( SpawnResponse ) ),
				(9, typeof( WorldState )),
				(10, typeof( UnitSync )),
				(12, typeof( NavigateToPositionResponse ))
			};
	}
}