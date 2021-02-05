using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal interface IWorldCamera
	{
		Camera ActiveCamera { get; }
		void TrackGameObjects( Transform[] targets );

		bool IsActive { get; }
		void SetActive( bool active );
	}
}