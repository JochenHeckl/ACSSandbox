using PlayFab;
using System.Linq;
using PlayFab.MultiplayerAgent.Model;
using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.ACSPlayFab.Server
{
	public class PlayFabServerIntegration
	{
		private Action handleServerActive;
		private Action handleShutDown;
		private Action<DateTime?> handleMaintenance;
		private Action<string> handleAgentError;

		public void SetupPlayFabEnvironment(
			string playFabTitleId,
			string playFabDeveloperSecret,
			Action handleServerActiveIn,
			Action handleShutDownIn,
			Action<System.DateTime?> handleMaintenanceIn,
			Action<string> handleAgentErrorIn )
		{
			handleServerActive = handleServerActiveIn;
			handleShutDown = handleShutDownIn;
			handleMaintenance = handleMaintenanceIn;
			handleAgentError = handleAgentErrorIn;

			PlayFabSettings.TitleId = playFabTitleId;
			PlayFabSettings.DeveloperSecretKey = playFabDeveloperSecret;

			PlayFabMultiplayerAgentAPI.Start();

			PlayFabMultiplayerAgentAPI.IsDebugging = false;

			PlayFabMultiplayerAgentAPI.OnServerActiveCallback += handleServerActive.Invoke;
			PlayFabMultiplayerAgentAPI.OnShutDownCallback += handleShutDown.Invoke;
			PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += handleMaintenance.Invoke;
			PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += handleAgentError.Invoke;

			PlayFabMultiplayerAgentAPI.ReadyForPlayers();
		}

		public void ResetPlayFabEnvironment()
		{
			PlayFabMultiplayerAgentAPI.OnAgentErrorCallback -= handleAgentError.Invoke;
			PlayFabMultiplayerAgentAPI.OnMaintenanceCallback -= handleMaintenance.Invoke;
			PlayFabMultiplayerAgentAPI.OnShutDownCallback -= handleShutDown.Invoke;
			PlayFabMultiplayerAgentAPI.OnServerActiveCallback -= handleServerActive.Invoke;
		}

		public void ReportClientConnections( string[] clientIds )
		{
			var connectPlayers = clientIds.Select( x => new ConnectedPlayer( x ) );
			PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers( connectPlayers.ToList() );
		}
	}
}