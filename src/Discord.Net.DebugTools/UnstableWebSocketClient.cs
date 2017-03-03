using Discord.Net.WebSockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Providers.UnstableWebSocket
{
    internal class UnstableWebSocketClient : IWebSocketClient, IDisposable
    {
        public const int ReceiveChunkSize = 16 * 1024; //16KB
        public const int SendChunkSize = 4 * 1024; //4KB
        private const int HR_TIMEOUT = -2147012894;
        private const double FailureRate = 0.10; //10%

        public event Func<byte[], int, int, Task> BinaryMessage;
        public event Func<string, Task> TextMessage;
        public event Func<Exception, Task> Closed;

        private readonly SemaphoreSlim _lock;
        private readonly Dictionary<string, string> _headers;
        private readonly Random _rand;
        private ClientWebSocket _client;
        private Task _task;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken, _parentToken;
        private bool _isDisposed, _isDisconnecting;

        public UnstableWebSocketClient()
        {
            _lock = new SemaphoreSlim(1, 1);
            _rand = new Random();
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
            _headers = new Dictionary<string, string>();
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

            _client = new ClientWebSocket();
            _client.Options.Proxy = null;
            _client.Options.KeepAliveInterval = TimeSpan.Zero;
            foreach (var header in _headers)
            {
                if (header.Value != null)
                    _client.Options.SetRequestHeader(header.Key, header.Value);
            }

            await _client.ConnectAsync(new Uri(host), _cancelToken).ConfigureAwait(false);
            _task = RunAsync(_cancelToken);
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
        private async Task DisconnectInternalAsync(bool isDisposing = false)
        {
            try { _cancelTokenSource.Cancel(false); } catch { }

            _isDisconnecting = true;
            try
            {
                await (_task ?? Task.Delay(0)).ConfigureAwait(false);
                _task = null;
            }
            finally { _isDisconnecting = false; }

            if (_client != null)
            {
                if (!isDisposing)
                {
                    try { await _client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", new CancellationToken()); }
                    catch { }
                }
                try { _client.Dispose(); }
                catch { }
                
                _client = null;
            }
        }
        private async Task OnClosed(Exception ex)
        {
            if (_isDisconnecting)
                return; //Ignore, this disconnect was requested.

            System.Diagnostics.Debug.WriteLine("OnClosed - " + ex.Message);
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync(false);
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
            _parentToken = cancelToken;
            _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _cancelTokenSource.Token).Token;
        }

        public async Task SendAsync(byte[] data, int index, int count, bool isText)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!UnstableCheck())
                    return;

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
                            ArraySegment<byte> streamBuffer;
                            if (stream.TryGetBuffer(out streamBuffer))
                                result = streamBuffer.Array;
                            else
                                result = stream.ToArray();
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

        private bool UnstableCheck()
        {
            return _rand.NextDouble() > FailureRate;
        }
    }
}