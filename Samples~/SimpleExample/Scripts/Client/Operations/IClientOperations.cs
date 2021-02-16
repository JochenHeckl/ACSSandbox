using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	interface IClientOperations
	{
		void Send( object message );
		void RegisterHandler<MessageType>( Action<MessageType> handler ) where MessageType : class;
		void DeregisterHandler<MessageType>( Action<MessageType> handler ) where MessageType : class;
	}
}
