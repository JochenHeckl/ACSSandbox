using System;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal class StartupClient : IContext
	{
		private readonly IClientRuntimeData runtimeData;
		private readonly IClientResources resources;

		public StartupClient( IClientRuntimeData runtimeDataIn, IClientResources resourcesIn )
		{
			runtimeData = runtimeDataIn;
			resources = resourcesIn;
		}

		public void EnterContext( IContextContainer contextContainer )
		{
			runtimeData.LobbyCamera = UnityEngine.Object.Instantiate( resources.LobbyCamera, runtimeData.WorldRoot );
			runtimeData.WorldCamera = UnityEngine.Object.Instantiate( resources.WorldCamera, runtimeData.WorldRoot );

			contextContainer.PushContext( contextContainer.Resolve<ConnectToServer>() );
		}

		public void LeaveContext( IContextContainer contextContainer )
		{
		}

		public void ActivateContext( IContextContainer contextContainer )
		{
		}

		public void DeactivateContext( IContextContainer contextContainer )
		{
		}

		public void Update( IContextContainer contextContainer, float deltaTimeSec )
		{
		}
	}
}
