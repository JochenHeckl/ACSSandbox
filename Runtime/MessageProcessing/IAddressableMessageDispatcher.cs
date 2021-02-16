using System;

namespace de.JochenHeckl.Unity.ACSSandbox
{
	public interface IAddressableMessageDispatcher<AddressType> where AddressType : struct
	{
		void RegisterHandler<MessageType>( Action<AddressType, MessageType> messageHandler ) where MessageType : class;
		void DeregisterHandler<MessageType>( Action<AddressType, MessageType> messageHandler ) where MessageType : class;

		void DispatchMessage( AddressType clientId, object message );
	}
}
