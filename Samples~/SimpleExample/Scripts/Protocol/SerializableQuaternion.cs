
using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Protocol
{
	public class SerializableQuaternion
	{
		public static implicit operator Quaternion( SerializableQuaternion quaternion )
			=> new Quaternion( quaternion.x, quaternion.y, quaternion.z, quaternion.w );

		public static implicit operator SerializableQuaternion( Quaternion quaternion ) => new SerializableQuaternion()
		{
			x = quaternion.x,
			y = quaternion.y,
			z = quaternion.z,
			w = quaternion.w,
		};

		public float x, y, z, w;
	}
}
