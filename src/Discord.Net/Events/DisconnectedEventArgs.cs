using System;

namespace Discord
{
    public class DisconnectedEventArgs : EventArgs
    {
        public bool WasUnexpected { get; }
        public Exception Exception { get; }

        public DisconnectedEventArgs(bool wasUnexpected, Exception ex)
        {
            WasUnexpected = wasUnexpected;
            Exception = ex;
        }
    }
}
