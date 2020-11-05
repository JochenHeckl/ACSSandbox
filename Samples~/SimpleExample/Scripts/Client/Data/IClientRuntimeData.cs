using UnityEngine;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal interface IClientRuntimeData
	{
		public float TimeSec { get; }

		public Camera LobbyCamera { get; set; }
		public Camera WorldCamera { get; set; }

		Transform WorldRoot { get; }
		RectTransform UserInterfaceRoot { get; }
		ViewModels ViewModels { get; }
		bool IsAuthenticated { get; set; }
		PingData PingData { get; set; }
		ServerData GlobalServerData { get; set; }
		World World { get; set; }
	}
}