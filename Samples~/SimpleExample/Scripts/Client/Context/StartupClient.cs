using System;

using UnityEngine;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class StartupClient : IContext
	{
		private IContextResolver contextResolver;
		private IClientRuntimeData runtimeData;
		private IClientResources resources;

		public StartupClient( 
			IContextResolver contextResolverIn,
			IClientRuntimeData runtimeDataIn,
			IClientResources resourcesIn )
		{
			contextResolver = contextResolverIn;
			runtimeData = runtimeDataIn;
			resources = resourcesIn;
		}

		public void EnterContext( IContextContainer contextContainer )
		{
			// more or less a placeholder for now.
			// we probably have to check for availability of services, update the client etc

			runtimeData.LobbyCamera = GameObject.Instantiate<Camera>( resources.LobbyCamera, runtimeData.WorldRoot );
			runtimeData.WorldCamera = GameObject.Instantiate<Camera>( resources.WorldCamera, runtimeData.WorldRoot );

			contextContainer.PushContext( contextResolver.Resolve<ConnectToServer>() );
		}

		public void LeaveContext( IContextContainer contextContainer )
		{
			
		}

		public void Update( IContextContainer contextContainer, float deltaTimeSec )
		{
		}
	}
}
