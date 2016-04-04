using System;

namespace Discord
{
    public class ChannelEventArgs : EventArgs
    {
        public IChannel Channel { get; }

        public ChannelEventArgs(IChannel channel)
        {
            Channel = channel;
        }
    }
}
