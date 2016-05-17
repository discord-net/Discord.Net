using System;

namespace Discord.WebSocket
{
    public class DisconnectedEventArgs : EventArgs
    {
        public bool WasUnexpected { get; }
        public Exception Exception { get; }

        public DisconnectedEventArgs(bool wasUnexpected, Exception exception = null)
        {
            WasUnexpected = wasUnexpected;
            Exception = exception;
        }
    }
}
