using System;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

using UnityEditor;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal class ClientOperations : IClientOperations
	{
		private readonly ClientConfiguration configuration;
		private readonly IClientResources resources;
		private readonly IClientRuntimeData runtimeData;
		private readonly INetworkClient networkClient;
		private readonly IMessageSerializer messageSerializer;
		private readonly IMessageDispatcher messageDispatcher;

		public ClientOperations(
			ClientConfiguration configurationIn,
			IClientResources resourcesIn,
			IClientRuntimeData runtimeDataIn,
			INetworkClient networkClientIn,
			IMessageSerializer messageSerializerIn,
			IMessageDispatcher messageDispatcherIn )
		{
			configuration = configurationIn;
			resources = resourcesIn;
			runtimeData = runtimeDataIn;
			networkClient = networkClientIn;
			messageSerializer = messageSerializerIn;
			messageDispatcher = messageDispatcherIn;
		}

		public void RegisterHandler<MessageType>( Action<MessageType> handler ) where MessageType : class
		{
			messageDispatcher.RegisterHandler( handler );
		}
	
		public void DeregisterHandler<MessageType>(Action<MessageType> handler ) where MessageType : class
		{
			messageDispatcher.DeregisterHandler( handler );
		}

		public void Send( object message )
		{
			networkClient.Send( messageSerializer.Serialize( message ) );
		}
	}
}
