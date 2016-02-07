using System;

namespace Discord.Logging
{
    public interface ILogger
    {
        LogSeverity Level { get; }

        void Log(LogSeverity severity, string message, Exception exception = null);
#if !NET45
        void Log(LogSeverity severity, FormattableString message, Exception exception = null);
#endif

        void Error(string message, Exception exception = null);
#if !NET45
        void Error(FormattableString message, Exception exception = null);
#endif
        void Error(Exception exception);

        void Warning(string message, Exception exception =  null);
#if !NET45
        void Warning(FormattableString message, Exception exception = null);
#endif
        void Warning(Exception exception);

        void Info(string message, Exception exception = null);
#if !NET45
        void Info(FormattableString message, Exception exception = null);
#endif
        void Info(Exception exception);

        void Verbose(string message, Exception exception = null);
#if !NET45
        void Verbose(FormattableString message, Exception exception = null);
#endif
        void Verbose(Exception exception);

        void Debug(string message, Exception exception = null);
#if !NET45
        void Debug(FormattableString message, Exception exception = null);
#endif
        void Debug(Exception exception);
    }
}
