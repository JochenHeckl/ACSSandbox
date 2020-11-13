using System.Numerics;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
	public class SpawnArea : MonoBehaviour
	{
		public float spawnAreaRadius;

		private void OnDrawGizmos()
		{
			Gizmos.color = new Color( 0.1f, 0.1f, 0.1f, 0.5f );
			Gizmos.DrawSphere( transform.position, spawnAreaRadius );
		}
	}
}
