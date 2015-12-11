#if !DOTNET5_4
using SuperSocket.ClientEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;
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

            _webSocket.DataReceived += OnWebSocketBinary;
            _webSocket.MessageReceived += OnWebSocketText;
            _webSocket.Error += OnWebSocketError;
            _webSocket.Closed += OnWebSocketClosed;
            _webSocket.Opened += OnWebSocketOpened;

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
            {
                //We dont want a slow disconnect to mess up the next connection, so lets just unregister events
                socket.DataReceived -= OnWebSocketBinary;
                socket.MessageReceived -= OnWebSocketText;
                socket.Error -= OnWebSocketError;
                socket.Closed -= OnWebSocketClosed;
                socket.Opened -= OnWebSocketOpened;
                socket.Close();
            }

            return TaskHelper.CompletedTask;
        }

        private async void OnWebSocketError(object sender, ErrorEventArgs e)
        {
            await _parent.SignalDisconnect(e.Exception, isUnexpected: true).ConfigureAwait(false);
            _waitUntilConnect.Set();
        }
        private async void OnWebSocketClosed(object sender, EventArgs e)
        {
            var ex = new Exception($"Connection lost or close message received.");
            await _parent.SignalDisconnect(ex, isUnexpected: false/*true*/).ConfigureAwait(false);
            _waitUntilConnect.Set();
        }
        private void OnWebSocketOpened(object sender, EventArgs e)
        {
            _waitUntilConnect.Set();
        }
        private void OnWebSocketText(object sender, MessageReceivedEventArgs e)
        {
            RaiseTextMessage(e.Message);
        }
        private void OnWebSocketBinary(object sender, DataReceivedEventArgs e)
        {
            RaiseBinaryMessage(e.Data);
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