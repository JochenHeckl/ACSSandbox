using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace de.JochenHeckl.Unity.ACSSandbox
{
	public class MessageSerializerJson : MessageSerializerWithTypeIdPrefix
	{
		public override object Deserialize( byte[] data, int offset, Type type )
		{
			var jsonString = Encoding.UTF8.GetString(
				data,
				offset,
				data.Length - offset );

			return JsonConvert.DeserializeObject( jsonString, type );
		}
		public override byte[] Serialize( MemoryStream memoryStream, object message )
		{
			using ( var streamWriter = new StreamWriter( memoryStream ) )
			{
				using ( var writer = new JsonTextWriter( streamWriter ) )
				{
					var serializer = new JsonSerializer();
					serializer.Serialize( writer, message );
				}
			}

			return memoryStream.ToArray();
		}
	}
}