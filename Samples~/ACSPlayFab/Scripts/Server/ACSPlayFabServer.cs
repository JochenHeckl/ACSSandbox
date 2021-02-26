using UnityEngine;
using PlayFab;
using System.Collections;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.ACSPlayFab.Server
{
	public class ACSPlayFabServer : MonoBehaviour
	{
		public string playFabTitleId;
		public string playFabDeveloperSecretKey;
		public int serverPort;
		private INetworkServer server;

		public void Start()
		{
			StartCoroutine( ShutdownIn5Minutes() );

			PlayFabSettings.TitleId = playFabTitleId;
			PlayFabSettings.DeveloperSecretKey = playFabDeveloperSecretKey;

			PlayFabMultiplayerAgentAPI.Start();
			PlayFabMultiplayerAgentAPI.IsDebugging = false;

			PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += HandleOnMaintenance;
			PlayFabMultiplayerAgentAPI.OnShutDownCallback += HandleOnShutdown;
			PlayFabMultiplayerAgentAPI.OnServerActiveCallback += HandleOnServerActive;
			PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += HandleOnAgentError;

			PlayFabMultiplayerAgentAPI.ReadyForPlayers();
		}

		private IEnumerator ShutdownIn5Minutes()
		{
			yield return new WaitForSeconds( 60 * 5.0f );
			Application.Quit();
		}

		private void HandleOnMaintenance( System.DateTime? NextScheduledMaintenanceUtc )
		{
			Debug.Log( $"Received MaintenaceNotification for {NextScheduledMaintenanceUtc}" );
		}

		private void HandleOnShutdown()
		{
			Debug.Log("Shutting down.");

			if ( server != null )
			{
				server.StopServer();
			}
		}

		private void HandleOnServerActive()
		{
			server = new NetworkServerUnityTransport();
			server.StartServer( serverPort, maxMessageBufferSizeByteIn : 1024 );
		}

		private void HandleOnAgentError( string error )
		{
			Debug.LogError( error );
		}

		public void OnDestroy()
		{
			server.StopServer();
		}

		public void Update()
		{
			server.ProcessNetworkEvents();

			// echo all messages back to the client
			var inboundMessages = server.FetchInboundMessages();

			foreach ( var inboundMessage in inboundMessages )
			{
				server.Send( inboundMessage.clientConnectionId, inboundMessage.message );
			}
		}
	}
}