using System;
using System.Buffers;
using ACSSandbox.AreaServiceProtocol.ClientToServer;
using ACSSandbox.AreaServiceProtocol.ServerToClient;
using MemoryPack;
using MemoryPack.Compression;

namespace ACSSandbox.AreaServiceProtocol
{
    public class ProtocolSerializerMemoryPack : IAreaServiceProtocolSerializer
    {
        private delegate IMessage Deserializer(ReadOnlySpan<byte> data);

        readonly Deserializer[] deserializers = new Deserializer[byte.MaxValue];

        public ProtocolSerializerMemoryPack()
        {
            deserializers[(byte)MessageTypeId.ClientHeartBeat] =
                DeserializeInternal<ClientHeartBeat>;
            deserializers[(byte)MessageTypeId.ServerHeartBeat] =
                DeserializeInternal<ServerHeartBeat>;
            deserializers[(byte)MessageTypeId.LoginRequest] = DeserializeInternal<LoginRequest>;
            deserializers[(byte)MessageTypeId.LoginResult] = DeserializeInternal<LoginResult>;
        }

        private static IMessage DeserializeInternal<MessageType>(ReadOnlySpan<byte> dataRaw)
            where MessageType : IMessage
        {
            return MemoryPackSerializer.Deserialize<MessageType>(dataRaw);
        }

        public ReadOnlySpan<byte> Serialize<MessageType>(MessageType message)
            where MessageType : IMessage
        {
            var typeIdAsBytes = new[] { (byte)message.MessageTypeId };

            using var compressor = new BrotliCompressor();
            compressor.Write(typeIdAsBytes);
            MemoryPackSerializer.Serialize(compressor, message);
            return compressor.ToArray();
        }

        public (MessageTypeId typeId, IMessage message) Deserialize(ReadOnlySpan<byte> messageRaw)
        {
            using var decompressor = new BrotliDecompressor();
            var messageBytes = decompressor.Decompress(messageRaw).ToArray();

            var messageTypeId = messageBytes[0];

            return (
                (MessageTypeId)messageTypeId,
                deserializers[messageTypeId](messageBytes.AsSpan(1))
            );
        }
    }
}
