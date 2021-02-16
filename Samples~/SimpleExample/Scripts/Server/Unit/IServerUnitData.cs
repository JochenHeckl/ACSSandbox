using de.JochenHeckl.Unity.ACSSandbox.Example.Common;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	public interface IServerUnitData : IUnitData
	{
		Vector3 Destination { get; set; }
		float MaxSpeedMetersPerSec { get; set;  }
	}
}