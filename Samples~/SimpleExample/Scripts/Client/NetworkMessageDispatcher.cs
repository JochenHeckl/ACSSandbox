using System;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class NetworkMessageDispatcher : TypeMapMessageDispatcher, IClientSystem
	{
		private struct TypeDispatchData
		{
			public int messageCount;
			public float maxDispatchTimeMS;
			public float averageDispatchTimeMS;
		}

		private INetworkClient networkClient;
		private IMessageSerializer messageSerializer;

		private Dictionary<Type, TypeDispatchData> dispatchData;

		public NetworkMessageDispatcher(
			INetworkClient networkClientIn,
			IMessageSerializer messageSerializerIn )
		{
			networkClient = networkClientIn;
			messageSerializer = messageSerializerIn;
		}

		public void Initialize()
		{
			dispatchData = new Dictionary<Type, TypeDispatchData>();
		}

		public void Shutdown()
		{
			foreach ( var message in dispatchData )
			{
				Debug.Log( $"{message.Value.messageCount} {message.Key.Name} messages dispatched at" +
					$" an average {message.Value.averageDispatchTimeMS} ms." +
					$" Peek dispatch time {message.Value.maxDispatchTimeMS}." );
			}
		}

		public void Update( float deltaTimeSec )
		{
			if ( networkClient.IsConnected )
			{
				var stopwatch = new Stopwatch();

				foreach ( var messageRaw in networkClient.FecthInboundMessages() )
				{
					var message = messageSerializer.Deserialize( messageRaw );

					stopwatch.Restart();

					DispatchMessage( message );

					var messageDispatchTimeMS = stopwatch.ElapsedMilliseconds;

					var curDispatchData = dispatchData[message.GetType()];
					dispatchData[message.GetType()] = new TypeDispatchData()
					{
						messageCount = curDispatchData.messageCount + 1,
						maxDispatchTimeMS = Mathf.Max( messageDispatchTimeMS, curDispatchData.maxDispatchTimeMS ),

						averageDispatchTimeMS =
							((curDispatchData.averageDispatchTimeMS * curDispatchData.messageCount) + messageDispatchTimeMS)
							/ (curDispatchData.messageCount + 1)
					};

					if ( messageDispatchTimeMS > 100f)
					{
						UnityEngine.Debug.LogWarning( $"Dispatching message of type {message.GetType()} too {messageDispatchTimeMS} ms." );
					}
				}
			}
		}
	}
}
