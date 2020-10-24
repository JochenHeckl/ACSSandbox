using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
	public interface IMessageDispatcher
	{
		void RegisterHandler<MessageType>( Action<MessageType> messageHandler ) where MessageType : class;
		void UnregisterHandler<MessageType>( Action<MessageType> messageHandler ) where MessageType : class;

		void DispatchMessage( object message );
	}
}
