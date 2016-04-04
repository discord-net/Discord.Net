using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public class DefaultWebSocketEngine : IWebSocketEngine
    {
        public const int ReceiveChunkSize = 12 * 1024; //12KB
        public const int SendChunkSize = 4 * 1024; //4KB
        protected const int HR_TIMEOUT = -2147012894;

        public event EventHandler<BinaryMessageEventArgs> BinaryMessage = delegate { };
        public event EventHandler<TextMessageEventArgs> TextMessage = delegate { };

        protected readonly ConcurrentQueue<string> _sendQueue;
        protected readonly ClientWebSocket _client;
        protected Task _receiveTask, _sendTask;
        protected CancellationTokenSource _cancelToken;
        protected bool _isDisposed;

        public DefaultWebSocketEngine()
        {
            _sendQueue = new ConcurrentQueue<string>();

            _client = new ClientWebSocket();
            _client.Options.Proxy = null;
            _client.Options.KeepAliveInterval = TimeSpan.Zero;
        }
        protected virtual void Dispose(bool disposing)
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

        public async Task Connect(string host, CancellationToken cancelToken)
        {
            await Disconnect().ConfigureAwait(false);

            _cancelToken = new CancellationTokenSource();
            var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelToken.Token, cancelToken).Token;

            await _client.ConnectAsync(new Uri(host), combinedToken).ConfigureAwait(false);
            _receiveTask = TaskHelper.CreateLongRunning(() => ReceiveAsync(combinedToken), combinedToken);
            _sendTask = TaskHelper.CreateLongRunning(() => SendAsync(combinedToken), combinedToken);
        }
        public async Task Disconnect()
        {
            _cancelToken.Cancel();

            string ignored;
            while (_sendQueue.TryDequeue(out ignored)) { }

            _client.Abort();

            var receiveTask = _receiveTask ?? TaskHelper.CompletedTask;
            var sendTask = _sendTask ?? TaskHelper.CompletedTask;
            await Task.WhenAll(receiveTask, sendTask).ConfigureAwait(false);
        }

        public void SetHeader(string key, string value)
        {
            _client.Options.SetRequestHeader(key, value);
        }

        public void QueueMessage(string message)
        {
            _sendQueue.Enqueue(message);
        }

        //TODO: Check this code
        private Task ReceiveAsync(CancellationToken cancelToken)
        {
            return Task.Run(async () =>
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
                                throw new Exception($"Connection timed out.");
                            }

                            if (result.MessageType == WebSocketMessageType.Close)
                                throw new WebSocketException((int)result.CloseStatus.Value, result.CloseStatusDescription);
                            else
                                stream.Write(buffer.Array, buffer.Offset, buffer.Count);

                        }
                        while (result == null || !result.EndOfMessage);

                        var array = stream.ToArray();
                        if (result.MessageType == WebSocketMessageType.Binary)
                            BinaryMessage(this, new BinaryMessageEventArgs(array));
                        else if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string text = Encoding.UTF8.GetString(array, 0, array.Length);
                            TextMessage(this, new TextMessageEventArgs(text));
                        }

                        stream.Position = 0;
                        stream.SetLength(0);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }

        //TODO: Check this code
        private Task SendAsync(CancellationToken cancelToken)
        {
            return Task.Run(async () =>
            {
                byte[] bytes = new byte[SendChunkSize];

                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        string json;
                        while (_sendQueue.TryDequeue(out json))
                        {
                            int byteCount = Encoding.UTF8.GetBytes(json, 0, json.Length, bytes, 0);
                            int frameCount = (int)Math.Ceiling((double)byteCount / SendChunkSize);

                            int offset = 0;
                            for (int i = 0; i < frameCount; i++, offset += SendChunkSize)
                            {
                                bool isLast = i == (frameCount - 1);

                                int count;
                                if (isLast)
                                    count = byteCount - (i * SendChunkSize);
                                else
                                    count = SendChunkSize;

                                try
                                {
                                    await _client.SendAsync(new ArraySegment<byte>(bytes, offset, count), WebSocketMessageType.Text, isLast, cancelToken).ConfigureAwait(false);
                                }
                                catch (Win32Exception ex) when (ex.HResult == HR_TIMEOUT)
                                {
                                    return;
                                }
                            }
                        }
                        await Task.Delay(DiscordConfig.WebSocketQueueInterval, cancelToken).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }
    }
}
