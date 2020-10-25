using System;
using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public class TypeMapMessageDispatcher : IMessageDispatcher
	{
		private readonly Dictionary<Type, Action<object>> messageHandlerMap;

		public TypeMapMessageDispatcher()
		{
			messageHandlerMap = new Dictionary<Type, Action<object>>();
		}

		public void RegisterHandler<MessageType>( Action<MessageType> messageHandler ) where MessageType : class
		{
			messageHandlerMap[typeof( MessageType )] = ( message ) => messageHandler( (MessageType) message );
		}

		public void UnregisterHandler<MessageType>( Action<MessageType> messageHandler ) where MessageType : class
		{
			if( !messageHandlerMap.Remove( typeof( MessageType ) ) )
			{
				throw new InvalidOperationException( $"No handler was registered for messages of type {typeof( MessageType )}." );
			}
		}

		public void DispatchMessage( object message )
		{
			if ( messageHandlerMap.TryGetValue( message.GetType(), out var handler ) )
			{
				handler( message );
			}
			else
			{
				throw new InvalidOperationException( $"No handler was registered to process message of type {message.GetType()}." );
			}
		}
	}
}