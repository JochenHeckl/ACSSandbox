using System;

using de.JochenHeckl.Unity.DataBinding;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public class World : ViewBehaviour
	{
		public Guid WorldId => Guid.Parse( gameObject.name );
	}
}