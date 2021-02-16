using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	internal class ServerConfiguration : IServerConfiguration
	{
		public Guid ServerWorldId { get; set; }

		public int ServerPort { get; set; }
		public float IntegrationTimeStepSec { get; set; }
		public float IntegrationTimeLapse { get; set; }
		public float UnitDataSyncIntervalSec { get; set; }
		public int MaxMessageSizeByte { get; set; }
		public float MaxUnitSpeed { get; set; }
	}
}