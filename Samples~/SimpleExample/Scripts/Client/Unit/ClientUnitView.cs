using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public class ClientUnitView : UnitView
	{
		public ClientUnitData ClientUnitData { get; set; }

		public void Update()
		{
			if ( ClientUnitData != null )
			{
				transform.position = ClientUnitData.Position;
				transform.rotation = ClientUnitData.Rotation;
			}
		}
	}
}