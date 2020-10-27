using System;
using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol;
using de.JochenHeckl.Unity.IoCLight;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class ConnectToServer : IContext
	{
		private float connectionTimeoutSec;
		private bool isConnecting;
		private bool loginMessageWasSent;

		private ContextUIView connectToServerView;

		private readonly IContextResolver contextResolver;
		private readonly ClientConfiguration configuration;
		private readonly IClientResources resources;
		private readonly IClientRuntimeData runtimeData;
		private readonly INetworkClient networkClient;
		private readonly IMessageSerializer messageSerializer;
		private readonly IMessageDispatcher messageDispatcher;

		public ConnectToServer(
			IContextResolver contextResolverIn,
			ClientConfiguration configurationIn,
			IClientRuntimeData runtimeDataIn,
			IClientResources clientResourcesIn,
			INetworkClient networkClientIn,
			IMessageSerializer messageSerializerIn,
			IMessageDispatcher messageDispatcherIn )
		{
			contextResolver = contextResolverIn;
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

			if ( connectToServerView == null )
			{
				connectToServerView = UnityEngine.Object.Instantiate( resources.ConnectToServerView, runtimeData.UserInterfaceRoot );
			}

			var loginViewModel = MakeLogionViewModel();
			runtimeData.ViewModels.ConnectToServerViewModel = loginViewModel;

			runtimeData.LobbyCamera.gameObject.SetActive( true );
			runtimeData.WorldCamera.gameObject.SetActive( false );

			connectToServerView.DataSource = loginViewModel;
			connectToServerView.Show();
		}

		public void LeaveContext( IContextContainer contextContainer )
		{
			connectToServerView.Hide();
			UnityEngine.Object.Destroy( connectToServerView.gameObject, connectToServerView.fadeInTimeSec );

			messageDispatcher.UnregisterHandler<LoginResponse>( HandleLoginResponse );
		}

		public void Update( IContextContainer contextContainer, float deltaTimeSec )
		{
			if ( isConnecting && networkClient.IsConnected )
			{
				isConnecting = false;
			}

			if ( isConnecting && Time.realtimeSinceStartup > connectionTimeoutSec )
			{
				// connection timeout

				isConnecting = false;

				runtimeData.ViewModels.ConnectToServerViewModel.EnableLogin = !isConnecting;
				runtimeData.ViewModels.ConnectToServerViewModel.NotifyViewModelChanged();

				networkClient.ResetConnection();

				Debug.LogError( "Timeout connecting to server." );
			}

			if( networkClient.IsConnected && !loginMessageWasSent )
			{
				Send( new LoginRequest()
				{
					// TODO: use reasonable data
					// just some hard coded login data to begin with...
					// this would be data received from some kind of service like PlayFab

					UserId = Environment.UserName,
					LoginToken = $"{Environment.UserName}@{Environment.MachineName}"
				} );

				loginMessageWasSent = true;
			}

			if( runtimeData.IsAuthenticated )
			{
				contextContainer.SwitchToContext( contextResolver.Resolve<EnterWorld>() );
			}
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
				LoginAction = HandleLoginRequest
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

		private void HandleLoginRequest( string serverAddress, int serverPort )
		{
			runtimeData.ViewModels.ConnectToServerViewModel.EnableLogin = false;
			runtimeData.ViewModels.ConnectToServerViewModel.NotifyViewModelChanged();

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