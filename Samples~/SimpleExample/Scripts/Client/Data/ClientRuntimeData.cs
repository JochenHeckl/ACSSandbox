using UnityEngine;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class ClientRuntimeData : IClientRuntimeData
	{
		public float TimeSec { get; set; }

		public Camera LobbyCamera { get; set; }
		public Camera WorldCamera { get; set; }

		public Transform WorldRoot { get; set; }
		public RectTransform UserInterfaceRoot { get; set; }

		public ViewModels ViewModels { get; } = new ViewModels();
		public PingData PingData { get; set; }
		public ServerData ServerData { get; set; }
		public bool IsAuthenticated { get; set; }
		public World World { get; set; }
		public IDictionary<long, (ClientUnitData unitData, ClientUnitView unitView)> Units { get; set; } =
			new Dictionary<long, (ClientUnitData unitData, ClientUnitView unitView)>();
	}
}