using System;

namespace Discord
{
    public class DisconnectedEventArgs : EventArgs
    {
        public bool WasUnexpected => false;
        public Exception Exception => null;
    }
}
