using Discord.Net.WebSockets;
using System;

namespace Discord.Logging
{
    public class Logger
    {
        private readonly LogManager _manager;

        public string Name { get; }
        public LogSeverity Level => _manager.Level;

        internal Logger(LogManager manager, string name)
        {
            _manager = manager;
            Name = name;
        }

        public void Log(LogSeverity severity, string message, Exception exception = null)
            => _manager.Log(severity, Name, message, exception);
        public void Log(LogSeverity severity, FormattableString message, Exception exception = null)
            => _manager.Log(severity, Name, message, exception);

        public void Error(string message, Exception exception = null)
            => _manager.Error(Name, message, exception);
        public void Error(FormattableString message, Exception exception = null)
            => _manager.Error(Name, message, exception);
        public void Warning(string message, Exception exception = null)
            => _manager.Warning(Name, message, exception);
        public void Warning(FormattableString message, Exception exception = null)
            => _manager.Warning(Name, message, exception);
        public void Info(string message, Exception exception = null)
            => _manager.Info(Name, message, exception);
        public void Info(FormattableString message, Exception exception = null)
            => _manager.Info(Name, message, exception);
        public void Verbose(string message, Exception exception = null)
            => _manager.Verbose(Name, message, exception);
        public void Verbose(FormattableString message, Exception exception = null)
            => _manager.Verbose(Name, message, exception);
        public void Debug(string message, Exception exception = null)
            => _manager.Debug(Name, message, exception);
        public void Debug(FormattableString message, Exception exception = null)
            => _manager.Debug(Name, message, exception);
    }
}
