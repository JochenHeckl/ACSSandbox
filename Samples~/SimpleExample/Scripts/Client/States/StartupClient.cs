using System;

using de.JochenHeckl.Unity.ACSSandbox.Example.Common;
using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal class StartupClient : IState
	{
		private readonly IClientRuntimeData runtimeData;
		private readonly IClientResources resources;

		public StartupClient( IClientRuntimeData runtimeDataIn, IClientResources resourcesIn )
		{
			runtimeData = runtimeDataIn;
			resources = resourcesIn;
		}

		public void EnterState( IStateMachine contextContainer )
		{
			runtimeData.LobbyCamera = UnityEngine.Object.Instantiate( resources.LobbyCamera, runtimeData.WorldRoot );
			runtimeData.WorldCamera = UnityEngine.Object.Instantiate( resources.WorldCamera, runtimeData.WorldRoot );

			contextContainer.PushState( contextContainer.StateResolver.Resolve<ConnectToServer>() );
		}

		public void LeaveState( IStateMachine contextContainer )
		{
		}

		public void ActivateState( IStateMachine contextContainer )
		{
		}

		public void DeactivateState( IStateMachine contextContainer )
		{
		}

		public void UpdateState( IStateMachine contextContainer, float deltaTimeSec )
		{
		}
	}
}
