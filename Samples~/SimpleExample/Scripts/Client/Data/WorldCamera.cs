using UnityEngine;
using Cinemachine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public class WorldCamera : MonoBehaviour, IWorldCamera
	{
		public Camera activeCamera;
		public CinemachineVirtualCamera virtualCamera;

		public Camera ActiveCamera => activeCamera;
		
		public void SetActive( bool active )
		{
			gameObject.SetActive( active );
		}

		public void TrackGameObjects( Transform[] targets )
		{
			if ( targets.Length == 0 )
			{
				virtualCamera.LookAt = null;
				virtualCamera.Follow = null;
			}

			if ( targets.Length == 1 && (virtualCamera.Follow != targets[0]) )
			{
				virtualCamera.LookAt = targets[0];
				virtualCamera.Follow = targets[0];
			}
		}
	}
}