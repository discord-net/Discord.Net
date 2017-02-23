using System;
using System.Text;

namespace Discord
{
    public struct LogMessage
    {
        public LogSeverity Severity { get; }
        public string Source { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public LogMessage(LogSeverity severity, string source, string message, Exception exception = null)
        {
            Severity = severity;
            Source = source;
            Message = message;
            Exception = exception;
        }

        public override string ToString() => ToString(null);
        public string ToString(StringBuilder builder = null, bool fullException = true, bool prependTimestamp = true, DateTimeKind timestampKind = DateTimeKind.Local, int? padSource = 11)
        {
            string sourceName = Source;
            string message = Message;
            string exMessage = fullException ? Exception?.ToString() : Exception?.Message;

            int maxLength = 1 + 
                (prependTimestamp ? 8 : 0) + 1 +
                (padSource.HasValue ? padSource.Value : sourceName?.Length ?? 0) + 1 + 
                (message?.Length ?? 0) +
                (exMessage?.Length ?? 0) + 3;

            if (builder == null)
                builder = new StringBuilder(maxLength);
            else
            {
                builder.Clear();
                builder.EnsureCapacity(maxLength);
            }

            if (prependTimestamp)
            {
                DateTime now;
                if (timestampKind == DateTimeKind.Utc)
                    now = DateTime.UtcNow;
                else
                    now = DateTime.Now;
                if (now.Hour < 10)
                    builder.Append('0');
                builder.Append(now.Hour);
                builder.Append(':');
                if (now.Minute < 10)
                    builder.Append('0');
                builder.Append(now.Minute);
                builder.Append(':');
                if (now.Second < 10)
                    builder.Append('0');
                builder.Append(now.Second);
                builder.Append(' ');
            }
            if (sourceName != null)
            {
                if (padSource.HasValue)
                {
                    if (sourceName.Length < padSource.Value)
                    {
                        builder.Append(sourceName);
                        builder.Append(' ', padSource.Value - sourceName.Length);
                    }
                    else if (sourceName.Length > padSource.Value)
                        builder.Append(sourceName.Substring(0, padSource.Value));
                    else
                        builder.Append(sourceName);
                }
                builder.Append(' ');
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
                if (!string.IsNullOrEmpty(Message))
                {
                    builder.Append(':');
                    builder.AppendLine();
                }
                builder.Append(exMessage);
            }

            return builder.ToString();
        }
    }
}
