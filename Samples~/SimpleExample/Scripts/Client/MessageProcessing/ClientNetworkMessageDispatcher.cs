using System;
using System.IO;

using de.JochenHeckl.Unity.ACSSandbox.Example.Common;
using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal class ClientNetworkMessageDispatcher : TypeMapMessageDispatcher, IClientSystem
	{
		private INetworkClient networkClient;
		private IMessageSerializer messageSerializer;

		private TimeSampler<Type> dispatchTimes = new TimeSampler<Type>();

		public ClientNetworkMessageDispatcher(
			INetworkClient networkClientIn,
			IMessageSerializer messageSerializerIn )
		{
			networkClient = networkClientIn;
			messageSerializer = messageSerializerIn;
		}

		public void Initialize()
		{
			foreach ( var message in MessageIds.ServerToClientMessageIds )
			{
				dispatchTimes.InitSample( message.messageType );
			}
		}

		public void Shutdown()
		{
			File.WriteAllLines( "clientNetworkMessageDispatch.TimeSamples.md", dispatchTimes.MarkDownSamples(
				"Client network message dispatching",
				( x ) => x.Name ) );
		}

		public void Update( float deltaTimeSec )
		{
			if ( networkClient.IsConnected )
			{

				foreach ( var messageRaw in networkClient.FecthInboundMessages() )
				{
					var message = messageSerializer.Deserialize( messageRaw );

					dispatchTimes.StartSample();

					DispatchMessage( message );

					dispatchTimes.StopSample( message.GetType() );
				}
			}
		}
	}
}
