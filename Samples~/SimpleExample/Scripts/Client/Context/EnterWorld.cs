using System;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class EnterWorld : IContext
	{
		private ContextUIView contextUI;

		private readonly IContextResolver contextResolver;
		private readonly ClientConfiguration configuration;
		private readonly IClientResources resources;
		private readonly IClientRuntimeData runtimeData;
		private readonly INetworkClient networkClient;
		private readonly IMessageSerializer messageSerializer;
		private readonly IMessageDispatcher messageDispatcher;

		public EnterWorld(
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
			runtimeData.GlobalServerData = null;

			if ( contextUI == null )
			{
				contextUI = UnityEngine.Object.Instantiate( resources.EnterWorldView, runtimeData.UserInterfaceRoot );
			}

			var viewModel = EnterWorldViewModel();
			runtimeData.ViewModels.EnterWorldViewModel = viewModel;

			runtimeData.LobbyCamera.gameObject.SetActive( true );
			runtimeData.WorldCamera.gameObject.SetActive( false );

			contextUI.DataSource = viewModel;
			contextUI.Show();

			messageDispatcher.RegisterHandler<ServerDataResponse>( HandleGlobalServerDataResponse );

			Send( new ServerDataRequest() );
		}

		public void LeaveContext( IContextContainer contextContainer )
		{
			contextUI.Hide();
			UnityEngine.Object.Destroy( contextUI.gameObject, contextUI.fadeInTimeSec );

			messageDispatcher.UnregisterHandler<ServerDataResponse>( HandleGlobalServerDataResponse );
		}

		public void Update( IContextContainer contextContainer, float deltaTimeSec )
		{
			if( runtimeData.GlobalServerData != null )
			{
				if ( runtimeData.ViewModels.EnterWorldViewModel.StatusText != resources.StringResources.LoadingWorldText )
				{
					runtimeData.ViewModels.EnterWorldViewModel.StatusText = resources.StringResources.LoadingWorldText;
					runtimeData.ViewModels.EnterWorldViewModel.NotifyViewModelChanged();
				}
			}
		}

		private EnterWorldViewModel EnterWorldViewModel()
		{
			return new EnterWorldViewModel()
			{
				StatusText = resources.StringResources.AcquireServerDataText
			};
		}

		private void HandleGlobalServerDataResponse( ServerDataResponse message )
		{
			runtimeData.GlobalServerData = new ServerData()
			{
				UptimeSec = message.UptimeSec,
				LoggedInUserCount = message.LoggedInUserCount,
				ServerWorldId = message.WorldId
			};
		}

		private void Send( object message )
		{
			networkClient.Send( messageSerializer.Serialize( message ) );
		}
	}
}
