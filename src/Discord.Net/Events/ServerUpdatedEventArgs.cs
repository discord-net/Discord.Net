using System;

namespace Discord
{
    public class ServerUpdatedEventArgs : EventArgs
    {
        public Server Before { get; }
        public Server After { get; }

        public ServerUpdatedEventArgs(Server before, Server after)
        {
            Before = before;
            After = after;
        }
    }
}
