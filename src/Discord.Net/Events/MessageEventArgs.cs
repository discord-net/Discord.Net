using System;

namespace Discord
{
    public class MessageEventArgs : EventArgs
    {
        public Message Message { get; }

        public User User => Message.User;
        public Channel Channel => Message.Channel;
        public Server Server => Message.Server;

        public MessageEventArgs(Message msg) { Message = msg; }
    }
}
