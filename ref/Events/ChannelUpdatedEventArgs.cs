using System;

namespace Discord
{
    public class ChannelUpdatedEventArgs : EventArgs
    {
        public IChannel Before => null;
        public IChannel After => null;
    }
}
