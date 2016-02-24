using System;

namespace Discord
{
    public class ChannelUpdatedEventArgs : EventArgs
    {
        public IChannel Before { get; }
        public IChannel After { get; }

        public ChannelUpdatedEventArgs(IChannel before, IChannel after)
        {
            Before = before;
            After = after;
        }
    }
}
