using System;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

using Debug = UnityEngine.Debug;
using System.IO;
using de.JochenHeckl.Unity.ACSSandbox.Protocol;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class ServerNetworkMessageDispatcher : TypeMapMessageDispatcher, IServerSystem
	{
		private INetworkServer networkServer;
		private IMessageSerializer messageSerializer;

		private TimeSampler<Type> dispatchTimes = new TimeSampler<Type>();

		public ServerNetworkMessageDispatcher(
			INetworkServer networkServerIn,
			IMessageSerializer messageSerializerIn )
		{
			networkServer = networkServerIn;
			messageSerializer = messageSerializerIn;
		}

		public void Initialize()
		{
			foreach ( var message in MessageIds.ClientToServerMessageIds )
			{
				dispatchTimes.InitSample( message.messageType );
			}
		}

		public void Shutdown()
		{
			File.WriteAllLines( "serverNetworkMessageDispatch.TimeSamples.md", dispatchTimes.MarkDownSamples(
				"Server network message dispatching",
				( x ) => x.Name ) );
		}

		public void Update( float deltaTimeSec )
		{
			foreach ( var messageRaw in networkServer.FetchInboundMessages() )
			{
				dispatchTimes.StartSample();

				var message = messageSerializer.Deserialize( messageRaw.message );
				DispatchMessage( messageRaw.clientConnectionId, message );

				dispatchTimes.StopSample( message.GetType() );
			}
		}
	}
}
