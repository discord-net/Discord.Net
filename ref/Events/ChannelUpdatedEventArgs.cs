using System;

namespace Discord
{
    public class ChannelUpdatedEventArgs : EventArgs
    {
        public Channel Before => null;
        public Channel After => null;
        public Server Server => null;
    }
}
