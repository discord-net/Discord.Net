using System;

namespace Discord
{
    public class ChannelEventArgs : EventArgs
    {
        public Channel Channel { get; }

        public Server Server => Channel.Server;

        public ChannelEventArgs(Channel channel) { Channel = channel; }
    }
}
