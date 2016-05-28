using System;
using System.Threading.Tasks;

namespace Discord.Logging
{
    public interface ILogger
    {
        LogSeverity Level { get; }

        Task Log(LogSeverity severity, string message, Exception exception = null);
        Task Log(LogSeverity severity, FormattableString message, Exception exception = null);
        Task Log(LogSeverity severity, Exception exception);

        Task Error(string message, Exception exception = null);
        Task Error(FormattableString message, Exception exception = null);
        Task Error(Exception exception);

        Task Warning(string message, Exception exception = null);
        Task Warning(FormattableString message, Exception exception = null);
        Task Warning(Exception exception);

        Task Info(string message, Exception exception = null);
        Task Info(FormattableString message, Exception exception = null);
        Task Info(Exception exception);

        Task Verbose(string message, Exception exception = null);
        Task Verbose(FormattableString message, Exception exception = null);
        Task Verbose(Exception exception);

        Task Debug(string message, Exception exception = null);
        Task Debug(FormattableString message, Exception exception = null);
        Task Debug(Exception exception);
    }
}
