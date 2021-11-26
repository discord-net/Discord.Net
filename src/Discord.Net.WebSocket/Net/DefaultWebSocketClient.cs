using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    internal class DefaultWebSocketClient : IWebSocketClient, IDisposable
    {
        public const int ReceiveChunkSize = 16 * 1024; //16KB
        public const int SendChunkSize = 4 * 1024; //4KB
        private const int HR_TIMEOUT = -2147012894;

        public event Func<byte[], int, int, Task> BinaryMessage;
        public event Func<string, Task> TextMessage;
        public event Func<Exception, Task> Closed;

        private readonly SemaphoreSlim _lock;
        private readonly Dictionary<string, string> _headers;
        private readonly IWebProxy _proxy;
        private ClientWebSocket _client;
        private Task _task;
        private CancellationTokenSource _disconnectTokenSource, _cancelTokenSource;
        private CancellationToken _cancelToken, _parentToken;
        private bool _isDisposed, _isDisconnecting;

        public DefaultWebSocketClient(IWebProxy proxy = null)
        {
            _lock = new SemaphoreSlim(1, 1);
            _disconnectTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
            _headers = new Dictionary<string, string>();
            _proxy = proxy;
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    DisconnectInternalAsync(isDisposing: true).GetAwaiter().GetResult();
                    _disconnectTokenSource?.Dispose();
                    _cancelTokenSource?.Dispose();
                    _lock?.Dispose();
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

            _disconnectTokenSource?.Dispose();
            _cancelTokenSource?.Dispose();

            _disconnectTokenSource = new CancellationTokenSource();
            _cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _disconnectTokenSource.Token);
            _cancelToken = _cancelTokenSource.Token;

            _client?.Dispose();
            _client = new ClientWebSocket();
            _client.Options.Proxy = _proxy;
            _client.Options.KeepAliveInterval = TimeSpan.Zero;
            foreach (var header in _headers)
            {
                if (header.Value != null)
                    _client.Options.SetRequestHeader(header.Key, header.Value);
            }

            await _client.ConnectAsync(new Uri(host), _cancelToken).ConfigureAwait(false);
            _task = RunAsync(_cancelToken);
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
        private async Task DisconnectInternalAsync(int closeCode = 1000, bool isDisposing = false)
        {
            _isDisconnecting = true;

            try { _disconnectTokenSource.Cancel(false); }
            catch { }

            if (_client != null)
            {
                if (!isDisposing)
                {
                    var status = (WebSocketCloseStatus)closeCode;
                    try { await _client.CloseOutputAsync(status, "", new CancellationToken()); }
                    catch { }
                }
                try { _client.Dispose(); }
                catch { }

                _client = null;
            }

            try
            {
                await (_task ?? Task.Delay(0)).ConfigureAwait(false);
                _task = null;
            }
            finally { _isDisconnecting = false; }
        }
        private async Task OnClosed(Exception ex)
        {
            if (_isDisconnecting)
                return; //Ignore, this disconnect was requested.

            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync(isDisposing: false);
            }
            finally
            {
                _lock.Release();
            }
            await Closed(ex);
        }

        public void SetHeader(string key, string value)
        {
            _headers[key] = value;
        }
        public void SetCancelToken(CancellationToken cancelToken)
        {
            _cancelTokenSource?.Dispose();

            _parentToken = cancelToken;
            _cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _disconnectTokenSource.Token);
            _cancelToken = _cancelTokenSource.Token;
        }

        public async Task SendAsync(byte[] data, int index, int count, bool isText)
        {
            try
            {
                await _lock.WaitAsync(_cancelToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            try
            {
                if (_client == null) return;

                int frameCount = (int)Math.Ceiling((double)count / SendChunkSize);

                for (int i = 0; i < frameCount; i++, index += SendChunkSize)
                {
                    bool isLast = i == (frameCount - 1);

                    int frameSize;
                    if (isLast)
                        frameSize = count - (i * SendChunkSize);
                    else
                        frameSize = SendChunkSize;

                    var type = isText ? WebSocketMessageType.Text : WebSocketMessageType.Binary;
                    await _client.SendAsync(new ArraySegment<byte>(data, index, count), type, isLast, _cancelToken).ConfigureAwait(false);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task RunAsync(CancellationToken cancelToken)
        {
            var buffer = new ArraySegment<byte>(new byte[ReceiveChunkSize]);

            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    WebSocketReceiveResult socketResult = await _client.ReceiveAsync(buffer, cancelToken).ConfigureAwait(false);
                    byte[] result;
                    int resultCount;

                    if (socketResult.MessageType == WebSocketMessageType.Close)
                        throw new WebSocketClosedException((int)socketResult.CloseStatus, socketResult.CloseStatusDescription);

                    if (!socketResult.EndOfMessage)
                    {
                        //This is a large message (likely just READY), lets create a temporary expandable stream
                        using (var stream = new MemoryStream())
                        {
                            stream.Write(buffer.Array, 0, socketResult.Count);
                            do
                            {
                                if (cancelToken.IsCancellationRequested) return;
                                socketResult = await _client.ReceiveAsync(buffer, cancelToken).ConfigureAwait(false);
                                stream.Write(buffer.Array, 0, socketResult.Count);
                            }
                            while (socketResult == null || !socketResult.EndOfMessage);

                            //Use the internal buffer if we can get it
                            resultCount = (int)stream.Length;

                            result = stream.TryGetBuffer(out var streamBuffer) ? streamBuffer.Array : stream.ToArray();

                        }
                    }
                    else
                    {
                        //Small message
                        resultCount = socketResult.Count;
                        result = buffer.Array;
                    }

                    if (socketResult.MessageType == WebSocketMessageType.Text)
                    {
                        string text = Encoding.UTF8.GetString(result, 0, resultCount);
                        await TextMessage(text).ConfigureAwait(false);
                    }
                    else
                        await BinaryMessage(result, 0, resultCount).ConfigureAwait(false);
                }
            }
            catch (Win32Exception ex) when (ex.HResult == HR_TIMEOUT)
            {
                var _ = OnClosed(new Exception("Connection timed out.", ex));
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                //This cannot be awaited otherwise we'll deadlock when DiscordApiClient waits for this task to complete.
                var _ = OnClosed(ex);
            }
        }
    }
}
