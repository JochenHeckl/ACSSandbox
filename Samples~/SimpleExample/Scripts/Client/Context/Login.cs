using System;
using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Example;
using de.JochenHeckl.Unity.IoCLight;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class Login : IContext
	{
		private readonly IContainer iocContainer;

		private ClientConfiguration configuration;
		private IClientResources resources;
		private IClientRuntimeData runtimeData;
		private INetworkClient networkClient;
		private IMessageSerializer messageSerializer;
		private IMessageDispatcher messageDispatcher;
		private LoginView loginUserInterface;

		private float connectionTimeoutSec;
		private bool isConnecting;
		private bool loginMessageWasSent;

		public Login(
			IContainer iocContainerIn,
			ClientConfiguration configurationIn,
			IClientRuntimeData runtimeDataIn,
			IClientResources clientResourcesIn,
			INetworkClient networkClientIn,
			IMessageSerializer messageSerializerIn,
			IMessageDispatcher messageDispatcherIn )
		{
			iocContainer = iocContainerIn;
			configuration = configurationIn;
			resources = clientResourcesIn;
			runtimeData = runtimeDataIn;
			networkClient = networkClientIn;
			messageSerializer = messageSerializerIn;
			messageDispatcher = messageDispatcherIn;
		}

		public void EnterContext( IContextContainer contextContainer )
		{
			messageDispatcher.RegisterHandler<LoginResponse>( HandleLoginResponse );

			if ( loginUserInterface == null )
			{
				loginUserInterface = UnityEngine.Object.Instantiate( resources.LoginView, runtimeData.UserInterfaceRoot );
			}

			var loginViewModel = MakeLogionViewModel();
			runtimeData.ViewModels.LoginViewModel = loginViewModel;
			
			loginUserInterface.DataSource = loginViewModel;
			loginUserInterface.Show();
		}

		public void LeaveContext( IContextContainer contextContainer )
		{
			loginUserInterface.Hide();
			UnityEngine.Object.Destroy( loginUserInterface.gameObject, loginUserInterface.fadeInTimeSec );
		}

		public void Update( IContextContainer contextContainer, float deltaTimeSec )
		{
			if ( isConnecting && networkClient.IsConnected )
			{
				isConnecting = false;

				contextContainer.SwitchToContext( iocContainer.Resolve<AcquireGlobalServerData>() );
			}

			if ( isConnecting && Time.realtimeSinceStartup > connectionTimeoutSec )
			{
				// connection timeout

				isConnecting = false;

				runtimeData.ViewModels.LoginViewModel.EnableLogin = !isConnecting;
				runtimeData.ViewModels.LoginViewModel.NotifyViewModelChanged();

				networkClient.ResetConnection();

				Debug.LogError( "Timeout connecting to server." );
			}

			if( networkClient.IsConnected && !loginMessageWasSent )
			{
				Send( new LoginRequest()
				{
					// TODO: use reasonable data
					// just some hard coded login data to begin with...
					// this would be data received from some kind of service like playfab

					UserId = Environment.UserName,
					LoginToken = $"{Environment.UserName}@{Environment.MachineName}"
				} );

				loginMessageWasSent = true;
			}
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
			loginViewModel.ServerAddressText = configuration.WellKnownServers[selectedIndex].ServerAddress;
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

		private void HandleLoginResponse( LoginResponse message )
		{
			runtimeData.IsAuthenticated = message.LoginResult == LoginResult.OK;
		}

		private void Send( object message )
		{
			networkClient.Send( messageSerializer.Serialize( message ) );
		}
	}
}