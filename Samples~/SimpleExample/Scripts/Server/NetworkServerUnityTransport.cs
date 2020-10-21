﻿using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal struct ServerUpdateConnectionsJob : IJob
	{
		public NetworkDriver networkDriver;
		public NativeList<NetworkConnection> networkConnections;

		public void Execute()
		{
			// Clean up connections

			for ( var connectionIndex = 0; connectionIndex < networkConnections.Length; connectionIndex++ )
			{
				if ( !networkConnections[connectionIndex].IsCreated )
				{
					networkConnections.RemoveAtSwapBack( connectionIndex );
					--connectionIndex;
				}
			}

			// Accept new connections

			NetworkConnection networkConnection;

			while ( (networkConnection = networkDriver.Accept()) != default )
			{
				networkConnections.Add( networkConnection );
			}
		}
	}

	internal class NetworkServerUnityTransport : INetworkServer
	{
		private NetworkDriver networkDriver;
		private NetworkPipeline pipeline;

		private NativeList<NetworkConnection> connections;
		private JobHandle processingJobHandle;

		private readonly ServerConfiguration configuration;

		private readonly Queue<(int connectionId, byte[] message)> inboundMessages = new Queue<(int connectionId, byte[] message)>();

		public int[] ClientConnections { get; }

		public NetworkServerUnityTransport( ServerConfiguration configurationIn )
		{
			configuration = configurationIn;
		}

		public void Initialize()
		{
			if ( !connections.IsCreated )
			{
				connections = new NativeList<NetworkConnection>( Allocator.Persistent );
			}

			if ( !networkDriver.IsCreated )
			{
				networkDriver = NetworkDriver.Create( new ReliableUtility.Parameters { WindowSize = 32 } );
				pipeline = networkDriver.CreatePipeline( typeof( ReliableSequencedPipelineStage ) );
			}

			var endpoint = NetworkEndPoint.AnyIpv4;
			endpoint.Port = (ushort) configuration.ServerPort;

			var bindResult = networkDriver.Bind( endpoint );

			if ( bindResult != 0 )
			{
				throw new InvalidOperationException( $"Failed to bind networkServer to port {configuration.ServerPort}. BindResult is {bindResult}." );
			}

			networkDriver.Listen();

			Debug.Log( $"NetworkServer listening on local end point {networkDriver.LocalEndPoint().Address}." );
		}

		public void Shutdown()
		{
			processingJobHandle.Complete();

			if ( networkDriver.IsCreated )
			{
				if ( connections.IsCreated )
				{
					foreach ( var connection in connections.AsArray().Where( x => x.IsCreated ) )
					{
						networkDriver.Disconnect( connection );
					}

					connections.Dispose();
				}

				networkDriver.Dispose();
			}
			
			Debug.Log( $"NetworkServer successfully shut down." );
		}

		public void Update( float deltaTimeSec )
		{
			ProcessNetworkEvents();
		}

		public IEnumerable<(int clientConnectionId, byte[] message)> FetchInboundMessages()
		{
			while ( inboundMessages.Count > 0 )
			{
				var nextMessage = inboundMessages.Dequeue();

				yield return (nextMessage.connectionId, nextMessage.message.ToArray());
			}
		}

		public void ProcessNetworkEvents()
		{
			processingJobHandle.Complete();

			DataStreamReader inboundDataStream;
			NetworkEvent.Type networkEventType;

			for ( var connectionIndex = 0; connectionIndex < connections.Length; connectionIndex++ )
			{
				while ( (networkEventType = networkDriver.PopEventForConnection( connections[connectionIndex], out inboundDataStream ))
					!= NetworkEvent.Type.Empty )
				{
					// we do not get connects - this is handled by accepts
					// if ( networkEventType == NetworkEvent.Type.Connect ){}

					if ( networkEventType == NetworkEvent.Type.Disconnect )
					{
						connections[connectionIndex] = default;
					}

					if ( networkEventType == NetworkEvent.Type.Data )
					{
						if ( inboundDataStream.Length <= configuration.MaxMessageSizeByte )
						{
							using ( var readBuffer = new NativeArray<byte>( inboundDataStream.Length, Allocator.Temp ) )
							{
								inboundDataStream.ReadBytes( readBuffer );
								inboundMessages.Enqueue( (connections[connectionIndex].InternalId, readBuffer.ToArray()) );
							}
						}
						else
						{
							Debug.LogError( $"Max inbound message size of {configuration.MaxMessageSizeByte} exceeded " +
								$"by message from {connections[connectionIndex]}. Ignoring the message." );
						}
					}
				}
			}

			var connectionJob = new ServerUpdateConnectionsJob
			{
				networkDriver = networkDriver,
				networkConnections = connections
			};

			processingJobHandle = networkDriver.ScheduleUpdate();
			processingJobHandle = connectionJob.Schedule( processingJobHandle );
		}

		public void Send( int clientConnectionId, byte[] message )
		{
			Send( new int[] { clientConnectionId }, message );
		}

		public void Send( int[] clientConnectionIds, byte[] message )
		{
			if ( networkDriver.IsCreated )
			{
				processingJobHandle.Complete();

				var currentConnections = connections.ToArray();

				var adressees = clientConnectionIds
					.Select( clientConnectionId => currentConnections.FirstOrDefault( x => x.InternalId == clientConnectionId ) )
					.Where( x => x.IsCreated )
					.ToArray();

				using ( var messageNative = new NativeArray<byte>( message, Allocator.Temp ) )
				{
					foreach ( var connection in adressees )
					{
						var writer = networkDriver.BeginSend( connection, message.Length );
						writer.WriteBytes( messageNative );
						networkDriver.EndSend( writer );
					}
				}
			}
		}
	}
}
