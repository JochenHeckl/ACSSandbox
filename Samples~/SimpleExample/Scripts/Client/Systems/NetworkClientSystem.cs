using System;
using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal class NetworkClientSystem : IClientSystem
    {
		private float nextNetworkConnectionRetrySec;
		private NetworkConfiguration configuration;
		private IClientRuntimeData runtimeData;
		private INetworkClient networkClient;

		public NetworkClientSystem(
			ClientConfiguration configurationIn,
			IClientRuntimeData runtimeDataIn,
			INetworkClient networkClientIn )
		{
			configuration = configurationIn.NetworkConfiguration;
			runtimeData = runtimeDataIn;

			networkClient = networkClientIn;
		}

		public void Initialize()
		{
			if ( configuration.AutoConnect )
			{
				if ( string.IsNullOrEmpty( configuration.AutoConnectServerAddress ) || (configuration.AutoConnectServerPort == 0) )
				{
					throw new InvalidOperationException( "Auto connect was requested, but auto connect server was not configured properly." );
				}

				runtimeData.ServerAddress = configuration.AutoConnectServerAddress;
				runtimeData.ServerPort = configuration.AutoConnectServerPort;

				networkClient.Connect( configuration.AutoConnectServerAddress, configuration.AutoConnectServerPort );
				nextNetworkConnectionRetrySec = Time.realtimeSinceStartup + configuration.NetworkConnectionRetryIntervalSec;
			}
		}

		public void Shutdown()
		{
			networkClient.ResetConnection();
		}

		public void Update( float deltaTime )
		{
			if ( networkClient.IsConnected )
			{
				nextNetworkConnectionRetrySec = Time.realtimeSinceStartup + configuration.NetworkConnectionRetryIntervalSec;
			}

			networkClient.UpdateProcessing();

			if ( TextTryReconnect() )
			{
				Debug.Log( "reconnecting to server..." );

				networkClient.ResetConnection();
				networkClient.Connect( runtimeData.ServerAddress, runtimeData.ServerPort );
			}
		}

		private bool TextTryReconnect()
		{
			return !networkClient.IsConnected
				&& (Time.realtimeSinceStartup > nextNetworkConnectionRetrySec)
				&& !String.IsNullOrEmpty( runtimeData.ServerAddress )
				&& runtimeData.ServerPort > 0;
		}
	}
}
