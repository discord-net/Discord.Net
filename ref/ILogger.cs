using System;

namespace Discord.Logging
{
    public interface ILogger
    {
        LogSeverity Level { get; }

        void Log(LogSeverity severity, string message, Exception exception = null);
        void Error(string message, Exception exception = null);
        void Error(Exception exception);
        void Warning(string message, Exception exception = null);
        void Warning(Exception exception);
        void Info(string message, Exception exception = null);
        void Info(Exception exception);
        void Verbose(string message, Exception exception = null);
        void Verbose(Exception exception);
        void Debug(string message, Exception exception = null);
        void Debug(Exception exception);

#if DOTNET5_4
        void Log(LogSeverity severity, FormattableString message, Exception exception = null);
        void Error(FormattableString message, Exception exception = null);
        void Warning(FormattableString message, Exception exception = null);
        void Info(FormattableString message, Exception exception = null);
        void Verbose(FormattableString message, Exception exception = null);
        void Debug(FormattableString message, Exception exception = null);
#endif
    }
}
