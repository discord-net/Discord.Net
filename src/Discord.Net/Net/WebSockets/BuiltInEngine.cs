#if NETSTANDARD1_3
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketClient = System.Net.WebSockets.ClientWebSocket;

namespace Discord.Net.WebSockets
{
    internal class BuiltInEngine : IWebSocketEngine
    {
        private const int ReceiveChunkSize = 12 * 1024; //12KB
        private const int SendChunkSize = 4 * 1024; //4KB
        private const int HR_TIMEOUT = -2147012894;

        private readonly DiscordConfig _config;
        private readonly ConcurrentQueue<string> _sendQueue;
        private WebSocketClient _webSocket;
        private Task _tempTask;

        public event EventHandler<BinaryMessageEventArgs> BinaryMessage = delegate { };
        public event EventHandler<TextMessageEventArgs> TextMessage = delegate { };
        private void OnBinaryMessage(byte[] data)
            => BinaryMessage(this, new BinaryMessageEventArgs(data));
        private void OnTextMessage(string msg)
            => TextMessage(this, new TextMessageEventArgs(msg));

        internal BuiltInEngine(DiscordConfig config)
        {
            _config = config;
            _sendQueue = new ConcurrentQueue<string>();
        }

        public async Task Connect(string host, CancellationToken cancelToken)
        {
            _webSocket = new WebSocketClient();
            _webSocket.Options.Proxy = null;
            _webSocket.Options.SetRequestHeader("User-Agent", _config.UserAgent);
            _webSocket.Options.KeepAliveInterval = TimeSpan.Zero;
            _tempTask = await _webSocket.ConnectAsync(new Uri(host), cancelToken)//.ConfigureAwait(false);
                .ContinueWith(t => ReceiveAsync(cancelToken)).ConfigureAwait(false); 
            //TODO: ContinueWith is a temporary hack, may be a bug related to https://github.com/dotnet/corefx/issues/4429
        }

        public Task Disconnect()
        {
            string ignored;
            while (_sendQueue.TryDequeue(out ignored)) { }
            
            _webSocket = null;

            return TaskHelper.CompletedTask;
        }

        public IEnumerable<Task> GetTasks(CancellationToken cancelToken) 
            => new Task[] { /*ReceiveAsync(cancelToken),*/ _tempTask, SendAsync(cancelToken) };

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
                                result = await _webSocket.ReceiveAsync(buffer, cancelToken).ConfigureAwait(false);
                            }
                            catch (Win32Exception ex) when (ex.HResult == HR_TIMEOUT)
                            {
                                throw new Exception($"Connection timed out.");
                            }

                            if (result.MessageType == WebSocketMessageType.Close)
                                throw new WebSocketException((int)result.CloseStatus.Value, result.CloseStatusDescription);
                            else
                                stream.Write(buffer.Array, 0, result.Count);

                        }
                        while (result == null || !result.EndOfMessage);

                        var array = stream.ToArray();
                        if (result.MessageType == WebSocketMessageType.Binary)
                            OnBinaryMessage(array);
                        else if (result.MessageType == WebSocketMessageType.Text)
                            OnTextMessage(Encoding.UTF8.GetString(array, 0, array.Length));

                        stream.Position = 0;
                        stream.SetLength(0);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }
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
                            for (var i = 0; i < frameCount; i++, offset += SendChunkSize)
                            {
                                bool isLast = i == (frameCount - 1);

                                int count;
                                if (isLast)
                                    count = byteCount - (i * SendChunkSize);
                                else
                                    count = SendChunkSize;

                                try
                                {
                                    await _webSocket.SendAsync(new ArraySegment<byte>(bytes, offset, count), WebSocketMessageType.Text, isLast, cancelToken).ConfigureAwait(false);
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

        public void QueueMessage(string message)
            => _sendQueue.Enqueue(message);
    }
}
#endif