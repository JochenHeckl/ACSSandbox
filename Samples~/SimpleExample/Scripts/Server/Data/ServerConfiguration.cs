using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class ServerConfiguration
	{
		public Guid ServerWorldId { get; set; }

		public int ServerPort { get; set; }
		public float IntegrationTimeStepSec { get; set; }
		public float UnitDataSyncIntervalSec { get; internal set; }
		public float DefaultTimeLapse { get; set; }
		public int MaxMessageSizeByte { get; set; }
		public float DefaultMaxUnitSpeed { get; internal set; }
	}
}