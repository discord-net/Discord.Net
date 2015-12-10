#if !DOTNET5_4
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WS4NetWebSocket = WebSocket4Net.WebSocket;

namespace Discord.Net.WebSockets
{
    internal class WS4NetEngine : IWebSocketEngine
    {
        private readonly DiscordConfig _config;
        private readonly Logger _logger;
        private readonly ConcurrentQueue<string> _sendQueue;
        private readonly WebSocket _parent;
        private WS4NetWebSocket _webSocket;
        private ManualResetEventSlim _waitUntilConnect;

        public event EventHandler<WebSocketBinaryMessageEventArgs> BinaryMessage;
        public event EventHandler<WebSocketTextMessageEventArgs> TextMessage;
        private void RaiseBinaryMessage(byte[] data)
        {
            if (BinaryMessage != null)
                BinaryMessage(this, new WebSocketBinaryMessageEventArgs(data));
        }
        private void RaiseTextMessage(string msg)
        {
            if (TextMessage != null)
                TextMessage(this, new WebSocketTextMessageEventArgs(msg));
        }

        internal WS4NetEngine(WebSocket parent, DiscordConfig config, Logger logger)
        {
            _parent = parent;
            _config = config;
            _logger = logger;
            _sendQueue = new ConcurrentQueue<string>();
            _waitUntilConnect = new ManualResetEventSlim();
        }

        public Task Connect(string host, CancellationToken cancelToken)
        {
            _webSocket = new WS4NetWebSocket(host);
            _webSocket.EnableAutoSendPing = false;
            _webSocket.NoDelay = true;
            _webSocket.Proxy = null; //Disable

            _webSocket.DataReceived += (s, e) =>
            {
                RaiseBinaryMessage(e.Data);
            };
            _webSocket.MessageReceived += (s, e) =>
            {
                RaiseTextMessage(e.Message);
            };
            _webSocket.Error += async (s, e) =>
            {
                _logger.Log(LogSeverity.Error, "WebSocket Error", e.Exception);
                await _parent.SignalDisconnect(e.Exception, isUnexpected: true).ConfigureAwait(false);
                _waitUntilConnect.Set();
            };
            _webSocket.Closed += async (s, e) =>
            {
                /*string code = e.WasClean ? e.Code.ToString() : "Unexpected";
                string reason = e.Reason != "" ? e.Reason : "No Reason";*/
                var ex = new Exception($"Got Close Message");// ({code}): {reason}");
                await _parent.SignalDisconnect(ex, isUnexpected: false/*true*/).ConfigureAwait(false);
                _waitUntilConnect.Set();
            };
            _webSocket.Opened += (s, e) =>
            {
                _waitUntilConnect.Set();
            };

            _waitUntilConnect.Reset();
            _webSocket.Open();
            _waitUntilConnect.Wait(cancelToken);
            return TaskHelper.CompletedTask;
        }

        public Task Disconnect()
        {
            string ignored;
            while (_sendQueue.TryDequeue(out ignored)) { }

            var socket = _webSocket;
            _webSocket = null;
            if (socket != null)
                socket.Close();

            return TaskHelper.CompletedTask;
        }

        public IEnumerable<Task> GetTasks(CancellationToken cancelToken)
        {
            return new Task[]
            {
                SendAsync(cancelToken)
            };
        }

        private Task SendAsync(CancellationToken cancelToken)
        {
            var sendInterval = _config.WebSocketInterval;
            return Task.Run(async () =>
            {
                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        string json;
                        while (_sendQueue.TryDequeue(out json))
                            _webSocket.Send(json);
                        await Task.Delay(sendInterval, cancelToken).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }

        public void QueueMessage(string message)
        {
            _sendQueue.Enqueue(message);
        }
    }
}
#endif