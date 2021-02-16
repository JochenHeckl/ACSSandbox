namespace de.JochenHeckl.Unity.ACSSandbox
{
	public interface IMessageSerializer
	{
		byte[] Serialize( object message );
		object Deserialize( byte[] messageRaw );
	}
}