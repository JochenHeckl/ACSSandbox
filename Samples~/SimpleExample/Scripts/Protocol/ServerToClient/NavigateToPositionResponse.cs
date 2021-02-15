using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient
{
	public class NavigateToPositionResponse
	{
		public SerializableVector3 Position { get; set; }
		public NavigateToPositionResult Result { get; set; }
	}
}
