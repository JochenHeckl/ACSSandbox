
using de.JochenHeckl.Unity.ACSSandbox.Example.Common;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	public class ServerUnitData : IServerUnitData
	{
		public long UnitId { get; set; }
		public UnitTypeId UnityTypeId { get; set; }

		public string ControllingUserId { get; set; }
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }

		public Vector3 Destination { get; set; }
		public float MaxSpeedMetersPerSec { get; set; }
	}
}