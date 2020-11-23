namespace de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient
{
	public enum SpawnRequestResult
	{
		Success = 0,
		InternalError = 1,
		AuthenticationFailure = 2,
	}

	public class SpawnResponse
	{
		public SpawnRequestResult Result { get; set; }
		public long ControlledUnitId { get; set; }
	}
}
