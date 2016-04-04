using System;

namespace Discord
{
    public class TypingEventArgs : EventArgs
    {
        public IMessageChannel Channel { get; }
        public User User { get; }

        public TypingEventArgs(IMessageChannel channel, User user)
        {
            Channel = channel;
            User = user;
        }
    }
}
