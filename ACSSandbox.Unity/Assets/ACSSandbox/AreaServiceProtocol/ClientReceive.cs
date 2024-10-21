using System;
using ACSSandbox.AreaServiceProtocol.ServerToClient;
using ACSSandbox.Common.Network;
using UnityEditor;

namespace ACSSandbox.AreaServiceProtocol
{
    public class ClientReceive
    {
        private readonly IClientMessageSerializer<NetworkId> serializer;

        public ClientReceive(IClientMessageSerializer<NetworkId> serializer)
        {
            this.serializer = serializer;
        }

        public void RegisterMessageHandler<Message>(
            MessageTypeId messageTypeId,
            Action<Message> handler
        )
            where Message : IMessage
        {
            serializer.RegisterServerMessageDispatch<Message>(messageTypeId, handler);
        }

        public void HandleInboundData(ReadOnlySpan<byte> inboundData)
        {
            serializer.DeserializedDispatch(inboundData);
        }
    }
}
