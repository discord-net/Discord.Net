using System;

namespace Discord.Logging
{
    internal class LogManager : ILogger
    {
        public LogSeverity Level { get; }

        public event EventHandler<LogMessageEventArgs> Message = delegate { };

        internal LogManager(LogSeverity minSeverity)
        {
            Level = minSeverity;
        }

        public void Log(LogSeverity severity, string source, string message, Exception ex = null)
        {
            if (severity <= Level)
                Message(this, new LogMessageEventArgs(severity, source, message, ex));
        }
        public void Log(LogSeverity severity, string source, FormattableString message, Exception ex = null)
        {
            if (severity <= Level)
                Message(this, new LogMessageEventArgs(severity, source, message.ToString(), ex));
        }
        public void Log(LogSeverity severity, string source, Exception ex)
        {
            if (severity <= Level)
                Message(this, new LogMessageEventArgs(severity, source, null, ex));
        }
        void ILogger.Log(LogSeverity severity, string message, Exception ex)
        {
            if (severity <= Level)
                Message(this, new LogMessageEventArgs(severity, "Discord", message, ex));
        }
        void ILogger.Log(LogSeverity severity, FormattableString message, Exception ex)
        {
            if (severity <= Level)
                Message(this, new LogMessageEventArgs(severity, "Discord", message.ToString(), ex));
        }
        void ILogger.Log(LogSeverity severity, Exception ex)
        {
            if (severity <= Level)
                Message(this, new LogMessageEventArgs(severity, "Discord", null, ex));
        }

        public void Error(string source, string message, Exception ex = null)
            => Log(LogSeverity.Error, source, message, ex);
        public void Error(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Error, source, message, ex);
        public void Error(string source, Exception ex)
            => Log(LogSeverity.Error, source, ex);
        void ILogger.Error(string message, Exception ex)
            => Log(LogSeverity.Error, "Discord", message, ex);
        void ILogger.Error(FormattableString message, Exception ex)
            => Log(LogSeverity.Error, "Discord", message, ex);
        void ILogger.Error(Exception ex)
            => Log(LogSeverity.Error, "Discord", ex);

        public void Warning(string source, string message, Exception ex = null)
            => Log(LogSeverity.Warning, source, message, ex);
        public void Warning(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Warning, source, message, ex);
        public void Warning(string source, Exception ex)
            => Log(LogSeverity.Warning, source, ex);
        void ILogger.Warning(string message, Exception ex)
            => Log(LogSeverity.Warning, "Discord", message, ex);
        void ILogger.Warning(FormattableString message, Exception ex)
            => Log(LogSeverity.Warning, "Discord", message, ex);
        void ILogger.Warning(Exception ex)
            => Log(LogSeverity.Warning, "Discord", ex);

        public void Info(string source, string message, Exception ex = null)
            => Log(LogSeverity.Info, source, message, ex);
        public void Info(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Info, source, message, ex);
        public void Info(string source, Exception ex)
            => Log(LogSeverity.Info, source, ex);
        void ILogger.Info(string message, Exception ex)
            => Log(LogSeverity.Info, "Discord", message, ex);
        void ILogger.Info(FormattableString message, Exception ex)
            => Log(LogSeverity.Info, "Discord", message, ex);
        void ILogger.Info(Exception ex)
            => Log(LogSeverity.Info, "Discord", ex);

        public void Verbose(string source, string message, Exception ex = null)
            => Log(LogSeverity.Verbose, source, message, ex);
        public void Verbose(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Verbose, source, message, ex);
        public void Verbose(string source, Exception ex)
            => Log(LogSeverity.Verbose, source, ex);
        void ILogger.Verbose(string message, Exception ex)
            => Log(LogSeverity.Verbose, "Discord", message, ex);
        void ILogger.Verbose(FormattableString message, Exception ex)
            => Log(LogSeverity.Verbose, "Discord", message, ex);
        void ILogger.Verbose(Exception ex)
            => Log(LogSeverity.Verbose, "Discord", ex);

        public void Debug(string source, string message, Exception ex = null)
            => Log(LogSeverity.Debug, source, message, ex);
        public void Debug(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Debug, source, message, ex);
        public void Debug(string source, Exception ex)
            => Log(LogSeverity.Debug, source, ex);
        void ILogger.Debug(string message, Exception ex)
            => Log(LogSeverity.Debug, "Discord", message, ex);
        void ILogger.Debug(FormattableString message, Exception ex)
            => Log(LogSeverity.Debug, "Discord", message, ex);
        void ILogger.Debug(Exception ex)
            => Log(LogSeverity.Debug, "Discord", ex);

        internal Logger CreateLogger(string name) => new Logger(this, name);
    }
}
