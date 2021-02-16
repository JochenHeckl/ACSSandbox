using System;

using UnityEngine;

using de.JochenHeckl.Unity.ACSSandbox.Example.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal interface IClientResources
	{
		ContextUIView ConnectToServerView { get; }
		ContextUIView EnterWorldView { get; }

		Camera LobbyCamera { get; }
		WorldCamera WorldCamera { get; }

		StringResources StringResources { get; }

		World GetWorld( Guid worldId );
		ClientUnitView GetUnitPrefab( UnitTypeId unitTypeId );
	}
}
