namespace Discord.WebSocket
{
    public class ChannelUpdatedEventArgs : ChannelEventArgs
    {
        public IChannel Before { get; }
        public IChannel After => Channel;

        public ChannelUpdatedEventArgs(IChannel before, IChannel after)
            : base(after)
        {
            Before = before;
        }
    }
}
