using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using de.JochenHeckl.Unity.ACSSandbox.Common;

using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient
{
	public class UnitSync
	{
		public class UnitData
		{
			public long UnitId { get; set; }
			
			public SerializableVector3 Position { get; set; }
			public SerializableQuaternion Rotation { get; set; }
			public string ControllingUserId { get; set; }
			public UnitTypeId UnitTypeId { get; set; }
		}

		public UnitData[] Units { get; set; }
		public float ServerIntegrationTimeSec { get; set; }
	}
}
