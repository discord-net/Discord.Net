using System;

namespace Discord.Net.WebSockets
{
    public class WebSocket : IDisposable
    {        
        protected readonly IWebSocketEngine _engine;
        protected bool _isDisposed;

        internal WebSocket(IWebSocketEngine engine)
        {
            _engine = engine;
        }        

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    _engine.Dispose();

                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);
    }
}
