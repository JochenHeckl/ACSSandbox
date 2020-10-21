namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	public class AuthenticatedClient
	{
		public int ClientConnectionId { get; set; }
		public string ClientAccountId { get; set; }
		public string AuthToken { get; set; }
	}
}