using System;
using System.Threading.Tasks;

namespace Discord.Logging
{
    public interface ILogger
    {
        LogSeverity Level { get; }

        Task LogAsync(LogSeverity severity, string message, Exception exception = null);
        Task LogAsync(LogSeverity severity, FormattableString message, Exception exception = null);
        Task LogAsync(LogSeverity severity, Exception exception);

        Task ErrorAsync(string message, Exception exception = null);
        Task ErrorAsync(FormattableString message, Exception exception = null);
        Task ErrorAsync(Exception exception);

        Task WarningAsync(string message, Exception exception = null);
        Task WarningAsync(FormattableString message, Exception exception = null);
        Task WarningAsync(Exception exception);

        Task InfoAsync(string message, Exception exception = null);
        Task InfoAsync(FormattableString message, Exception exception = null);
        Task InfoAsync(Exception exception);

        Task VerboseAsync(string message, Exception exception = null);
        Task VerboseAsync(FormattableString message, Exception exception = null);
        Task VerboseAsync(Exception exception);

        Task DebugAsync(string message, Exception exception = null);
        Task DebugAsync(FormattableString message, Exception exception = null);
        Task DebugAsync(Exception exception);
    }
}
