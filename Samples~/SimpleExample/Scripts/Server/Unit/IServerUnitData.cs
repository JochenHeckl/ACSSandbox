using de.JochenHeckl.Unity.ACSSandbox.Common;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	public interface IServerUnitData : IUnitData
	{
		Vector3 Destination { get; set; }
		float MaxSpeedMetersPerSec { get; set;  }
	}
}