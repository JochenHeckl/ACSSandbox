using System;

using UnityEngine;

using de.JochenHeckl.Unity.ACSSandbox.Common;


namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal interface IServerResources
	{
		World GetWorld( Guid worldId );
	}
}
