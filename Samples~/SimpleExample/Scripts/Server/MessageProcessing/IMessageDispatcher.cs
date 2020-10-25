using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	public interface IMessageDispatcher
	{
		void RegisterHandler<MessageType>( Action<int, MessageType> messageHandler ) where MessageType : class;
		void UnregisterHandler<MessageType>( Action<int, MessageType> messageHandler ) where MessageType : class;

		void DispatchMessage( int clientId, object message );
	}
}
