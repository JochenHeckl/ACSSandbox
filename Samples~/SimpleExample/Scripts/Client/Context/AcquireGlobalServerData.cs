using System;

using de.JochenHeckl.Unity.ACSSandbox.Client;
using de.JochenHeckl.Unity.ACSSandbox.Example;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
	internal class AcquireGlobalServerData : IContext
	{
		private IClientRuntimeData runtimeData;
		private INetworkClient networkClient;
		private IMessageSerializer messageSerializer;
		private IMessageDispatcher messageDispatcher;

		public AcquireGlobalServerData( 
			IClientRuntimeData runtimeDataIn,
			INetworkClient networkClientIn,
			IMessageSerializer messageSerializerIn,
			IMessageDispatcher messageDispatcherIn )
		{
			runtimeData = runtimeDataIn;
			networkClient = networkClientIn;
			messageSerializer = messageSerializerIn;
			messageDispatcher = messageDispatcherIn;
		}

		public void EnterContext( IContextContainer contextContainer )
		{
			runtimeData.GlobalServerData = null;

			messageDispatcher.RegisterHandler<GlobalServerDataResponse>( HandleGlobalServerDataResponse );

			Send( new GlobalServerDataRequest() );
		}

		public void LeaveContext( IContextContainer contextContainer )
		{
			
		}

		public void Update( IContextContainer contextContainer, float deltaTimeSec )
		{
			if( runtimeData.GlobalServerData != null )
			{
				// start match
			}
		}

		private void HandleGlobalServerDataResponse( GlobalServerDataResponse message )
		{
			runtimeData.GlobalServerData = new GlobalServerData()
			{
				UptimeSec = message.UptimeSec
			};
		}

		private void Send( object message )
		{
			networkClient.Send( messageSerializer.Serialize( message ) );
		}
	}
}
