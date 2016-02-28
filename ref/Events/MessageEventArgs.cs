using System;

namespace Discord
{
    public class MessageEventArgs : EventArgs
    {
        public Message Message => null;
        public User User => null;
        public ITextChannel Channel => null;
    }
}
