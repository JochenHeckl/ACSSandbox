namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class ServerConfiguration
	{
		public int ServerPort { get; set; }
		public float IntegrationTimeStepSec { get; set; }
		public float TimeLapse { get; set; }
		public int MaxMessageSizeByte { get; set; }
	}
}