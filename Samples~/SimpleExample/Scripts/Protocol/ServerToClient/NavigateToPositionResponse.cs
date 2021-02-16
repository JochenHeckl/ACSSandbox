using de.JochenHeckl.Unity.ACSSandbox.Example.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ServerToClient
{
	public class NavigateToPositionResponse
	{
		public SerializableVector3 Position { get; set; }
		public NavigateToPositionResult Result { get; set; }
	}
}
