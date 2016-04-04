using System;

namespace Discord.Logging
{
    public class LogManager
    {
        public LogSeverity Level { get; }

        public event EventHandler<LogMessageEventArgs> Message = delegate { };

        internal LogManager(LogSeverity minSeverity)
        {
            Level = minSeverity;
        }

        public void Log(LogSeverity severity, string source, string message, Exception exception = null)
        {
            if (severity <= Level)
                Message(this, new LogMessageEventArgs(severity, source, message, exception));
        }
        public void Log(LogSeverity severity, string source, FormattableString message, Exception exception = null)
        {
            if (severity <= Level)
                Message(this, new LogMessageEventArgs(severity, source, message.ToString(), exception));
        }

        public void Error(string source, string message, Exception ex = null)
            => Log(LogSeverity.Error, source, message, ex);
        public void Error(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Error, source, message, ex);
        public void Error(string source, Exception ex)
            => Log(LogSeverity.Error, source, (string)null, ex);

        public void Warning(string source, string message, Exception ex = null)
            => Log(LogSeverity.Warning, source, message, ex);
        public void Warning(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Warning, source, message, ex);
        public void Warning(string source, Exception ex)
            => Log(LogSeverity.Warning, source, (string)null, ex);

        public void Info(string source, string message, Exception ex = null)
            => Log(LogSeverity.Info, source, message, ex);
        public void Info(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Info, source, message, ex);
        public void Info(string source, Exception ex)
            => Log(LogSeverity.Info, source, (string)null, ex);

        public void Verbose(string source, string message, Exception ex = null)
            => Log(LogSeverity.Verbose, source, message, ex);
        public void Verbose(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Verbose, source, message, ex);
        public void Verbose(string source, Exception ex)
            => Log(LogSeverity.Verbose, source, (string)null, ex);

        public void Debug(string source, string message, Exception ex = null)
            => Log(LogSeverity.Debug, source, message, ex);
        public void Debug(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Debug, source, message, ex);
        public void Debug(string source, Exception ex)
            => Log(LogSeverity.Debug, source, (string)null, ex);        

        internal Logger CreateLogger(string name) => new Logger(this, name);
    }
}
