using System;

namespace Discord
{
    public class ServerEventArgs : EventArgs
    {
        public Server Server { get; }

        public ServerEventArgs(Server server) { Server = server; }
    }
}
