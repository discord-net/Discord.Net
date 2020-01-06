using System;
using System.Text;

namespace Discord
{
    public enum LogSeverity
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error
    }

    public struct LogMessage
    {
        public LogSeverity Level { get; }
        public string Source { get; }
        public string Message { get; }
        public Exception? Exception { get; }

        public LogMessage(LogSeverity level, string source, string message, Exception? exception = null)
        {
            Level = level;
            Source = source;
            Message = message;
            Exception = exception;
        }

        public override string ToString() => ToString();
        public string ToString(StringBuilder? builder = null,
            bool fullException = true,
            bool prependTimestamp = true,
            DateTimeKind timestampKind = DateTimeKind.Local,
            int? padSource = 11)
        {
            string? exMessage = fullException ? Exception?.ToString() : Exception?.Message;
            int maxLength = 1 +
                (prependTimestamp ? 8 : 0) + 1 +
                (padSource.HasValue ? padSource.Value : Source?.Length ?? 0) + 1 +
                (Message?.Length ?? 0) +
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
            if (Source != null)
            {
                if (padSource.HasValue)
                {
                    if (Source.Length < padSource.Value)
                    {
                        builder.Append(Source);
                        builder.Append(' ', padSource.Value - Source.Length);
                    }
                    else if (Source.Length > padSource.Value)
                        builder.Append(Source.Substring(0, padSource.Value));
                    else
                        builder.Append(Source);
                }
                else
                    builder.Append(Source);
                builder.Append(' ');
            }
            if (!string.IsNullOrEmpty(Message))
            {
                char c;
                for (int i = 0; i < Message.Length; i++)
                {
                    c = Message[i];
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

    public class Logger
    {
        public event Action<LogMessage>? Message;
        public string Name { get; set; }
        public LogSeverity MinSeverity { get; set; }

        public Logger(string source, LogSeverity minSeverity)
        {
            Name = source;
            MinSeverity = minSeverity;
        }

        public void Log(LogMessage message)
        {
            if (message.Level < MinSeverity)
                return;
            Message?.Invoke(message);
        }

        public void Log(LogSeverity severity, string message, Exception? err = null)
            => Log(new LogMessage(severity, Name, message, err));

        public void Trace(string message, Exception? err = null)
            => Log(LogSeverity.Trace, message, err);
        public void Debug(string message, Exception? err = null)
            => Log(LogSeverity.Debug, message, err);
        public void Info(string message, Exception? err = null)
            => Log(LogSeverity.Info, message, err);
        public void Warn(string message, Exception? err = null)
            => Log(LogSeverity.Warn, message, err);
        public void Error(string message, Exception? err = null)
            => Log(LogSeverity.Error, message, err);

        public void Trace(Exception err)
            => Log(LogSeverity.Trace, null!, err);
        public void Debug(Exception err)
            => Log(LogSeverity.Debug, null!, err);
        public void Info(Exception err)
            => Log(LogSeverity.Info, null!, err);
        public void Warn(Exception err)
            => Log(LogSeverity.Warn, null!, err);
        public void Error(Exception err)
            => Log(LogSeverity.Error, null!, err);

    }
}
