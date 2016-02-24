using System;

namespace Discord
{
    public class MessageUpdatedEventArgs : EventArgs
    {
        public Message Before => null;
        public Message After => null;
        public User User => null;
        public Channel Channel => null;
        public Server Server => null;
    }
}
