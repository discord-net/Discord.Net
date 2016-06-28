using System;
using System.Threading.Tasks;

namespace Discord.Logging
{
    internal class LogManager : ILogManager, ILogger
    {
        public LogSeverity Level { get; }

        public event Func<LogMessage, Task> Message { add { _messageEvent.Add(value); } remove { _messageEvent.Remove(value); } }
        private readonly AsyncEvent<Func<LogMessage, Task>> _messageEvent = new AsyncEvent<Func<LogMessage, Task>>();

        public LogManager(LogSeverity minSeverity)
        {
            Level = minSeverity;
        }

        public async Task LogAsync(LogSeverity severity, string source, string message, Exception ex = null)
        {
            if (severity <= Level)
                await _messageEvent.InvokeAsync(new LogMessage(severity, source, message, ex)).ConfigureAwait(false);
        }
        public async Task LogAsync(LogSeverity severity, string source, FormattableString message, Exception ex = null)
        {
            if (severity <= Level)
                await _messageEvent.InvokeAsync(new LogMessage(severity, source, message.ToString(), ex)).ConfigureAwait(false);
        }
        public async Task LogAsync(LogSeverity severity, string source, Exception ex)
        {
            if (severity <= Level)
                await _messageEvent.InvokeAsync(new LogMessage(severity, source, null, ex)).ConfigureAwait(false);
        }
        async Task ILogger.LogAsync(LogSeverity severity, string message, Exception ex)
        {
            if (severity <= Level)
                await _messageEvent.InvokeAsync(new LogMessage(severity, "Discord", message, ex)).ConfigureAwait(false);
        }
        async Task ILogger.LogAsync(LogSeverity severity, FormattableString message, Exception ex)
        {
            if (severity <= Level)
                await _messageEvent.InvokeAsync(new LogMessage(severity, "Discord", message.ToString(), ex)).ConfigureAwait(false);
        }
        async Task ILogger.LogAsync(LogSeverity severity, Exception ex)
        {
            if (severity <= Level)
                await _messageEvent.InvokeAsync(new LogMessage(severity, "Discord", null, ex)).ConfigureAwait(false);
        }

        public Task ErrorAsync(string source, string message, Exception ex = null)
            => LogAsync(LogSeverity.Error, source, message, ex);
        public Task ErrorAsync(string source, FormattableString message, Exception ex = null)
            => LogAsync(LogSeverity.Error, source, message, ex);
        public Task ErrorAsync(string source, Exception ex)
            => LogAsync(LogSeverity.Error, source, ex);
        Task ILogger.ErrorAsync(string message, Exception ex)
            => LogAsync(LogSeverity.Error, "Discord", message, ex);
        Task ILogger.ErrorAsync(FormattableString message, Exception ex)
            => LogAsync(LogSeverity.Error, "Discord", message, ex);
        Task ILogger.ErrorAsync(Exception ex)
            => LogAsync(LogSeverity.Error, "Discord", ex);

        public Task WarningAsync(string source, string message, Exception ex = null)
            => LogAsync(LogSeverity.Warning, source, message, ex);
        public Task WarningAsync(string source, FormattableString message, Exception ex = null)
            => LogAsync(LogSeverity.Warning, source, message, ex);
        public Task WarningAsync(string source, Exception ex)
            => LogAsync(LogSeverity.Warning, source, ex);
        Task ILogger.WarningAsync(string message, Exception ex)
            => LogAsync(LogSeverity.Warning, "Discord", message, ex);
        Task ILogger.WarningAsync(FormattableString message, Exception ex)
            => LogAsync(LogSeverity.Warning, "Discord", message, ex);
        Task ILogger.WarningAsync(Exception ex)
            => LogAsync(LogSeverity.Warning, "Discord", ex);

        public Task InfoAsync(string source, string message, Exception ex = null)
            => LogAsync(LogSeverity.Info, source, message, ex);
        public Task InfoAsync(string source, FormattableString message, Exception ex = null)
            => LogAsync(LogSeverity.Info, source, message, ex);
        public Task InfoAsync(string source, Exception ex)
            => LogAsync(LogSeverity.Info, source, ex);
        Task ILogger.InfoAsync(string message, Exception ex)
            => LogAsync(LogSeverity.Info, "Discord", message, ex);
        Task ILogger.InfoAsync(FormattableString message, Exception ex)
            => LogAsync(LogSeverity.Info, "Discord", message, ex);
        Task ILogger.InfoAsync(Exception ex)
            => LogAsync(LogSeverity.Info, "Discord", ex);

        public Task VerboseAsync(string source, string message, Exception ex = null)
            => LogAsync(LogSeverity.Verbose, source, message, ex);
        public Task VerboseAsync(string source, FormattableString message, Exception ex = null)
            => LogAsync(LogSeverity.Verbose, source, message, ex);
        public Task VerboseAsync(string source, Exception ex)
            => LogAsync(LogSeverity.Verbose, source, ex);
        Task ILogger.VerboseAsync(string message, Exception ex)
            => LogAsync(LogSeverity.Verbose, "Discord", message, ex);
        Task ILogger.VerboseAsync(FormattableString message, Exception ex)
            => LogAsync(LogSeverity.Verbose, "Discord", message, ex);
        Task ILogger.VerboseAsync(Exception ex)
            => LogAsync(LogSeverity.Verbose, "Discord", ex);

        public Task DebugAsync(string source, string message, Exception ex = null)
            => LogAsync(LogSeverity.Debug, source, message, ex);
        public Task DebugAsync(string source, FormattableString message, Exception ex = null)
            => LogAsync(LogSeverity.Debug, source, message, ex);
        public Task DebugAsync(string source, Exception ex)
            => LogAsync(LogSeverity.Debug, source, ex);
        Task ILogger.DebugAsync(string message, Exception ex)
            => LogAsync(LogSeverity.Debug, "Discord", message, ex);
        Task ILogger.DebugAsync(FormattableString message, Exception ex)
            => LogAsync(LogSeverity.Debug, "Discord", message, ex);
        Task ILogger.DebugAsync(Exception ex)
            => LogAsync(LogSeverity.Debug, "Discord", ex);

        public ILogger CreateLogger(string name) => new Logger(this, name);
    }
}
