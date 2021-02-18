using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Example.Common;
using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ClientToServer;
using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal class InteractWithWorld : IState
	{
		private ContextUIView contextUI;

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

			runtimeData.ViewModels.EnterWorldViewModel.StatusText = resources.StringResources.LoadingWorldText;
			runtimeData.ViewModels.EnterWorldViewModel.NotifyViewModelChanged();

			var worldPrefab = resources.GetWorld( runtimeData.ServerData.WorldId );
			runtimeData.World = Object.Instantiate( worldPrefab, runtimeData.WorldRoot );
			runtimeData.World.gameObject.RecursiveMoveToLayer( runtimeData.WorldRoot.gameObject.layer );

			runtimeData.World.ActiveCamera = runtimeData.WorldCamera.ActiveCamera;

			runtimeData.WorldCamera.SetActive( true );
			runtimeData.LobbyCamera.gameObject.SetActive( false );

			// TODO: replace with more sophisticated approach.
			operations.Send( new SpawnRequest()
			{
				SpawnLocation = runtimeData.World.spawnLocations.First().transform.position
			} );
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
			if ( contextUI != null )
			{
				contextUI.Hide();
			}
		}

		public void UpdateState( IStateMachine contextContainer, float deltaTimeSec )
		{
		}

		private void ShowContextUI()
		{
			if ( contextUI == null )
			{
				contextUI = UnityEngine.Object.Instantiate( resources.EnterWorldView, runtimeData.UserInterfaceRoot );
			}

			runtimeData.ViewModels.EnterWorldViewModel = MakeEnterWorldViewModel();
			contextUI.DataSource = runtimeData.ViewModels.EnterWorldViewModel;
			contextUI.Show();
		}

		private EnterWorldViewModel MakeEnterWorldViewModel()
		{
			return new EnterWorldViewModel()
			{
				StatusText = "xxx"
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
