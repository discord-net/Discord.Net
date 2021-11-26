using Discord.Net.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;
using WS4NetSocket = WebSocket4Net.WebSocket;

namespace Discord.Net.Providers.WS4Net
{
    internal class WS4NetClient : IWebSocketClient, IDisposable
    {
        public event Func<byte[], int, int, Task> BinaryMessage;
        public event Func<string, Task> TextMessage;
        public event Func<Exception, Task> Closed;

        private readonly SemaphoreSlim _lock;
        private readonly Dictionary<string, string> _headers;
        private WS4NetSocket _client;
        private CancellationTokenSource _disconnectCancelTokenSource;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken, _parentToken;
        private ManualResetEventSlim _waitUntilConnect;
        private bool _isDisposed;

        public WS4NetClient()
        {
            _headers = new Dictionary<string, string>();
            _lock = new SemaphoreSlim(1, 1);
            _disconnectCancelTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
            _waitUntilConnect = new ManualResetEventSlim();
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    DisconnectInternalAsync(isDisposing: true).GetAwaiter().GetResult();
                    _lock?.Dispose();
                    _cancelTokenSource?.Dispose();
                }
                _isDisposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        public async Task ConnectAsync(string host)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternalAsync(host).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }
        private async Task ConnectInternalAsync(string host)
        {
            await DisconnectInternalAsync().ConfigureAwait(false);

            _disconnectCancelTokenSource?.Dispose();
            _cancelTokenSource?.Dispose();
            _client?.Dispose();

            _disconnectCancelTokenSource = new CancellationTokenSource();
            _cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _disconnectCancelTokenSource.Token);
            _cancelToken = _cancelTokenSource.Token;

            _client = new WS4NetSocket(host, "", customHeaderItems: _headers.ToList())
            {
                EnableAutoSendPing = false,
                NoDelay = true,
                Proxy = null
            };

            _client.MessageReceived += OnTextMessage;
            _client.DataReceived += OnBinaryMessage;
            _client.Opened += OnConnected;
            _client.Closed += OnClosed;

            _client.Open();
            _waitUntilConnect.Wait(_cancelToken);
        }

        public async Task DisconnectAsync(int closeCode = 1000)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync(closeCode: closeCode).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }
        private Task DisconnectInternalAsync(int closeCode = 1000, bool isDisposing = false)
        {
            _disconnectCancelTokenSource.Cancel();
            if (_client == null)
                return Task.Delay(0);

            if (_client.State == WebSocketState.Open)
            {
                try { _client.Close(closeCode, ""); }
                catch { }
            }

            _client.MessageReceived -= OnTextMessage;
            _client.DataReceived -= OnBinaryMessage;
            _client.Opened -= OnConnected;
            _client.Closed -= OnClosed;

            try { _client.Dispose(); }
            catch { }
            _client = null;

            _waitUntilConnect.Reset();
            return Task.Delay(0);
        }

        public void SetHeader(string key, string value)
        {
            _headers[key] = value;
        }
        public void SetCancelToken(CancellationToken cancelToken)
        {
            _cancelTokenSource?.Dispose();
            _parentToken = cancelToken;
            _cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _disconnectCancelTokenSource.Token);
            _cancelToken = _cancelTokenSource.Token;
        }

        public async Task SendAsync(byte[] data, int index, int count, bool isText)
        {
            await _lock.WaitAsync(_cancelToken).ConfigureAwait(false);
            try
            {
                if (isText)
                    _client.Send(Encoding.UTF8.GetString(data, index, count));
                else
                    _client.Send(data, index, count);
            }
            finally
            {
                _lock.Release();
            }
        }

        private void OnTextMessage(object sender, MessageReceivedEventArgs e)
        {
            TextMessage(e.Message).GetAwaiter().GetResult();
        }
        private void OnBinaryMessage(object sender, DataReceivedEventArgs e)
        {
            BinaryMessage(e.Data, 0, e.Data.Length).GetAwaiter().GetResult();
        }
        private void OnConnected(object sender, object e)
        {
            _waitUntilConnect.Set();
        }
        private void OnClosed(object sender, object e)
        {
            var ex = (e as SuperSocket.ClientEngine.ErrorEventArgs)?.Exception ?? new Exception("Unexpected close");
            Closed(ex).GetAwaiter().GetResult();
        }
    }
}
