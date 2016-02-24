using System;

namespace Discord
{
    public class ServerUpdatedEventArgs : EventArgs
    {
        public Server Before => null;
        public Server After => null;
    }
}
