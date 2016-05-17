namespace Discord.WebSocket
{
    public class MessageUpdatedEventArgs : MessageEventArgs
    {
        public Message Before { get; }
        public Message After => Message;

        public MessageUpdatedEventArgs(Message before, Message after)
            : base(after)
        {
            Before = before;
        }
    }
}
