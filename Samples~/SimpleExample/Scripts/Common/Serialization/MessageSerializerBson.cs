using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
	public class MessageSerializerBson : MessageSerializerWithTypeIdPrefix
	{
		public override byte[] Serialize( MemoryStream memoryStream, object message )
		{
#pragma warning disable CS0618 // Type or member is obsolete
			using ( var writer = new BsonWriter( memoryStream ) )
#pragma warning restore CS0618 // Type or member is obsolete
			{
				var serializer = new JsonSerializer();
				serializer.Serialize( writer, message );
			}

			return memoryStream.ToArray();
		}

		public override object Deserialize( byte[] data, int offset, Type type )
		{
#pragma warning disable CS0618 // Type or member is obsolete
			using ( var reader = new BsonReader( new MemoryStream( data, offset, data.Length - offset ) ) )
#pragma warning restore CS0618 // Type or member is obsolete
			{
				var serializer = new JsonSerializer();
				return serializer.Deserialize( reader, type );
			}
		}
	}
}