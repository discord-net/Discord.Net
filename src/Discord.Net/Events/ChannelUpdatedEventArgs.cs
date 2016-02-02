using System;

namespace Discord
{
    public class ChannelUpdatedEventArgs : EventArgs
    {
        public Channel Before { get; }
        public Channel After { get; }

        public Server Server => After.Server;

        public ChannelUpdatedEventArgs(Channel before, Channel after)
        {
            Before = before;
            After = after;
        }
    }
}
