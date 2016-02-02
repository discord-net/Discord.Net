using System;

namespace Discord
{
    public class LogMessageEventArgs : EventArgs
    {
        public LogSeverity Severity { get; }
        public string Source { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public LogMessageEventArgs(LogSeverity severity, string source, string msg, Exception exception)
        {
            Severity = severity;
            Source = source;
            Message = msg;
            Exception = exception;
        }
    }
}
