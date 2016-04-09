using System;

namespace Discord
{
    public class TypingEventArgs : EventArgs
    {
        public IMessageChannel Channel { get; }
        public IUser User { get; }

        public TypingEventArgs(IMessageChannel channel, IUser user)
        {
            Channel = channel;
            User = user;
        }
    }
}
