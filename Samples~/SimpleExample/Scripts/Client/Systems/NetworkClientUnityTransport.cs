using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using de.JochenHeckl.Unity.ACSSandbox.Common;

using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class NetworkClientUnityTransport : INetworkClient
	{
		public bool IsConnected { get; private set; }
		private Queue<byte[]> inboundMessages = new Queue<byte[]>();

		private NetworkDriver networkDriver;
		private NetworkPipeline pipeline;
		private NetworkConnection networkConnection;
		private JobHandle processingJobHandle = new JobHandle();

		private ClientConfiguration configuration;
		private IClientRuntimeData runtimeData;

		private string serverAddress;
		private int serverPort;

		private float nextNetworkConnectionRetrySec;

		public NetworkClientUnityTransport(
			ClientConfiguration configurationIn,
			IClientRuntimeData runtimeDataIn )
		{
			configuration = configurationIn;
			runtimeData = runtimeDataIn;
		}

		public void Initialize()
		{
			if ( configuration.AutoConnect )
			{
				if ( string.IsNullOrEmpty( configuration.AutoConnectServerAddress ) || (configuration.AutoConnectServerPort == 0) )
				{
					throw new InvalidOperationException( "Auto connect was requested, but auto connect server was not configured properly." );
				}

				Connect( configuration.AutoConnectServerAddress, configuration.AutoConnectServerPort );
				nextNetworkConnectionRetrySec = Time.realtimeSinceStartup + configuration.NetworkConnectionRetryIntervalSec;
			}
		}

		public void Shutdown()
		{
			ResetConnection();
		}

		public void Update( float deltaTime )
		{
			if ( !networkDriver.IsCreated )
			{
				return;
			}

			ProcessNetworkEvents();

			if ( !IsConnected && (runtimeData.TimeSec > nextNetworkConnectionRetrySec) )
			{
				Debug.Log( "reconnecting to server..." );

				ResetConnection();
				Connect( serverAddress, serverPort );
			}
		}

		public void Send( byte[] message )
		{
			if ( networkDriver.IsCreated )
			{
				// TODO: Implement this as
				// processingJobHandle = SendMessageJob.Shedule( processingJobHandle );

				processingJobHandle.Complete();

				var writer = networkDriver.BeginSend( networkConnection );

				if ( writer.IsCreated )
				{

					using ( var nativeMessage = new NativeArray<byte>( message, Allocator.Temp ) )
					{
						writer.WriteBytes( nativeMessage );
					}

					networkDriver.EndSend( writer );
				}
			}
		}

		public IEnumerable<byte[]> FecthInboundMessages()
		{
			while ( inboundMessages.Any() )
			{
				yield return inboundMessages.Dequeue();
			}

			inboundMessages.Clear();
		}

		public void Connect( string serverAddressIn, int serverPortIn )
		{
			if ( IsConnected )
			{
				throw new InvalidOperationException( "This client is connected to a server already." );
			}
			else
			{
				serverAddress = serverAddressIn;
				serverPort = serverPortIn;

				nextNetworkConnectionRetrySec = runtimeData.TimeSec + configuration.NetworkConnectionRetryIntervalSec;
			}

			var serverIp = Dns.GetHostEntry( serverAddress ).AddressList
				.Where( x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork )
				.Select( x => x.ToString() )
				.FirstOrDefault();

			if ( serverIp == null )
			{
				throw new InvalidOperationException( $"Failed to resolve server host name {serverAddress}:{serverPort}." );
			}

			networkDriver = NetworkDriver.Create( new ReliableUtility.Parameters { WindowSize = 32 } );
			pipeline = networkDriver.CreatePipeline( typeof( ReliableSequencedPipelineStage ) );

			var endpoint = NetworkEndPoint.Parse( serverIp, (ushort) serverPort );
			networkConnection = networkDriver.Connect( endpoint );
		}

		public void ResetConnection()
		{
			processingJobHandle.Complete();

			if ( networkDriver.IsCreated && networkConnection.IsCreated && IsConnected )
			{
				networkConnection.Disconnect( networkDriver );
				networkDriver.ScheduleUpdate().Complete();
			}

			if ( networkDriver.IsCreated )
			{
				networkDriver.Dispose();
			}

			IsConnected = false;
		}

		private void ProcessNetworkEvents()
		{
			processingJobHandle.Complete();

			ProcessNetworkEvents( networkDriver, networkConnection );

			processingJobHandle = networkDriver.ScheduleUpdate();
		}

		private void ProcessNetworkEvents( NetworkDriver networkDriver, NetworkConnection networkConnection )
		{
			DataStreamReader stream;
			NetworkEvent.Type networkEventType;

			while ( (networkEventType = networkDriver.PopEventForConnection( networkConnection, out stream )) != NetworkEvent.Type.Empty )
			{
				if ( networkEventType == NetworkEvent.Type.Connect )
				{
					IsConnected = true;

					Debug.Log( "Network connection established." );
				}

				if ( networkEventType == NetworkEvent.Type.Disconnect )
				{
					IsConnected = false;
					networkConnection = default;

					Debug.Log( "Network connection lost." );
				}

				if ( networkEventType == NetworkEvent.Type.Data )
				{
					using ( var messageBuffer = new NativeArray<byte>( stream.Length, Allocator.Temp ) )
					{
						stream.ReadBytes( messageBuffer );
						inboundMessages.Enqueue( messageBuffer.ToArray() );
					}
				}
			}
		}
	}
}
