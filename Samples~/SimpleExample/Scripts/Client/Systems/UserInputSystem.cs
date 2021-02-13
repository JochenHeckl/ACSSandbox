using UnityEngine;

using de.JochenHeckl.Unity.ACSSandbox.Protocol.ClientToServer;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal class UserInputSystem : IClientSystem
	{
		private ClientConfiguration configuration;
		private IClientRuntimeData runtimeData;
		private IClientOperations operations;
		
		private KeyCode navigationCommandKey;

		public UserInputSystem(
			ClientConfiguration configurationIn,
			IClientRuntimeData runtimeDataIn,
			IClientOperations operationsIn )
		{
			configuration = configurationIn;
			runtimeData = runtimeDataIn;
			operations = operationsIn;
		}

		public void Initialize()
		{
			navigationCommandKey = KeyCode.Mouse1;
		}

		public void Shutdown()
		{
			
		}

		public void Update( float deltaTimeSec )
		{
			if( Input.GetKeyDown( navigationCommandKey ) )
			{
				if( runtimeData.WorldCamera.IsActive )
				{
					var pickingRay = runtimeData.WorldCamera.ActiveCamera.ScreenPointToRay( Input.mousePosition );

					var layerMask = 1 << runtimeData.World.WorldLayer;

					if ( Physics.Raycast( pickingRay, out var rayCastHit, layerMask ) )
					{
						operations.Send( new NavigateToPositionRequest()
						{
							Destination = rayCastHit.point
						} );
					}
				}
			}
		}
	}
}