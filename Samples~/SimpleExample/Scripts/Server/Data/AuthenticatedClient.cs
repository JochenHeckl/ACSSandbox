namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	public class AuthenticatedClient
	{
		public string UserId { get; set; }
		public string LoginToken { get; set; }
		public int ClientConnectionId { get; set; }
	}
}