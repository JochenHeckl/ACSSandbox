using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
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