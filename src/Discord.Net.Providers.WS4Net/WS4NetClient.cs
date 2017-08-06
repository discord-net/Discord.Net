using Discord.Net.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Utf8;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;
using WS4NetSocket = WebSocket4Net.WebSocket;

namespace Discord.Net.Providers.WS4Net
{
    internal class WS4NetClient : IWebSocketClient, IDisposable
    {
        public event Func<ReadOnlyBuffer<byte>, bool, Task> Message;
        public event Func<Exception, Task> Closed;

        private readonly SemaphoreSlim _lock;
        private readonly Dictionary<string, string> _headers;
        private WS4NetSocket _client;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken, _parentToken;
        private ManualResetEventSlim _waitUntilConnect;
        private bool _isDisposed;

        public WS4NetClient()
        {
            _headers = new Dictionary<string, string>();
            _lock = new SemaphoreSlim(1, 1);
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
            _waitUntilConnect = new ManualResetEventSlim();
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    DisconnectInternalAsync(true).GetAwaiter().GetResult();
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

            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _cancelTokenSource.Token).Token;

            _client = new WS4NetSocket(host, customHeaderItems: _headers.ToList())
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

        public async Task DisconnectAsync()
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }
        private Task DisconnectInternalAsync(bool isDisposing = false)
        {
            _cancelTokenSource.Cancel();
            if (_client == null)
                return Task.Delay(0);

            if (_client.State == WebSocketState.Open)
            {
                try { _client.Close(1000, ""); }
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
            _parentToken = cancelToken;
            _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _cancelTokenSource.Token).Token;
        }

        public async Task SendAsync(ReadOnlyBuffer<byte> data, bool isText)
        {
            await _lock.WaitAsync(_cancelToken).ConfigureAwait(false);
            try
            {
                if (isText)
                    _client.Send(new Utf8String(data.Span).ToString());
                else
                {
                    if (data.DangerousTryGetArray(out var array))
                        _client.Send(array.Array, 0, data.Length);
                    else
                        _client.Send(data.ToArray(), 0, data.Length);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        private void OnTextMessage(object sender, MessageReceivedEventArgs e)
        {
            //TODO: Inefficient, but were dropping this plugin ASAP
            Message(new ReadOnlyBuffer<byte>(Encoding.UTF8.GetBytes(e.Message)), true).GetAwaiter().GetResult();
        }
        private void OnBinaryMessage(object sender, DataReceivedEventArgs e)
        {
            Message(new ReadOnlyBuffer<byte>(e.Data, 0, e.Data.Count()), false).GetAwaiter().GetResult();
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