using System;
using System.Threading.Tasks;

namespace Discord.Logging
{
    public interface ILogManager
    {
        LogSeverity Level { get; }

        Task LogAsync(LogSeverity severity, string source, string message, Exception ex = null);
        Task LogAsync(LogSeverity severity, string source, FormattableString message, Exception ex = null);
        Task LogAsync(LogSeverity severity, string source, Exception ex);

        Task ErrorAsync(string source, string message, Exception ex = null);
        Task ErrorAsync(string source, FormattableString message, Exception ex = null);
        Task ErrorAsync(string source, Exception ex);

        Task WarningAsync(string source, string message, Exception ex = null);
        Task WarningAsync(string source, FormattableString message, Exception ex = null);
        Task WarningAsync(string source, Exception ex);

        Task InfoAsync(string source, string message, Exception ex = null);
        Task InfoAsync(string source, FormattableString message, Exception ex = null);
        Task InfoAsync(string source, Exception ex);

        Task VerboseAsync(string source, string message, Exception ex = null);
        Task VerboseAsync(string source, FormattableString message, Exception ex = null);
        Task VerboseAsync(string source, Exception ex);

        Task DebugAsync(string source, string message, Exception ex = null);
        Task DebugAsync(string source, FormattableString message, Exception ex = null);
        Task DebugAsync(string source, Exception ex);

        ILogger CreateLogger(string name);
    }
}
