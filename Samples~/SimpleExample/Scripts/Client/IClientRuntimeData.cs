using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal interface IClientRuntimeData
	{
		float NextNetworkConnectionRetrySec { get; set; }

		RectTransform UserInterfaceRoot { get; set; }
		ViewModels ViewModels { get; }
	}
}