// using System;

// namespace ACSSandbox.AreaServiceProtocol
// {
//     public interface IServerMessageSerializer<ClientIdTpe>
//     {
//         ReadOnlySpan<byte> Serialize<MessageType>(MessageType message)
//             where MessageType : IMessage;
//         void DeserializedDispatch(ReadOnlySpan<byte> data);

//         void RegisterServerMessageDispatch<MessageType>(
//             MessageIdType messageTypeId,
//             Action<MessageType> dispatch
//         )
//             where MessageType : IMessage<MessageIdType>;
//     }
// }
