using System;
using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	public class TypeMapMessageDispatcher : IMessageDispatcher
	{
		private readonly Dictionary<Type, Action<int, object>> messageHandlerMap;

		public TypeMapMessageDispatcher()
		{
			messageHandlerMap = new Dictionary<Type, Action<int, object>>();
		}

		public void RegisterHandler<MessageType>( Action<int, MessageType> messageHandler ) where MessageType : class
		{
			messageHandlerMap[typeof( MessageType )] = ( clientId, message ) => messageHandler( clientId, (MessageType) message );
		}

		public void UnregisterHandler<MessageType>( Action<int, MessageType> messageHandler ) where MessageType : class
		{
			if ( !messageHandlerMap.Remove( typeof( MessageType ) ) )
			{
				throw new InvalidOperationException( $"No handler was registered for messages of type {typeof( MessageType )}." );
			}
		}

		public void DispatchMessage( int clientId, object message )
		{
			if ( messageHandlerMap.TryGetValue( message.GetType(), out var handler ) )
			{
				handler( clientId, message );
			}
			else
			{
				throw new InvalidOperationException( $"No handler was registered to process message of type {message.GetType()}." );
			}
		}
	}
}