using Microsoft.Extensions.Logging;
using System;

namespace Discord.Logging
{
    internal class DefaultLoggerProvider : ILoggerProvider
    {
        private LogLevel MinimumLevel { get; }

        private bool _isDisposed = false;
        

        internal DefaultLoggerProvider(LogLevel minLevel = LogLevel.Information)
        {
            this.MinimumLevel = minLevel;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (this._isDisposed)
                throw new InvalidOperationException("This logger provider is already disposed.");

            return new DefaultLogger(this.MinimumLevel);
        }

        public void Dispose()
        {
            this._isDisposed = true;
        }
    }
}
