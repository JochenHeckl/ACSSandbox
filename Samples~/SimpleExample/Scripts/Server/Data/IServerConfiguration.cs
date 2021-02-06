using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	interface IServerConfiguration
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
