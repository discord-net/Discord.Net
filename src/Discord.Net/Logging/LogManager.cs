using System;

namespace Discord.Logging
{
    public class LogManager
    {
        private readonly DiscordClient _client;

        public LogSeverity Level { get; }

        public event EventHandler<LogMessageEventArgs> Message = delegate { };

        internal LogManager(DiscordClient client)
		{
            _client = client;
            Level = client.Config.LogLevel;
        }

        public void Log(LogSeverity severity, string source, FormattableString message, Exception exception = null)
        {
            if (severity <= Level)
            {
                try { Message(this, new LogMessageEventArgs(severity, source, message.ToString(), exception)); }
                catch { } //We dont want to log on log errors
            }
        }
        public void Log(LogSeverity severity, string source, string message, Exception exception = null)
        {
            if (severity <= Level)
            {
                try { Message(this, new LogMessageEventArgs(severity, source, message, exception)); }
                catch { } //We dont want to log on log errors
            }
        }

        public void Error(string source, string message, Exception ex = null)
            => Log(LogSeverity.Error, source, message, ex);
        public void Error(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Error, source, message, ex);
        public void Warning(string source, string message, Exception ex = null)
            => Log(LogSeverity.Warning, source, message, ex);
        public void Warning(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Warning, source, message, ex);
        public void Info(string source, string message, Exception ex = null)
            => Log(LogSeverity.Info, source, message, ex);
        public void Info(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Info, source, message, ex);
        public void Verbose(string source, string message, Exception ex = null)
            => Log(LogSeverity.Verbose, source, message, ex);
        public void Verbose(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Verbose, source, message, ex);
        public void Debug(string source, string message, Exception ex = null)
            => Log(LogSeverity.Debug, source, message, ex);
        public void Debug(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Debug, source, message, ex);

        public Logger CreateLogger(string name) => new Logger(this, name);
    }
}
