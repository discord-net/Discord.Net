using Discord.Extensions;
using System;
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
        
        private readonly ClientWebSocket _client;
        private Task _task;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken, _parentToken;
        private bool _isDisposed;

        public DefaultWebSocketClient()
        {
            _client = new ClientWebSocket();
            _client.Options.Proxy = null;
            _client.Options.KeepAliveInterval = TimeSpan.Zero;

            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
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

        public async Task Connect(string host)
        {
            //Assume locked
            await Disconnect().ConfigureAwait(false);

            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _cancelTokenSource.Token).Token;

            await _client.ConnectAsync(new Uri(host), _cancelToken).ConfigureAwait(false);
            _task = Run(_cancelToken);
        }
        public async Task Disconnect()
        {
            //Assume locked
            _cancelTokenSource.Cancel();

            if (_client.State == WebSocketState.Open)
                try { await _client?.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None); } catch { }
            
            await (_task ?? Task.CompletedTask).ConfigureAwait(false);
        }

        public void SetHeader(string key, string value)
        {
            _client.Options.SetRequestHeader(key, value);
        }
        public void SetCancelToken(CancellationToken cancelToken)
        {
            _parentToken = cancelToken;
            _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _cancelTokenSource.Token).Token;
        }

        public async Task Send(byte[] data, int index, int count, bool isText)
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
                    await _client.SendAsync(new ArraySegment<byte>(data, index, count), isText ? WebSocketMessageType.Text : WebSocketMessageType.Binary, isLast, _cancelToken).ConfigureAwait(false);
                }
                catch (Win32Exception ex) when (ex.HResult == HR_TIMEOUT)
                {
                    return;
                }
            }
        }

        //TODO: Check this code
        private async Task Run(CancellationToken cancelToken)
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

                        if (result.MessageType == WebSocketMessageType.Close)
                            throw new WebSocketException((int)result.CloseStatus.Value, result.CloseStatusDescription);
                        else
                            stream.Write(buffer.Array, 0, result.Count);

                    }
                    while (result == null || !result.EndOfMessage);

                    var array = stream.ToArray();
                    if (result.MessageType == WebSocketMessageType.Binary)
                        await BinaryMessage.Raise(array, 0, array.Length).ConfigureAwait(false);
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string text = Encoding.UTF8.GetString(array, 0, array.Length);
                        await TextMessage.Raise(text).ConfigureAwait(false);
                    }

                    stream.Position = 0;
                    stream.SetLength(0);
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
