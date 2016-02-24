using System;

namespace Discord
{
    public class LogMessageEventArgs : EventArgs
    {
        public LogSeverity Severity => default(LogSeverity);
        public string Source => null;
        public string Message => null;
        public Exception Exception => null;
    }
}
