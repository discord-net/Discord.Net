using System;
using System.Text;

namespace Discord
{
    public class LogMessageEventArgs : EventArgs
    {
        public LogSeverity Severity { get; }
        public string Source { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public LogMessageEventArgs(LogSeverity severity, string source, string message, Exception exception = null)
        {
            Severity = severity;
            Source = source;
            Message = message;
            Exception = exception;
        }

        public override string ToString() => ToString(null, true);

        public string ToString(StringBuilder builder = null, bool fullException = true)
        {
            string sourceName = Source;
            string message = Message;
            string exMessage = fullException ? Exception?.ToString() : Exception?.Message;

            int maxLength = 1 + (sourceName?.Length ?? 0) + 2 + (message?.Length ?? 0) + 3 + (exMessage?.Length ?? 0);
            if (builder == null)
                builder = new StringBuilder(maxLength);
            else
            {
                builder.Clear();
                builder.EnsureCapacity(maxLength);
            }

            if (sourceName != null)
            {
                builder.Append('[');
                builder.Append(sourceName);
                builder.Append("] ");
            }
            if (!string.IsNullOrEmpty(Message))
            {
                for (int i = 0; i < message.Length; i++)
                {
                    //Strip control chars
                    char c = message[i];
                    if (!char.IsControl(c))
                        builder.Append(c);
                }
            }
            if (exMessage != null)
            {
                builder.AppendLine(":");
                builder.Append(exMessage);
            }

            return builder.ToString();
        }
    }
}
