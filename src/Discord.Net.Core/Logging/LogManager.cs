using System;
using System.Threading.Tasks;

namespace Discord.Logging
{
    internal class LogManager
    {
        public LogSeverity Level { get; }
        private Logger ClientLogger { get; }

        public event Func<LogMessage, Task> Message { add { _messageEvent.Add(value); } remove { _messageEvent.Remove(value); } }
        private readonly AsyncEvent<Func<LogMessage, Task>> _messageEvent = new AsyncEvent<Func<LogMessage, Task>>();

        public LogManager(LogSeverity minSeverity)
        {
            Level = minSeverity;
            ClientLogger = new Logger(this, "Discord");
        }

        public async Task LogAsync(LogSeverity severity, string source, Exception ex)
        {
            try
            {
                if (severity <= Level)
                    await _messageEvent.InvokeAsync(new LogMessage(severity, source, null, ex)).ConfigureAwait(false);
            }
            catch { }
        }
        public async Task LogAsync(LogSeverity severity, string source, string message, Exception ex = null)
        {
            try
            {
                if (severity <= Level)
                await _messageEvent.InvokeAsync(new LogMessage(severity, source, message, ex)).ConfigureAwait(false);
            }
            catch { }
        }
#if NETSTANDARD1_3
        public async Task LogAsync(LogSeverity severity, string source, FormattableString message, Exception ex = null)
        {
            try
            {
                if (severity <= Level)
                    await _messageEvent.InvokeAsync(new LogMessage(severity, source, message.ToString(), ex)).ConfigureAwait(false);
            }
            catch { }
        }
#endif

        public Task ErrorAsync(string source, Exception ex)
            => LogAsync(LogSeverity.Error, source, ex);
        public Task ErrorAsync(string source, string message, Exception ex = null)
            => LogAsync(LogSeverity.Error, source, message, ex);
#if NETSTANDARD1_3
        public Task ErrorAsync(string source, FormattableString message, Exception ex = null)
            => LogAsync(LogSeverity.Error, source, message, ex);
#endif

        public Task WarningAsync(string source, Exception ex)
            => LogAsync(LogSeverity.Warning, source, ex);
        public Task WarningAsync(string source, string message, Exception ex = null)
            => LogAsync(LogSeverity.Warning, source, message, ex);
#if NETSTANDARD1_3
        public Task WarningAsync(string source, FormattableString message, Exception ex = null)
            => LogAsync(LogSeverity.Warning, source, message, ex);
#endif

        public Task InfoAsync(string source, Exception ex)
            => LogAsync(LogSeverity.Info, source, ex);
        public Task InfoAsync(string source, string message, Exception ex = null)
            => LogAsync(LogSeverity.Info, source, message, ex);
#if NETSTANDARD1_3
        public Task InfoAsync(string source, FormattableString message, Exception ex = null)
            => LogAsync(LogSeverity.Info, source, message, ex);
#endif

        public Task VerboseAsync(string source, Exception ex)
            => LogAsync(LogSeverity.Verbose, source, ex);
        public Task VerboseAsync(string source, string message, Exception ex = null)
            => LogAsync(LogSeverity.Verbose, source, message, ex);
#if NETSTANDARD1_3
        public Task VerboseAsync(string source, FormattableString message, Exception ex = null)
            => LogAsync(LogSeverity.Verbose, source, message, ex);
#endif

        public Task DebugAsync(string source, Exception ex)
            => LogAsync(LogSeverity.Debug, source, ex);
        public Task DebugAsync(string source, string message, Exception ex = null)
            => LogAsync(LogSeverity.Debug, source, message, ex);
#if NETSTANDARD1_3
        public Task DebugAsync(string source, FormattableString message, Exception ex = null)
            => LogAsync(LogSeverity.Debug, source, message, ex);
#endif

        public Logger CreateLogger(string name) => new Logger(this, name);

        public async Task WriteInitialLog()
        {
            await ClientLogger.InfoAsync($"Discord.Net v{DiscordConfig.Version} (API v{DiscordConfig.APIVersion})").ConfigureAwait(false);
        }
    }
}
