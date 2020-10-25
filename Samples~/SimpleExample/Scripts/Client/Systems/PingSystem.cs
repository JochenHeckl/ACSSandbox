using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class PingSystem : IClientSystem
	{
		private readonly ClientConfiguration configuration;
		private readonly IClientRuntimeData runtimeData;
		private readonly INetworkClient networkClient;
		private readonly IMessageSerializer messageSerializer;
		private readonly IMessageDispatcher messageDispatcher;

		private float nextPingTimeSec;
		private int pingResponseCounter;

		public PingSystem(
			ClientConfiguration configurationIn,
			IClientRuntimeData runtimeDataIn,
			INetworkClient networkClientIn,
			IMessageSerializer messageSerializerIn,
			IMessageDispatcher messageDispatcherIn )
		{
			configuration = configurationIn;
			runtimeData = runtimeDataIn;
			networkClient = networkClientIn;
			messageSerializer = messageSerializerIn;
			messageDispatcher = messageDispatcherIn;
		}

		public void Initialize()
		{
			pingResponseCounter = 0;

			runtimeData.PingData = new PingData()
			{
				LastPingTimeMS = 0f,
				MovingAverage5PingMS = 0f,
				ServerIntegrationTimeSec = 0f
			};

			messageDispatcher.RegisterHandler<PingResponse>( HandlePingResponse );
		}

		public void Shutdown()
		{

		}

		public void Update( float deltaTimeSec )
		{
			if ( networkClient.IsConnected && (runtimeData.TimeSec > nextPingTimeSec) )
			{
				nextPingTimeSec += configuration.NetworkConnectionPingIntervalSec;

				Send( new PingRequest()
				{
					PingRequestTimeSec = runtimeData.TimeSec
				} );
			}
		}

		private void HandlePingResponse( PingResponse message )
		{
			var pingTimeSec = runtimeData.TimeSec - message.PingRequestTimeSec;

			if ( pingResponseCounter == 5 )
			{
				runtimeData.PingData = new PingData()
				{
					LastPingTimeMS = pingTimeSec,
					MovingAverage5PingMS = (runtimeData.PingData.MovingAverage5PingMS + pingTimeSec) / 6f,
					ServerIntegrationTimeSec = message.ServerIntegrationTimeSec
				};
			}
			else
			{
				pingResponseCounter++;

				runtimeData.PingData = new PingData()
				{
					LastPingTimeMS = pingTimeSec,
					MovingAverage5PingMS = (runtimeData.PingData.MovingAverage5PingMS + pingTimeSec) / pingResponseCounter
				};
			}
		}

		private void Send( object message )
		{
			networkClient.Send( messageSerializer.Serialize( message ) );
		}
	}
}
