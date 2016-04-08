using Discord.API.Client;
using Discord.Logging;
using Newtonsoft.Json;
using Nito.AsyncEx;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public abstract partial class WebSocket
    {
        private readonly AsyncLock _lock;
        protected readonly IWebSocketEngine _engine;
        protected readonly DiscordConfig _config;
        protected readonly ManualResetEventSlim _connectedEvent;
        protected readonly TaskManager _taskManager;
        protected readonly JsonSerializer _serializer;
        protected CancellationTokenSource _cancelSource;
        protected CancellationToken _parentCancelToken;
        protected int _heartbeatInterval;
        private DateTime _lastHeartbeat;

        /// <summary> Gets the logger used for this client. </summary>
        protected internal Logger Logger { get; }
        public CancellationToken CancelToken { get; private set; }

        public string Host { get; set; }
        /// <summary> Gets the current connection state of this client. </summary>
        public ConnectionState State { get; private set; }

        public event EventHandler Connected = delegate { };
        private void OnConnected()
            => Connected(this, EventArgs.Empty);
        public event EventHandler<DisconnectedEventArgs> Disconnected = delegate { };
        private void OnDisconnected(bool wasUnexpected, Exception error)
            => Disconnected(this, new DisconnectedEventArgs(wasUnexpected, error));

        public WebSocket(DiscordConfig config, JsonSerializer serializer, Logger logger)
        {
            _config = config;
            _serializer = serializer;
            Logger = logger;

            _lock = new AsyncLock();
            _taskManager = new TaskManager(Cleanup);
            CancelToken = new CancellationToken(true);
            _connectedEvent = new ManualResetEventSlim(false);

#if !DOTNET5_4
            _engine = new WS4NetEngine(config, _taskManager);
#else
            _engine = new BuiltInEngine(config);
#endif
            _engine.BinaryMessage += (s, e) =>
            {
                using (var compressed = new MemoryStream(e.Data, 2, e.Data.Length - 2))
                using (var decompressed = new MemoryStream())
                {
                    using (var zlib = new DeflateStream(compressed, CompressionMode.Decompress))
                        zlib.CopyTo(decompressed);
                    decompressed.Position = 0;
                    using (var reader = new StreamReader(decompressed))
                        ProcessMessage(reader.ReadToEnd()).GetAwaiter().GetResult();
                }
            };
            _engine.TextMessage += (s, e) => ProcessMessage(e.Message).Wait();
        }

        protected async Task BeginConnect(CancellationToken parentCancelToken)
        {
            try
            {
                using (await _lock.LockAsync().ConfigureAwait(false))
                {
                    _parentCancelToken = parentCancelToken;

                    await _taskManager.Stop().ConfigureAwait(false);
                    _taskManager.ClearException();
                    State = ConnectionState.Connecting;

                    _cancelSource = new CancellationTokenSource();
                    CancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelSource.Token, parentCancelToken).Token;
                    _lastHeartbeat = DateTime.UtcNow;

                    await _engine.Connect(Host, CancelToken).ConfigureAwait(false);
                    await Run().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                //TODO: Should this be inside the lock?
                await _taskManager.SignalError(ex).ConfigureAwait(false);
                throw;
            }
        }
        protected async Task EndConnect()
        {
            try
            {
                State = ConnectionState.Connected;
                Logger.Info($"Connected");

                OnConnected();
                _connectedEvent.Set();
            }
            catch (Exception ex)
            {
                await _taskManager.SignalError(ex).ConfigureAwait(false);
            }
        }

        protected abstract Task Run();
        protected virtual async Task Cleanup()
        {
            var oldState = State;
            State = ConnectionState.Disconnecting;

            await _engine.Disconnect().ConfigureAwait(false);
            _cancelSource = null;
            _connectedEvent.Reset();

            if (oldState == ConnectionState.Connecting || oldState == ConnectionState.Connected)
            {
                var ex = _taskManager.Exception;
                if (ex == null)
                    Logger.Info("Disconnected");
                else
                    Logger.Error("Disconnected", ex);
                State = ConnectionState.Disconnected;
                OnDisconnected(!_taskManager.WasStopExpected, _taskManager.Exception);
            }
            else
                State = ConnectionState.Disconnected;
        }

        protected virtual Task ProcessMessage(string json)
        {
            return TaskHelper.CompletedTask;
        }
        protected void QueueMessage(IWebSocketMessage message)
        {
            string json = JsonConvert.SerializeObject(new WebSocketMessage(message));
            _engine.QueueMessage(json);
        }

        protected Task HeartbeatAsync(CancellationToken cancelToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        if (this.State == ConnectionState.Connected && _heartbeatInterval > 0)
                        {
                            SendHeartbeat();
                            await Task.Delay(_heartbeatInterval, cancelToken).ConfigureAwait(false);
                        }
                        else
                            await Task.Delay(1000, cancelToken).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }
        public abstract void SendHeartbeat();

        public virtual void WaitForConnection(CancellationToken cancelToken)
        {
            try
            {
                if (!_connectedEvent.Wait(_config.ConnectionTimeout, cancelToken))
                {
                    if (State != ConnectionState.Connected)
                        throw new TimeoutException();
                }
            }
            catch (OperationCanceledException)
            {
                _taskManager.ThrowException(); //Throws data socket's internal error if any occured
                throw;
            }
        }
    }
}
