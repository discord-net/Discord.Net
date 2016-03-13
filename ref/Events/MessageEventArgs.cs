using System;

namespace Discord
{
    public class MessageEventArgs : EventArgs
    {
        public Message Message => null;
        public IUser User => null;
        public ITextChannel Channel => null;
    }
}
