﻿using System;
using System.Linq;
using System.Buffers;
using ACSSandbox.AreaServiceProtocol.ClientToServer;
using ACSSandbox.AreaServiceProtocol.ServerToClient;
using MemoryPack;
using MemoryPack.Compression;
using System.Diagnostics;
using System.IO;

namespace ACSSandbox.AreaServiceProtocol
{
    public class ProtocolSerializerMemoryPack<ClientIdType> :
        IClientMessageSerializer<ClientIdType>,
        IServerMessageSerializer
    {
        private delegate void ClientMessageHandler(ClientIdType sourceId, ReadOnlySequence<byte> messageBytes);
        private delegate void ServerMessageHandler(ReadOnlySpan<byte> messageBytes);
        
        readonly ClientMessageHandler[] clientMessageHandlers = new ClientMessageHandler[byte.MaxValue];
        readonly ServerMessageHandler[] serverMessageHandlers = new ServerMessageHandler[byte.MaxValue];

        public ReadOnlySpan<byte> Serialize<MessageType>(MessageType message)
            where MessageType : IMessage
        {
            using var compressor = new BrotliCompressor();
            compressor.Write( stackalloc[]{(byte)message.MessageTypeId} );
            MemoryPackSerializer.Serialize(compressor, message);
            return compressor.ToArray();
            
            // no compression version
            // var stream = new MemoryStream(256);
            // stream.Write(stackalloc[]{(byte)message.MessageTypeId});
            // stream.Write(MemoryPackSerializer.Serialize<MessageType>(message));

            // return stream.ToArray();
        }

        public void DeserializedDispatch(ReadOnlySpan<byte> messageRaw)
        {
            using var decompressor = new BrotliDecompressor();
            var messageBytes = decompressor.Decompress(messageRaw);
            
            var messageTypeId = messageBytes.FirstSpan[0];
            serverMessageHandlers[messageTypeId]?.Invoke(messageRaw[1..]);

            // no compression version
            // var messageTypeId = messageRaw[0];
            // serverMessageHandlers[messageTypeId]?.Invoke(messageRaw[1..]);
        }

        public void DeserializedDispatch(ClientIdType sourceId, ReadOnlySpan<byte> messageRaw)
        {
            using var decompressor = new BrotliDecompressor();
            var messageBytes = decompressor.Decompress(messageRaw);
            var messageTypeId = messageBytes.FirstSpan[0];

            clientMessageHandlers[messageTypeId]?.Invoke(sourceId, messageBytes.Slice(1));
        }

        public void RegisterServerMessageDispatch<MessageType>(MessageTypeId messageTypeId, Action<MessageType> dispatch) where MessageType : IMessage
        {
            serverMessageHandlers[(byte)messageTypeId] = (data) => dispatch(MemoryPackSerializer.Deserialize<MessageType>(data));
        }

        public void RegisterClientMessageDispatch<MessageType>(MessageTypeId messageTypeId, Action<ClientIdType, MessageType> dispatch) where MessageType : IMessage
        {
            clientMessageHandlers[(byte)messageTypeId] = (sourceId, data) => dispatch(sourceId, MemoryPackSerializer.Deserialize<MessageType>(data));
        }
    }
}
