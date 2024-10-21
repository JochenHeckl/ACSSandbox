namespace ACSSandbox.AreaServiceProtocol
{
    public interface IMessage<MessageIdType>
    {
        public MessageIdType MessageTypeId { get; }
    }
}
