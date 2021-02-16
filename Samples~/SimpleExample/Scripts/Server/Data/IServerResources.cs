using System;

using de.JochenHeckl.Unity.ACSSandbox.Example.Common;


namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	internal interface IServerResources
	{
		World GetWorld( Guid worldId );
		ServerUnitView GetUnitPrefab( UnitTypeId unitTypeId );
	}
}
