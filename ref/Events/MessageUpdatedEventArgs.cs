using System;

namespace Discord
{
    public class MessageUpdatedEventArgs : EventArgs
    {
        public Message Before => null;
        public Message After => null;
        public IUser User => null;
        public ITextChannel Channel => null;
    }
}
