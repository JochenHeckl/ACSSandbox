using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal class InteractWithWorld : IState
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

		public void EnterState( IStateMachine contextContainer )
		{
			operations.RegisterHandler<SpawnResponse>( HandleSpawnResponse );
			operations.RegisterHandler<NavigateToPositionResponse>( HandleNavigateToPositionResponse );

			ShowContextUI();
		}

		public void LeaveState( IStateMachine contextContainer )
		{
			operations.DeregisterHandler<NavigateToPositionResponse>( HandleNavigateToPositionResponse );
			operations.DeregisterHandler<SpawnResponse>( HandleSpawnResponse );
		}

		public void ActivateState( IStateMachine contextContainer )
		{
			ShowContextUI();
		}

		public void DeactivateState( IStateMachine contextContainer )
		{
			contextUI.Hide();
		}

		public void UpdateState( IStateMachine contextContainer, float deltaTimeSec )
		{
			if ( !worldDataLoaded )
			{
				runtimeData.ViewModels.EnterWorldViewModel.StatusText = resources.StringResources.LoadingWorldText;
				runtimeData.ViewModels.EnterWorldViewModel.NotifyViewModelChanged();

				var worldPrefab = resources.GetWorld( runtimeData.ServerData.WorldId );
				runtimeData.World = Object.Instantiate( worldPrefab, runtimeData.WorldRoot );
				runtimeData.World.gameObject.RecursiveMoveToLayer( runtimeData.WorldRoot.gameObject.layer );

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
			if ( message.Result == SpawnRequestResult.Success )
			{
				runtimeData.ControlledUnitId = message.ControlledUnitId;
			}
			else
			{
				runtimeData.ControlledUnitId = Constants.InvalidUnitId;
			}
		}

		private void HandleNavigateToPositionResponse( NavigateToPositionResponse message )
		{
			if( message.Result != NavigateToPositionResult.Success )
			{
				// TODO Update ping visualization
			}
		}
	}
}
