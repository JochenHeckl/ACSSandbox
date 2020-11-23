using System.Collections;
using System.Collections.Generic;

using de.JochenHeckl.Unity.DataBinding;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	public class ServerUnitViewModel : ViewModelBase
	{
		public IServerUnitData UnitData { get; set; }
}
}