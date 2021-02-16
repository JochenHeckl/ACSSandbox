using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	internal interface IServerConfiguration
	{
		public Guid ServerWorldId { get; }
		public int ServerPort { get; }
		public float IntegrationTimeStepSec { get; }
		public float IntegrationTimeLapse { get; }
		public float UnitDataSyncIntervalSec { get; }
		public int MaxMessageSizeByte { get; }
		public float MaxUnitSpeed { get; }
	}
}
