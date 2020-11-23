using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class InteractWithWorld : IContext
	{
		private ContextUIView contextUI;

		private bool worldDataLoaded;

		private readonly IClientOperations operations;
		private readonly ClientConfiguration configuration;
		private readonly IClientResources resources;
		private readonly IClientRuntimeData runtimeData;
		
		public InteractWithWorld(
			IClientOperations operaitonsIn,
			ClientConfiguration configurationIn,
			IClientRuntimeData runtimeDataIn,
			IClientResources clientResourcesIn )
		{
			operations = operaitonsIn;
			configuration = configurationIn;
			resources = clientResourcesIn;
			runtimeData = runtimeDataIn;
		}

		public void EnterContext( IContextContainer contextContainer )
		{
			operations.RegisterHandler<SpawnResponse>( HandleSpawnResponse );
			
			ShowContextUI();
		}

		public void LeaveContext( IContextContainer contextContainer )
		{
			operations.RegisterHandler<SpawnResponse>( HandleSpawnResponse );
		}

		public void ActivateContext( IContextContainer contextContainer )
		{
			ShowContextUI();
		}

		public void DeactivateContext( IContextContainer contextContainer )
		{
			contextUI.Hide();
		}

		public void Update( IContextContainer contextContainer, float deltaTimeSec )
		{
			if ( !worldDataLoaded )
			{
				runtimeData.ViewModels.EnterWorldViewModel.StatusText = resources.StringResources.LoadingWorldText;
				runtimeData.ViewModels.EnterWorldViewModel.NotifyViewModelChanged();

				var worldPrefab = resources.GetWorld( runtimeData.ServerData.WorldId );
				runtimeData.World = UnityEngine.Object.Instantiate<World>( worldPrefab, runtimeData.WorldRoot );
				runtimeData.World.gameObject.layer = runtimeData.WorldRoot.gameObject.layer;

				runtimeData.World.ActiveCamera = runtimeData.WorldCamera.ActiveCamera;

				runtimeData.WorldCamera.SetActive( true );
				runtimeData.LobbyCamera.gameObject.SetActive( false );

				worldDataLoaded = true;

				operations.Send( new SpawnRequest()
				{
					SpawnLocation = runtimeData.World.spawnLocations.First().transform.position
				} );

				contextUI.Hide();
			}
		}

		private void ShowContextUI()
		{
			if ( contextUI == null )
			{
				contextUI = UnityEngine.Object.Instantiate( resources.EnterWorldView, runtimeData.UserInterfaceRoot );
			}

			var viewModel = MakeEnterWorldViewModel();
			runtimeData.ViewModels.EnterWorldViewModel = viewModel;

			runtimeData.LobbyCamera.gameObject.SetActive( true );
			runtimeData.WorldCamera.SetActive( false );

			contextUI.DataSource = viewModel;
			contextUI.Show();
		}

		private EnterWorldViewModel MakeEnterWorldViewModel()
		{
			return new EnterWorldViewModel()
			{
				StatusText = resources.StringResources.AcquireServerDataText
			};
		}

		private void HandleSpawnResponse( SpawnResponse message )
		{
			runtimeData.ControlledUnitId = message.ControlledUnitId;

			// spawnRequestSuccess = message.Result == SpawnRequestResult.Success;
		}
	}
}
