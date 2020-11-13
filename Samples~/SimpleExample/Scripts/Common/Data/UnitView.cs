using System;
using System.ComponentModel;

using de.JochenHeckl.Unity.DataBinding;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
	public class UnitView : ViewBehaviour
	{
		[Space(10)]
		[Header("Unit Data")]

		public long unitId;
		public long UnitId => unitId;

		public UnitTypeId unitTypeId;
		public UnitTypeId UnitTypeId => unitTypeId;

		public void Remove()
		{
			Destroy( gameObject );
		}
	}
}
