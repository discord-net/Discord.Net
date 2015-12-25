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
        private readonly ConcurrentQueue<string> _sendQueue;
        private readonly TaskManager _taskManager;
        private WS4NetWebSocket _webSocket;
        private ManualResetEventSlim _waitUntilConnect;

        public event EventHandler<WebSocketBinaryMessageEventArgs> BinaryMessage = delegate { };
        public event EventHandler<WebSocketTextMessageEventArgs> TextMessage = delegate { };
        private void OnBinaryMessage(byte[] data)
            => BinaryMessage(this, new WebSocketBinaryMessageEventArgs(data));
        private void OnTextMessage(string msg)
            => TextMessage(this, new WebSocketTextMessageEventArgs(msg));

        internal WS4NetEngine(DiscordConfig config, TaskManager taskManager)
        {
            _config = config;
            _taskManager = taskManager;
            _sendQueue = new ConcurrentQueue<string>();
            _waitUntilConnect = new ManualResetEventSlim();
        }

        public Task Connect(string host, CancellationToken cancelToken)
        {
            _webSocket = new WS4NetWebSocket(host);
            _webSocket.EnableAutoSendPing = false;
            _webSocket.NoDelay = true;
            _webSocket.Proxy = null;

            _webSocket.DataReceived += OnWebSocketBinary;
            _webSocket.MessageReceived += OnWebSocketText;
            _webSocket.Error += OnWebSocketError;
            _webSocket.Closed += OnWebSocketClosed;
            _webSocket.Opened += OnWebSocketOpened;

            _waitUntilConnect.Reset();
            _webSocket.Open();
            _waitUntilConnect.Wait(cancelToken);
            _taskManager.ThrowException(); //In case our connection failed
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

        private void OnWebSocketError(object sender, ErrorEventArgs e)
        {
            _taskManager.SignalError(e.Exception, isUnexpected: true);
            _waitUntilConnect.Set();
        }
        private void OnWebSocketClosed(object sender, EventArgs e)
        {
            Exception ex;
            if (e is ClosedEventArgs)
                ex = new Exception($"Received close code {(e as ClosedEventArgs).Code}: {(e as ClosedEventArgs).Reason ?? "No reason"}");
            else
                ex = new Exception($"Connection lost");
            _taskManager.SignalError(ex, isUnexpected: true);
            _waitUntilConnect.Set();
        }
        private void OnWebSocketOpened(object sender, EventArgs e)
            => _waitUntilConnect.Set();
        private void OnWebSocketText(object sender, MessageReceivedEventArgs e)
            => OnTextMessage(e.Message);
        private void OnWebSocketBinary(object sender, DataReceivedEventArgs e)
            => OnBinaryMessage(e.Data);

        public IEnumerable<Task> GetTasks(CancellationToken cancelToken) => new Task[] { SendAsync(cancelToken) };

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
            => _sendQueue.Enqueue(message);
    }
}
#endif