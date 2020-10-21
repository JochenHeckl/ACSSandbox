using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class ClientRuntimeData : IClientRuntimeData
	{
		public RectTransform UserInterfaceRoot { get; set; }
		public float NextNetworkConnectionRetrySec { get; set; }
		public ViewModels ViewModels { get; } = new ViewModels();
	}
}