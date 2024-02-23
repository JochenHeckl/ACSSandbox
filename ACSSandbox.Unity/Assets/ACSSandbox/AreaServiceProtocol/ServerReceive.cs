using System;
using ACSSandbox.AreaServiceProtocol.ClientToServer;
using ACSSandbox.Common.Network;

namespace ACSSandbox.AreaServiceProtocol
{
    public delegate void ClientHeartBeatHandler(NetworkId networkId, float clientTimeSec);
    public delegate void LoginRequestHandler(NetworkId networkId, string secret);

    public class ServerReceive
    {
        private readonly IAreaServiceProtocolSerializer serializer;
        private readonly Action<NetworkId, IMessage>[] dispatchers = new Action<
            NetworkId,
            IMessage
        >[byte.MaxValue];

        public ServerReceive(IAreaServiceProtocolSerializer serializer)
        {
            this.serializer = serializer;
        }

        public ClientHeartBeatHandler HandleClientHeartBeat
        {
            set =>
                dispatchers[(byte)MessageTypeId.ClientHeartBeat] = (networkId, message) =>
                {
                    if (message is ClientHeartBeat typeSafeMessage)
                    {
                        value(networkId, typeSafeMessage.clientTimeSec);
                    }
                };
        }

        public LoginRequestHandler HandleLoginRequest
        {
            set =>
                dispatchers[(byte)MessageTypeId.LoginRequest] = (networkId, message) =>
                {
                    if (message is LoginRequest typeSafeMessage)
                    {
                        value(networkId, typeSafeMessage.secret);
                    }
                };
        }

        public void HandleInboundData(NetworkId networkId, ReadOnlySpan<byte> inboundData)
        {
            var (messageTypeId, message) = serializer.Deserialize(inboundData);
            var dispatcher = dispatchers[(byte)messageTypeId];

            if (dispatcher != null)
            {
                dispatcher(networkId, message);
            }
            else
            {
                DiscardMessage(networkId, message);
            }
        }

        private static void DiscardMessage(NetworkId networkId, IMessage message)
        {
            throw new InvalidProgramException(
                $"You received a message of type {message.GetType().Name} from networkId {networkId} but there was no dispatcher registered to handle it."
            );
        }
    }
}
