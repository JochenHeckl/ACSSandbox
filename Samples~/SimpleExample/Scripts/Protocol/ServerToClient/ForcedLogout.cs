namespace de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ServerToClient
{
	public enum ForcedLogoutReason
	{
		None = 0,
		ConnectionTakeOver = 1,
		ServerShutDown = 2,
		AndministrativeAction = 3,
	};

	public class ForcedLogout
	{
		public ForcedLogoutReason Reason { get; set; }
	}
}
