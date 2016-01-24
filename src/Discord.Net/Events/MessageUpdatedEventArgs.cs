using System;

namespace Discord
{
    public class MessageUpdatedEventArgs : EventArgs
    {
        public Message Before { get; }
        public Message After { get; }

        public User User => After.User;
        public Channel Channel => After.Channel;
        public Server Server => After.Server;

        public MessageUpdatedEventArgs(Message before, Message after)
        {
            Before = before;
            After = after;
        }
    }
}
