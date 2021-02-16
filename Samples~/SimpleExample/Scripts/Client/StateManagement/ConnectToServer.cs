using System;
using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal partial class ConnectToServer : IState
	{
		private float connectionTimeoutSec;
		private bool isConnecting;
		private bool loginMessageWasSent;

		private ContextUIView contextUI;

		private IClientOperations operations;
		private ClientConfiguration configuration;
		private IClientResources resources;
		private IClientRuntimeData runtimeData;
		private INetworkClient networkClient;

		public ConnectToServer( 
			IClientOperations operationsIn,
			ClientConfiguration configurationIn,
			IClientResources resourcesIn,
			IClientRuntimeData runtimeDataIn,
			INetworkClient networkClientIn )
		{
			operations = operationsIn;
			configuration = configurationIn;
			resources = resourcesIn;
			runtimeData = runtimeDataIn;
			networkClient = networkClientIn;
		}

		public void EnterState( IStateMachine contextContainer )
		{
			runtimeData.ServerData = null;
		}

		public void LeaveState( IStateMachine contextContainer )
		{
		}

		public void ActivateState( IStateMachine contextContainer )
		{
			ShowContextUI();
		}

		public void DeactivateState( IStateMachine contextContainer )
		{
			HideContextUI();
		}

		public void UpdateState( IStateMachine contextContainer, float deltaTimeSec )
		{
			if ( TestConnectionEstablished() )
			{
				isConnecting = false;
			}

			if ( TestConnectionTimeout() )
			{
				HandleConnectionTimeout();
			}

			if ( TestLoginRequestRequired() )
			{
				operations.Send( new LoginRequest()
				{
					// TODO: use reasonable data
					// just some hard coded login data to begin with...
					// this would be data received from some kind of service like PlayFab

					UserId = Environment.UserName,
					LoginToken = $"{Environment.UserName}@{Environment.MachineName}"
				} );

				loginMessageWasSent = true;
			}

			if ( runtimeData.IsAuthenticated && (runtimeData.ServerData != null) )
			{
				contextContainer.PushState( contextContainer.StateResolver.Resolve<InteractWithWorld>() );
			}
		}

		private bool TestLoginRequestRequired()
		{
			return networkClient.IsConnected && !loginMessageWasSent;
		}

		private bool TestConnectionEstablished()
		{
			return isConnecting && networkClient.IsConnected;
		}

		private bool TestConnectionTimeout()
		{
			return isConnecting && Time.realtimeSinceStartup > connectionTimeoutSec;
		}

		private void HandleConnectionTimeout()
		{
			isConnecting = false;

			runtimeData.ViewModels.ConnectToServerViewModel.EnableLogin = !isConnecting;
			runtimeData.ViewModels.ConnectToServerViewModel.NotifyViewModelChanged();

			networkClient.ResetConnection();

			Debug.LogError( "Timeout connecting to server." );
		}

		private void ShowContextUI()
		{
			if ( contextUI == null )
			{
				contextUI = UnityEngine.Object.Instantiate( resources.ConnectToServerView, runtimeData.UserInterfaceRoot );
			}

			var loginViewModel = MakeLogionViewModel();
			runtimeData.ViewModels.ConnectToServerViewModel = loginViewModel;

			runtimeData.LobbyCamera.gameObject.SetActive( true );
			runtimeData.WorldCamera.SetActive( false );

			contextUI.DataSource = loginViewModel;
			contextUI.Show();
		}

		private void HideContextUI()
		{
			contextUI.Hide();
		}

		private ConnectToServerViewModel MakeLogionViewModel()
		{
			var preselectedHost = configuration.WellKnownServers.FirstOrDefault();

			var loginViewModel = new ConnectToServerViewModel()
			{
				// well known host
				WellKnownHostsLabel = resources.StringResources.WellKnownHostsLabel,
				SelectedOptionIndex = 0,

				OptionData = configuration.WellKnownServers
					.Select( x => new TMPro.TMP_Dropdown.OptionData()
					{
						text = x.DisplayName
					} )
					.ToList(),

				WellKnownHostValueChanged = new TMP_Dropdown.DropdownEvent(),

				// server address
				ServerAddressLabel = resources.StringResources.ServerAddressLabel,
				ServerAddressText = preselectedHost?.ServerAddress,

				// server Port
				ServerPortLabel = resources.StringResources.ServerPortLabel,
				ServerPortText = $"{preselectedHost.ServerPort}",

				// login button
				LoginButtonLabel = resources.StringResources.LoginLabel,
				EnableLogin = true,
				LoginAction = HandleLoginAction
			};

			loginViewModel.WellKnownHostValueChanged
				.AddListener( new UnityAction<int>( ( x ) => HandleWellKnownHostValueChanged( loginViewModel, x ) ) );

			return loginViewModel;
		}

		private void HandleWellKnownHostValueChanged( ConnectToServerViewModel loginViewModel, int selectedIndex )
		{
			loginViewModel.SelectedOptionIndex = selectedIndex;
			loginViewModel.ServerAddressText = configuration.WellKnownServers[selectedIndex].ServerAddress;
			loginViewModel.ServerPortText = $"{configuration.WellKnownServers[selectedIndex].ServerPort}";

			loginViewModel.NotifyViewModelChanged();
		}


		private void HandleLoginAction( string serverAddress, int serverPort )
		{
			runtimeData.ViewModels.ConnectToServerViewModel.EnableLogin = false;
			runtimeData.ViewModels.ConnectToServerViewModel.NotifyViewModelChanged();

			isConnecting = true;
			connectionTimeoutSec = Time.realtimeSinceStartup + configuration.NetworkConnectionTimeoutSec;

			networkClient.Connect( serverAddress, serverPort );
		}
	}
}