using UnityEngine;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	public class ClientUnitData : IUnitData
	{
		public long UnitId { get; set; }

		public UnitTypeId UnityTypeId { get; set; }
		public string ControllingUserId { get; set; }
		
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }
	}
}