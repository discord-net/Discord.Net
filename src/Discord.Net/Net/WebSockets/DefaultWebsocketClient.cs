using Discord.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public class DefaultWebSocketClient : IWebSocketClient
    {
        public const int ReceiveChunkSize = 12 * 1024; //12KB
        public const int SendChunkSize = 4 * 1024; //4KB
        private const int HR_TIMEOUT = -2147012894;

        public event Func<byte[], int, int, Task> BinaryMessage;
        public event Func<string, Task> TextMessage;
        public event Func<Exception, Task> Closed;

        private readonly SemaphoreSlim _sendLock;
        private readonly Dictionary<string, string> _headers;
        private ClientWebSocket _client;
        private Task _task;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken, _parentToken;
        private bool _isDisposed;

        public DefaultWebSocketClient()
        {
            _sendLock = new SemaphoreSlim(1, 1);
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
                    _client.Dispose();
                _isDisposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        public async Task ConnectAsync(string host)
        {
            //Assume locked
            await DisconnectAsync().ConfigureAwait(false);

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
            //Assume locked
            _cancelTokenSource.Cancel();
            
            if (_client != null && _client.State == WebSocketState.Open)
            {
                try
                {
                    var task = _client?.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    if (task != null)
                        await task.ConfigureAwait(false);
                }
                catch { }
            }
            
            await (_task ?? Task.CompletedTask).ConfigureAwait(false);
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
            await _sendLock.WaitAsync(_cancelToken).ConfigureAwait(false);
            try
            {
                //TODO: If connection is temporarily down, retry?
                int frameCount = (int)Math.Ceiling((double)count / SendChunkSize);

                for (int i = 0; i < frameCount; i++, index += SendChunkSize)
                {
                    bool isLast = i == (frameCount - 1);

                    int frameSize;
                    if (isLast)
                        frameSize = count - (i * SendChunkSize);
                    else
                        frameSize = SendChunkSize;

                    try
                    {
                        var type = isText ? WebSocketMessageType.Text : WebSocketMessageType.Binary;
                        await _client.SendAsync(new ArraySegment<byte>(data, index, count), type, isLast, _cancelToken).ConfigureAwait(false);
                    }
                    catch (Win32Exception ex) when (ex.HResult == HR_TIMEOUT)
                    {
                        return;
                    }
                }
            }
            finally
            {
                _sendLock.Release();
            }
        }
        
        private async Task RunAsync(CancellationToken cancelToken)
        {
            var buffer = new ArraySegment<byte>(new byte[ReceiveChunkSize]);
            var stream = new MemoryStream();

            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    WebSocketReceiveResult result = null;
                    do
                    {
                        if (cancelToken.IsCancellationRequested) return;

                        try
                        {
                            result = await _client.ReceiveAsync(buffer, cancelToken).ConfigureAwait(false);
                        }
                        catch (Win32Exception ex) when (ex.HResult == HR_TIMEOUT)
                        {
                            throw new Exception("Connection timed out.");
                        }

                        if (result.Count > 0)
                            stream.Write(buffer.Array, 0, result.Count);
                    }
                    while (result == null || !result.EndOfMessage);

                    var array = stream.ToArray();
                    stream.Position = 0;
                    stream.SetLength(0);

                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Binary:
                            await BinaryMessage(array, 0, array.Length).ConfigureAwait(false);
                            break;
                        case WebSocketMessageType.Text:
                            string text = Encoding.UTF8.GetString(array, 0, array.Length);
                            await TextMessage(text).ConfigureAwait(false);
                            break;
                        case WebSocketMessageType.Close:
                            var _ = Closed(new WebSocketClosedException((int)result.CloseStatus, result.CloseStatusDescription));
                            return;
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                //This cannot be awaited otherwise we'll deadlock when DiscordApiClient waits for this task to complete.
                var _ = Closed(ex);
            }
        }
    }
}
