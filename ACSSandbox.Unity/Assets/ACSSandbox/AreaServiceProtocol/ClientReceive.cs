using System;
using ACSSandbox.AreaServiceProtocol.ServerToClient;

namespace ACSSandbox.AreaServiceProtocol
{
    public delegate void ServerHeartBeatHandler(float serverTimeSec);
    public delegate void LoginResultHandler(LoginResultType result);

    public class ClientReceive
    {
        private readonly IAreaServiceProtocolSerializer serializer;
        private readonly Action<IMessage>[] dispatchers = new Action<IMessage>[byte.MaxValue];

        public ClientReceive(IAreaServiceProtocolSerializer serializer)
        {
            this.serializer = serializer;
        }

        public ServerHeartBeatHandler HandleServerHeartBeat
        {
            set =>
                dispatchers[(byte)MessageTypeId.ServerHeartBeat] = (message) =>
                {
                    if (message is ServerHeartBeat typeSafeMessage)
                    {
                        value(typeSafeMessage.serverTimeSec);
                    }
                };
        }
        public LoginResultHandler HandleLoginResult
        {
            set =>
                dispatchers[(byte)MessageTypeId.LoginResult] = (message) =>
                {
                    if (message is LoginResult typeSafeMessage)
                    {
                        value(typeSafeMessage.result);
                    }
                };
        }

        public void HandleInboundData(ReadOnlySpan<byte> inboundData)
        {
            var (messageTypeId, message) = serializer.Deserialize(inboundData);
            var dispatcher = dispatchers[(byte)messageTypeId];

            if (dispatcher != null)
            {
                dispatcher(message);
            }
            else
            {
                DiscardMessage(message);
            }
        }

        private static void DiscardMessage(IMessage message)
        {
            throw new InvalidProgramException(
                $"You received a message of type {message.GetType().Name} but there was no dispatcher registered to handle it."
            );
        }
    }
}
