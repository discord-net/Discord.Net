using Microsoft.Extensions.Logging;
using System;

namespace Discord.Logging
{
    internal class DefaultLogger : ILogger
    {
        private static readonly object _lock = new object();

        private LogLevel MinimumLevel { get; }

        internal DefaultLogger(LogLevel minLevel = LogLevel.Information)
        {
            this.MinimumLevel = minLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
                return;

            lock (_lock)
            {
                Console.Write($"{DateTime.Now} ");
                
                Console.Write(logLevel switch 
                {
                    LogLevel.Trace =>       "[Trace] ",
                    LogLevel.Debug =>       "[Debug] ",
                    LogLevel.Information => "[Info ] ",
                    LogLevel.Warning =>     "[Warn ] ",
                    LogLevel.Error =>       "[Error] ",
                    LogLevel.Critical =>    "[Crit ] ",
                    LogLevel.None =>        "[None ] ",
                    _ =>                    "[?????] "
                });

                var message = formatter(state, exception);
                Console.WriteLine(message);
                if (exception != null)
                    Console.WriteLine(exception);
            }
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel >= this.MinimumLevel;

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
