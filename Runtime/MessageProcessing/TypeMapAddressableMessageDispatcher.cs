using System;
using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox
{
	public class TypeMapAddressableMessageDispatcher<AddressType> 
		: IAddressableMessageDispatcher<AddressType> where AddressType : struct
	{
		private readonly Dictionary<Type, Action<AddressType, object>> messageHandlerMap;

		public TypeMapAddressableMessageDispatcher()
		{
			messageHandlerMap = new Dictionary<Type, Action<AddressType, object>>();
		}

		public void RegisterHandler<MessageType>( Action<AddressType, MessageType> messageHandler ) where MessageType : class
		{
			messageHandlerMap[typeof( MessageType )] = ( address, message ) => messageHandler( address, (MessageType) message );
		}

		public void DeregisterHandler<MessageType>( Action<AddressType, MessageType> messageHandler ) where MessageType : class
		{
			if ( !messageHandlerMap.Remove( typeof( MessageType ) ) )
			{
				throw new InvalidOperationException( $"No handler was registered for messages of type {typeof( MessageType )}." );
			}
		}

		public void DispatchMessage( AddressType address, object message )
		{
			if ( messageHandlerMap.TryGetValue( message.GetType(), out var handler ) )
			{
				handler( address, message );
			}
			else
			{
				throw new InvalidOperationException( $"No handler was registered to process message of type {message.GetType()}." );
			}
		}
	}
}