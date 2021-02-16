using System;
using System.Linq;
using System.IO;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class ServerNetworkMessageDispatcher : TypeMapAddressableMessageDispatcher<int>, IServerSystem
	{
		private readonly IServerRuntimeData runtimeData;
		private readonly INetworkServer networkServer;
		private readonly IMessageSerializer messageSerializer;

		private readonly Type[] unrestrictedMessageTypes =
		{
			typeof( Ping ),
			typeof( LoginRequest ),
			typeof( ServerDataRequest )
		};

		private readonly TimeSampler<Type> dispatchTimes = new TimeSampler<Type>();

		public ServerNetworkMessageDispatcher(
			IServerRuntimeData runtimeDataIn,
			INetworkServer networkServerIn,
			IMessageSerializer messageSerializerIn )
		{
			runtimeData = runtimeDataIn;
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

				var noAuthorizationRequired = unrestrictedMessageTypes.Contains( message.GetType() );
				var clientIsAuthorized = runtimeData.AuthenticatedClients.Any( x => x.ClientConnectionId == messageRaw.clientConnectionId );

				if ( noAuthorizationRequired || clientIsAuthorized )
				{
					DispatchMessage( messageRaw.clientConnectionId, message );
				}
				else
				{
					Debug.LogWarning( $"restricted message {message.GetType()} received from unauthorized client { messageRaw.clientConnectionId }." +
						$" Message type was {message.GetType()}" );
				}

				dispatchTimes.StopSample( message.GetType() );
			}
		}
	}
}
