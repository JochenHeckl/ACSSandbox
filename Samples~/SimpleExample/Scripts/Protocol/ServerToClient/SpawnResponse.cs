namespace de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient
{
	public enum SpawnRequestResult
	{
		Success = 0,
		InternalError = 1,
	}

	public class SpawnResponse
	{
		public SpawnRequestResult Result { get; set; }
	}
}
