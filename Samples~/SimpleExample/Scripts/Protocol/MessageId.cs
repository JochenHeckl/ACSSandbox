using System;
using System.Linq;

namespace de.JochenHeckl.Unity.ACSSandbox.Example
{
	public static class MessageIds
	{
		public static (byte messageId, Type messageType)[] ClientToServerMessageIds => new (byte messageId, Type messageType)[]
			{
				(1, typeof( PingRequest )),
				(3, typeof( LoginRequest )),
				(5, typeof( GlobalServerDataRequest ))
			};

		public static (byte messageId, Type messageType)[] ServerToClientMessageIds => new (byte messageId, Type messageType)[]
			{
				(2, typeof( PingResponse )),
				(4, typeof( LoginResponse )),
				(6, typeof( GlobalServerDataResponse )),
				(7, typeof( WorldState ))
			};
	}
}