using System;

namespace Discord.WebSocket
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
