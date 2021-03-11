using System.Collections;
using System.Collections.Generic;
using System.Text;

using de.JochenHeckl.Unity.ACSSandbox;

using UnityEngine;

using PlayFab;
using PlayFab.ClientModels;
using System;
using PlayFab.MultiplayerModels;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.ACSPlayFab.Client
{
	public class ACSPlayFabClient : MonoBehaviour
	{
		public string playfabTitleId;
		public string playFabBuildId;

		public string customIdOverride;

		public float messageIntervalSec = 3;

		private string serverAddress;
		private int serverPort;

		private INetworkClient client;
		private float nextMessageSec;

		private LoginResult loginResult;

		public void Start()
		{
			PlayFabSettings.TitleId = playfabTitleId;

			var customId = string.IsNullOrEmpty( customIdOverride ) ?
				PlayerPrefs.GetString( "CustomPlayFabId", Guid.NewGuid().ToString() ) :
				customIdOverride;


			Debug.Log( $"Sending login for {customId}");

			PlayFabClientAPI.LoginWithCustomID( new LoginWithCustomIDRequest()
			{
				CreateAccount = true,
				CustomId = customId,
				TitleId = playfabTitleId
			},
			HandlePlayFabLoginResult,
			HandlePlayFabLoginError );
		}

		private void HandlePlayFabLoginError( PlayFabError error )
		{
			throw new InvalidOperationException( error.ToString() );
		}

		private void HandlePlayFabLoginResult( LoginResult loginResultIn )
		{
			Debug.Log( loginResultIn.ToString() );

			loginResult = loginResultIn;
			PlayFabMultiplayerAPI.RequestMultiplayerServer( new RequestMultiplayerServerRequest()
			{
				BuildId = playFabBuildId,
				SessionId = Guid.NewGuid().ToString(),
				PreferredRegions = new List<string>() { $"{AzureRegion.NorthEurope}" },
			},
			HandleRequestMultiplayerServerResponse,
			HandleRequestMultiplayerServerError
			);
		}

		private void HandleRequestMultiplayerServerError( PlayFabError error )
		{
			Debug.LogError( error.ToString() );
		}

		private void HandleRequestMultiplayerServerResponse( RequestMultiplayerServerResponse response )
		{
			serverAddress = response.ServerId;
			serverPort = 8080;
		}

		private void OnDestroy()
		{
			if ( client != null )
			{
				client.ResetConnection();
			}
		}

		private void Update()
		{
			if ( (loginResult != null) && (serverAddress != null) && (serverPort != 0) && (client == null) )
			{
				client = new NetworkClientUnityTransport();
				client.Connect( serverAddress, serverPort );
			}

			if ( client != null )
			{
				client.ProcessNetworkEvents();

				if ( client.IsConnected )
				{
					if ( nextMessageSec < Time.time )
					{
						string message = "ACS PlayFab Sample Message";
						client.Send( Encoding.UTF8.GetBytes( message ) );

						nextMessageSec = Time.time + messageIntervalSec;
					}
				}
			}
		}
	}
}