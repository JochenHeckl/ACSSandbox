using System;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
	public class World : MonoBehaviour
	{
		public Guid WorldId => Guid.Parse( gameObject.name );

		public SpawnArea[] spawnLocations;

		public Camera ActiveCamera { get; set; }

		public Transform unitRoot;
		public Transform UnitRoot => unitRoot;
	}
}