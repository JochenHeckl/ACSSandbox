using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal interface IClientRuntimeData
	{
		public float TimeSec { get;}
		RectTransform UserInterfaceRoot { get; }
		ViewModels ViewModels { get; }
		bool IsAuthenticated { get; set; }
		PingData PingData { get; set; }
		GlobalServerData GlobalServerData { get; set; }
	}
}