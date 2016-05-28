using System;
using System.Threading.Tasks;

namespace Discord.Logging
{
    internal class LogManager : ILogger
    {
        public LogSeverity Level { get; }

        public event Func<LogMessage, Task> Message;

        internal LogManager(LogSeverity minSeverity)
        {
            Level = minSeverity;
        }

        public async Task Log(LogSeverity severity, string source, string message, Exception ex = null)
        {
            if (severity <= Level)
                await Message.Raise(new LogMessage(severity, source, message, ex)).ConfigureAwait(false);
        }
        public async Task Log(LogSeverity severity, string source, FormattableString message, Exception ex = null)
        {
            if (severity <= Level)
                await Message.Raise(new LogMessage(severity, source, message.ToString(), ex)).ConfigureAwait(false);
        }
        public async Task Log(LogSeverity severity, string source, Exception ex)
        {
            if (severity <= Level)
                await Message.Raise(new LogMessage(severity, source, null, ex)).ConfigureAwait(false);
        }
        async Task ILogger.Log(LogSeverity severity, string message, Exception ex)
        {
            if (severity <= Level)
                await Message.Raise(new LogMessage(severity, "Discord", message, ex)).ConfigureAwait(false);
        }
        async Task ILogger.Log(LogSeverity severity, FormattableString message, Exception ex)
        {
            if (severity <= Level)
                await Message.Raise(new LogMessage(severity, "Discord", message.ToString(), ex)).ConfigureAwait(false);
        }
        async Task ILogger.Log(LogSeverity severity, Exception ex)
        {
            if (severity <= Level)
                await Message.Raise(new LogMessage(severity, "Discord", null, ex)).ConfigureAwait(false);
        }

        public Task Error(string source, string message, Exception ex = null)
            => Log(LogSeverity.Error, source, message, ex);
        public Task Error(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Error, source, message, ex);
        public Task Error(string source, Exception ex)
            => Log(LogSeverity.Error, source, ex);
        Task ILogger.Error(string message, Exception ex)
            => Log(LogSeverity.Error, "Discord", message, ex);
        Task ILogger.Error(FormattableString message, Exception ex)
            => Log(LogSeverity.Error, "Discord", message, ex);
        Task ILogger.Error(Exception ex)
            => Log(LogSeverity.Error, "Discord", ex);

        public Task Warning(string source, string message, Exception ex = null)
            => Log(LogSeverity.Warning, source, message, ex);
        public Task Warning(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Warning, source, message, ex);
        public Task Warning(string source, Exception ex)
            => Log(LogSeverity.Warning, source, ex);
        Task ILogger.Warning(string message, Exception ex)
            => Log(LogSeverity.Warning, "Discord", message, ex);
        Task ILogger.Warning(FormattableString message, Exception ex)
            => Log(LogSeverity.Warning, "Discord", message, ex);
        Task ILogger.Warning(Exception ex)
            => Log(LogSeverity.Warning, "Discord", ex);

        public Task Info(string source, string message, Exception ex = null)
            => Log(LogSeverity.Info, source, message, ex);
        public Task Info(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Info, source, message, ex);
        public Task Info(string source, Exception ex)
            => Log(LogSeverity.Info, source, ex);
        Task ILogger.Info(string message, Exception ex)
            => Log(LogSeverity.Info, "Discord", message, ex);
        Task ILogger.Info(FormattableString message, Exception ex)
            => Log(LogSeverity.Info, "Discord", message, ex);
        Task ILogger.Info(Exception ex)
            => Log(LogSeverity.Info, "Discord", ex);

        public Task Verbose(string source, string message, Exception ex = null)
            => Log(LogSeverity.Verbose, source, message, ex);
        public Task Verbose(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Verbose, source, message, ex);
        public Task Verbose(string source, Exception ex)
            => Log(LogSeverity.Verbose, source, ex);
        Task ILogger.Verbose(string message, Exception ex)
            => Log(LogSeverity.Verbose, "Discord", message, ex);
        Task ILogger.Verbose(FormattableString message, Exception ex)
            => Log(LogSeverity.Verbose, "Discord", message, ex);
        Task ILogger.Verbose(Exception ex)
            => Log(LogSeverity.Verbose, "Discord", ex);

        public Task Debug(string source, string message, Exception ex = null)
            => Log(LogSeverity.Debug, source, message, ex);
        public Task Debug(string source, FormattableString message, Exception ex = null)
            => Log(LogSeverity.Debug, source, message, ex);
        public Task Debug(string source, Exception ex)
            => Log(LogSeverity.Debug, source, ex);
        Task ILogger.Debug(string message, Exception ex)
            => Log(LogSeverity.Debug, "Discord", message, ex);
        Task ILogger.Debug(FormattableString message, Exception ex)
            => Log(LogSeverity.Debug, "Discord", message, ex);
        Task ILogger.Debug(Exception ex)
            => Log(LogSeverity.Debug, "Discord", ex);

        internal Logger CreateLogger(string name) => new Logger(this, name);
    }
}
