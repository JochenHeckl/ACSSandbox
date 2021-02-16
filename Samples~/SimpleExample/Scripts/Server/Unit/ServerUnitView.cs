using de.JochenHeckl.Unity.ACSSandbox.Example.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	public class ServerUnitView : UnitView
	{
		public IServerUnitData UnitData { get; set; }

		public void Update()
		{
			if ( UnitData != null )
			{
				transform.position = UnitData.Position;
				transform.rotation = UnitData.Rotation;
			}
		}
	}
}