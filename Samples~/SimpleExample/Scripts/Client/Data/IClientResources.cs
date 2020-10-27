using System;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal interface IClientResources
	{
		ContextUIView ConnectToServerView { get; }
		ContextUIView EnterWorldView { get; }

		Camera LobbyCamera { get; }
		Camera WorldCamera { get; }

		StringResources StringResources { get; }

		World GetWorld( Guid worldId );
	}
}
