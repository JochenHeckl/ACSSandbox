﻿using System;
using System.Collections.Generic;
using System.IO;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
	public abstract class MessageSerializerWithTypeIdPrefix : IMessageSerializer
	{
		private Dictionary<byte, Type> messageTypeMap = new Dictionary<byte, Type>();
		private Dictionary<Type, byte> messageTypeIdMap = new Dictionary<Type, byte>();

		public abstract byte[] Serialize( MemoryStream memoryStream, object message );
		public abstract object Deserialize( byte[] data, int offset, Type type );

		public void RegisterType( byte typeId, Type type )
		{
			messageTypeMap[typeId] = type;
			messageTypeIdMap[type] = typeId;
		}

		public object Deserialize( byte[] messageRaw )
		{
			return Deserialize( messageRaw, 1, messageTypeMap[messageRaw[0]] );
		}

		public byte[] Serialize( object message )
		{
			if ( messageTypeIdMap.TryGetValue( message.GetType(), out var typeId ) )
			{
				var memoryStream = new MemoryStream();
				memoryStream.WriteByte( typeId );

				return Serialize( memoryStream, message );
			}

			throw new InvalidOperationException($"Message of type {message.GetType().Name} is unknown. Did you forget to Register the message type?" );
		}
	}
}
