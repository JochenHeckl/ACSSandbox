using System;
using System.Buffers;
using System.IO;
using ACSSandbox.Common.Network;
using MemoryPack;
using MemoryPack.Compression;

namespace ACSSandbox.AreaServiceProtocol
{
    public class ProtocolSerializerMemoryPack<ClientIdType>
        : IClientMessageSerializer<ClientIdType>,
            IServerMessageSerializer<ClientIdType>
    {
        private delegate void ClientMessageHandler(
            ClientIdType sourceId,
            ReadOnlySequence<byte> messageBytes
        );
        private delegate void ServerMessageHandler(ReadOnlySpan<byte> messageBytes);

        private readonly ClientMessageHandler[] clientMessageHandlers = new ClientMessageHandler[
            byte.MaxValue
        ];
        private readonly ServerMessageHandler[] serverMessageHandlers = new ServerMessageHandler[
            byte.MaxValue
        ];

        private bool useCompression;
        private BrotliCompressor compressor;
        private BrotliDecompressor decompressor;

        public ProtocolSerializerMemoryPack(bool useCompression)
        {
            this.useCompression = useCompression;

            if (useCompression)
            {
                compressor = new();
            }
        }

        public ReadOnlySpan<byte> Serialize<MessageType>(MessageType message)
            where MessageType : IMessage
        {
            if (useCompression)
            {
                compressor.Write(stackalloc[] { (byte)message.MessageTypeId });
                MemoryPackSerializer.Serialize(compressor, message);
                return compressor.ToArray();
            }
            else
            {
                var stream = new MemoryStream(4096);
                stream.Write(stackalloc[] { (byte)message.MessageTypeId });
                stream.Write(MemoryPackSerializer.Serialize<MessageType>(message));

                return stream.ToArray();
            }
        }

        public void DeserializedDispatch(ReadOnlySpan<byte> messageRaw)
        {
            if (useCompression)
            {
                var messageBytes = decompressor.Decompress(messageRaw);

                var messageTypeId = messageBytes.FirstSpan[0];
                serverMessageHandlers[messageTypeId]?.Invoke(messageRaw[1..]);
            }
            else
            {
                var messageTypeId = messageRaw[0];
                serverMessageHandlers[messageTypeId]?.Invoke(messageRaw[1..]);
            }
        }

        public void DeserializedDispatch(ClientIdType sourceId, ReadOnlySpan<byte> messageRaw)
        {
            using var decompressor = new BrotliDecompressor();
            var messageBytes = decompressor.Decompress(messageRaw);
            var messageTypeId = messageBytes.FirstSpan[0];

            clientMessageHandlers[messageTypeId]?.Invoke(sourceId, messageBytes.Slice(1));
        }

        public void RegisterServerMessageDispatch<MessageType>(
            MessageTypeId messageTypeId,
            Action<MessageType> dispatch
        )
            where MessageType : IMessage
        {
            serverMessageHandlers[(byte)messageTypeId] = (data) =>
                dispatch(MemoryPackSerializer.Deserialize<MessageType>(data));
        }

        public void RegisterClientMessageDispatch<MessageType>(
            MessageTypeId messageTypeId,
            Action<ClientIdType, MessageType> dispatch
        )
            where MessageType : IMessage
        {
            clientMessageHandlers[(byte)messageTypeId] = (sourceId, data) =>
                dispatch(sourceId, MemoryPackSerializer.Deserialize<MessageType>(data));
        }

        public void RegisterClientMessageDispatch<Message>(
            MessageTypeId messageTypeId,
            Action<NetworkId, Message> handler
        )
            where Message : IMessage
        {
            throw new NotImplementedException();
        }
    }
}
