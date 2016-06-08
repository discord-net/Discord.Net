using System;
using System.Threading.Tasks;

namespace Discord.Logging
{
    internal class Logger
    {
        private readonly LogManager _manager;

        public string Name { get; }
        public LogSeverity Level => _manager.Level;

        public Logger(LogManager manager, string name)
        {
            _manager = manager;
            Name = name;
        }

        public Task Log(LogSeverity severity, string message, Exception exception = null)
            => _manager.Log(severity, Name, message, exception);
        public Task Log(LogSeverity severity, FormattableString message, Exception exception = null)
            => _manager.Log(severity, Name, message, exception);

        public Task Error(string message, Exception exception = null)
            => _manager.Error(Name, message, exception);
        public Task Error(FormattableString message, Exception exception = null)
            => _manager.Error(Name, message, exception);
        public Task Error(Exception exception)
            => _manager.Error(Name, exception);

        public Task Warning(string message, Exception exception = null)
            => _manager.Warning(Name, message, exception);
        public Task Warning(FormattableString message, Exception exception = null)
            => _manager.Warning(Name, message, exception);
        public Task Warning(Exception exception)
            => _manager.Warning(Name, exception);

        public Task Info(string message, Exception exception = null)
            => _manager.Info(Name, message, exception);
        public Task Info(FormattableString message, Exception exception = null)
            => _manager.Info(Name, message, exception);
        public Task Info(Exception exception)
            => _manager.Info(Name, exception);

        public Task Verbose(string message, Exception exception = null)
            => _manager.Verbose(Name, message, exception);
        public Task Verbose(FormattableString message, Exception exception = null)
            => _manager.Verbose(Name, message, exception);
        public Task Verbose(Exception exception)
            => _manager.Verbose(Name, exception);

        public Task Debug(string message, Exception exception = null)
            => _manager.Debug(Name, message, exception);
        public Task Debug(FormattableString message, Exception exception = null)
            => _manager.Debug(Name, message, exception);
        public Task Debug(Exception exception)
            => _manager.Debug(Name, exception);
    }
}
