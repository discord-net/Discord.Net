using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Logging
{
    internal class DefaultLoggerFactory : ILoggerFactory
    {
        private List<ILoggerProvider> Providers { get; } = new List<ILoggerProvider>();
        private bool _isDisposed = false;

        public void Dispose()
        {
            if (this._isDisposed)
                return;
            this._isDisposed = true;

            foreach (var provider in this.Providers)
                provider.Dispose();

            this.Providers.Clear();
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (this._isDisposed)
                throw new InvalidOperationException("This logger factory is already disposed.");

            // HEHEHE XDXD
            var provider = Providers.FirstOrDefault();

            return provider?.CreateLogger(categoryName) ?? throw new ArgumentNullException(nameof(provider));
        }

        public void AddProvider(ILoggerProvider provider)
        {
            this.Providers.Add(provider);
        }
    }
}
