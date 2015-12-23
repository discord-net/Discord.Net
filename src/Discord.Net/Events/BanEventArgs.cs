using System;

namespace Discord
{
    public class BanEventArgs : EventArgs
    {
        public Server Server { get; }
        public ulong UserId { get; }

        public BanEventArgs(Server server, ulong userId)
        {
            Server = server;
            UserId = userId;
        }
    }
}
