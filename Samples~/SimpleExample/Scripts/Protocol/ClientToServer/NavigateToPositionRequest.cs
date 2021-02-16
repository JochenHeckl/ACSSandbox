using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ClientToServer
{
	public class NavigateToPositionRequest
	{
		public SerializableVector3 Destination { get; set; }
	}
}
