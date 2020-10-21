using System;
using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Common;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class LoginContext : IContext
	{
		private ClientConfiguration configuration;
		private IClientResources resources;
		private IClientRuntimeData runtimeData;
		private INetworkClient networkClient;

		private LoginView loginUserInterface;

		private float connectionTimeoutSec;
		private bool isConnecting;

		public LoginContext(
			ClientConfiguration configurationIn,
			IClientRuntimeData runtimeDataIn,
			IClientResources clientResourcesIn,
			INetworkClient networkClientIn )
		{
			configuration = configurationIn;
			resources = clientResourcesIn;
			runtimeData = runtimeDataIn;
			networkClient = networkClientIn;
		}

		public void EnterContext( IContext fromContext )
		{
			if ( loginUserInterface == null )
			{
				loginUserInterface = UnityEngine.Object.Instantiate( resources.LoginView, runtimeData.UserInterfaceRoot );
			}

			var loginViewModel = MakeLogionViewModel();
			runtimeData.ViewModels.LoginViewModel = loginViewModel;
			
			loginUserInterface.DataSource = loginViewModel;
			loginUserInterface.Show();
		}

		private LoginViewModel MakeLogionViewModel()
		{
			var preselectedHost = configuration.WellKnownServers.FirstOrDefault();

			var loginViewModel = new LoginViewModel()
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
				LoginAction = HandleLoginRequest
			};

			loginViewModel.WellKnownHostValueChanged
				.AddListener( new UnityAction<int>( ( x ) => HandleWellKnownHostValueChanged( loginViewModel, x ) ) );

			return loginViewModel;
		}

		private void HandleWellKnownHostValueChanged( LoginViewModel loginViewModel, int selectedIndex )
		{
			loginViewModel.SelectedOptionIndex = selectedIndex;
			loginViewModel.ServerAddressText  = configuration.WellKnownServers[selectedIndex].ServerAddress;
			loginViewModel.ServerPortText = $"{configuration.WellKnownServers[selectedIndex].ServerPort}";

			loginViewModel.NotifyViewModelChanged();
		}

		private void HandleLoginRequest( string serverAddress, int serverPort )
		{
			runtimeData.ViewModels.LoginViewModel.EnableLogin = false;
			runtimeData.ViewModels.LoginViewModel.NotifyViewModelChanged();

			isConnecting = true;
			connectionTimeoutSec = Time.realtimeSinceStartup + configuration.NetworkConnectionTimeoutSec;

			networkClient.Connect( serverAddress, serverPort );
		}

		public void LeaveContext( IContext toContext )
		{
			loginUserInterface.Hide();
			UnityEngine.Object.Destroy( loginUserInterface.gameObject, loginUserInterface.fadeInTimeSec );
		}

		public void Update( float deltaTimeSec )
		{
			if( networkClient.IsConnected )
			{
				isConnecting = false;
			}

			if( isConnecting && Time.realtimeSinceStartup > connectionTimeoutSec )
			{
				isConnecting = false;

				networkClient.ResetConnection();

				runtimeData.ViewModels.LoginViewModel.EnableLogin = true;
				runtimeData.ViewModels.LoginViewModel.NotifyViewModelChanged();

				// connection timeout
				Debug.LogError( "Timeout connecting to server." );
			}
		}
	}
}