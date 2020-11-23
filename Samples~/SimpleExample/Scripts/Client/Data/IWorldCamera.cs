using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal interface IWorldCamera
	{
		Camera ActiveCamera { get; }
		void TrackGameObjects( Transform[] targets );
		void SetActive( bool active );
	}
}