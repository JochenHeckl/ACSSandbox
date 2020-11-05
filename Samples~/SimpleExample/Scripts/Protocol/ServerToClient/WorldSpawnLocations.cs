using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace de.JochenHeckl.Unity.ACSSandbox.Protocol.ServerToClient
{
	public class WorldSpawnLocations
	{
		public class SpawnArea
		{
			public Vector3 Position { get; set; }
			public float AreaRadius { get; set; }
		}

		public SpawnArea[] SpawnAreas { get; set; }
	}
}
