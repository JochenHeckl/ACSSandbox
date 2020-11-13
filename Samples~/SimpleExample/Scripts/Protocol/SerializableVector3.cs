
using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Protocol
{
	public class SerializableVector3
	{
		public static implicit operator Vector3( SerializableVector3 vector ) => new Vector3(vector.x, vector.y, vector.z );
		
		public static implicit operator SerializableVector3( Vector3 vector ) => new SerializableVector3()
		{
			x = vector.x,
			y = vector.y,
			z = vector.z,
		};

		public float x, y, z;
	}
}
