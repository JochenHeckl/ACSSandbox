using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class ClientRuntimeData : IClientRuntimeData
	{
		public float TimeSec { get; set; }

		public RectTransform UserInterfaceRoot { get; set; }
		public ViewModels ViewModels { get; } = new ViewModels();
		public PingData PingData { get; set; }
		public GlobalServerData GlobalServerData { get; set; }
		public bool IsAuthenticated { get; set; }
	}
}