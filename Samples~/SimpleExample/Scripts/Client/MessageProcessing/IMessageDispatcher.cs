using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public interface IMessageDispatcher
	{
		void RegisterHandler<MessageType>( Action<MessageType> messageHandler ) where MessageType : class;
		void DeregisterHandler<MessageType>( Action<MessageType> messageHandler ) where MessageType : class;

		void DispatchMessage( object message );
	}
}
