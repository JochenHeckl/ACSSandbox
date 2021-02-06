using de.JochenHeckl.Unity.ACSSandbox.Common;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public class ClientUnitView : UnitView
	{
		public float maxLerpDelaySec = 0.1f;
		public ClientUnitData UnitData { get; set; }

		public void Update()
		{
			if ( UnitData != null )
			{
				var lerpFactor = Mathf.Min( 1.0f, (Time.deltaTime / maxLerpDelaySec) );
				
				transform.position = Vector3.Lerp( transform.position, UnitData.Position, lerpFactor );
				transform.rotation = UnitData.Rotation;
			}
		}
	}
}