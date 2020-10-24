namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
	public interface IMessageSerializer
	{
		byte[] Serialize( object message );
		object Deserialize( byte[] messageRaw );
	}
}