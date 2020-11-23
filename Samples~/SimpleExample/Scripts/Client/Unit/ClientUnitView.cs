using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public class ClientUnitView : UnitView
	{
		public ClientUnitData UnitData { get; set; }

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