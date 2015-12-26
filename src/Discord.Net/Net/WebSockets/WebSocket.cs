using Discord.API.Client;
using Discord.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
	public abstract partial class WebSocket
    {
        private readonly Semaphore _lock;
        protected readonly IWebSocketEngine _engine;
		protected readonly DiscordClient _client;
		protected readonly ManualResetEventSlim _connectedEvent;
        protected readonly TaskManager _taskManager;
        protected readonly JsonSerializer _serializer;
        protected CancellationTokenSource _cancelTokenSource;
        protected int _heartbeatInterval;
		private DateTime _lastHeartbeat;
        
        /// <summary> Gets the logger used for this client. </summary>
        protected internal Logger Logger { get; }

        public CancellationToken CancelToken { get; private set; }

        public CancellationToken? ParentCancelToken { get; set; }

		public string Host { get; set; }
        /// <summary> Gets the current connection state of this client. </summary>
        public ConnectionState State { get; private set; }

        public event EventHandler Connected = delegate { };
		private void OnConnected()
		    => Connected(this, EventArgs.Empty);
        public event EventHandler<DisconnectedEventArgs> Disconnected = delegate { };
		private void OnDisconnected(bool wasUnexpected, Exception error)
            => Disconnected(this, new DisconnectedEventArgs(wasUnexpected, error));

		public WebSocket(DiscordClient client, JsonSerializer serializer, Logger logger)
		{
            _client = client;
            Logger = logger;
            _serializer = serializer;

            _lock = new Semaphore(1, 1);
            _taskManager = new TaskManager(Cleanup);
            CancelToken = new CancellationToken(true);
			_connectedEvent = new ManualResetEventSlim(false);

#if !DOTNET5_4
			_engine = new WS4NetEngine(client.Config, _taskManager);
#else
			//_engine = new BuiltInWebSocketEngine(this, client.Config);
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
			            ProcessMessage(reader.ReadToEnd()).Wait();
	            }
            };
			_engine.TextMessage += (s, e) => ProcessMessage(e.Message).Wait(); 
		}

		protected async Task BeginConnect()
		{
            try
            {
                _lock.WaitOne();
                try
                {
                    await _taskManager.Stop().ConfigureAwait(false);
                    _taskManager.ClearException();
                    State = ConnectionState.Connecting;
                    
                    _cancelTokenSource = new CancellationTokenSource();
                    CancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, ParentCancelToken.Value).Token;
                    _lastHeartbeat = DateTime.UtcNow;

                    await _engine.Connect(Host, CancelToken).ConfigureAwait(false);
                    await Run().ConfigureAwait(false);
                }
                finally
                {
                    _lock.Release();
                }
			}
			catch (Exception ex)
			{
                _taskManager.SignalError(ex, true);
                throw;
			}
		}
		protected void EndConnect()
		{
			try
            {
                State = ConnectionState.Connected;

				_connectedEvent.Set();
                Logger.Info($"Connected");
                OnConnected();
			}
			catch (Exception ex)
            {
                _taskManager.SignalError(ex, true);
            }
		}

		protected abstract Task Run();
		protected virtual async Task Cleanup()
		{
            var oldState = State;
            State = ConnectionState.Disconnecting;

            await _engine.Disconnect().ConfigureAwait(false);
			_cancelTokenSource = null;
			_connectedEvent.Reset();

            if (oldState == ConnectionState.Connected)
            {
                var ex = _taskManager.Exception;
                if (ex == null)
                    Logger.Info("Disconnected");
                else
                    Logger.Error("Disconnected", ex);
                OnDisconnected(_taskManager.WasUnexpected, _taskManager.Exception);
            }
            State = ConnectionState.Disconnected;
        }

		protected virtual Task ProcessMessage(string json)
		{
			if (Logger.Level >= LogSeverity.Debug)
                Logger.Debug( $"In: {json}");
			return TaskHelper.CompletedTask;
		}		
		protected void QueueMessage(IWebSocketMessage message)
		{
			string json = JsonConvert.SerializeObject(new WebSocketMessage(message));
			if (Logger.Level >= LogSeverity.Debug)
                Logger.Debug( $"Out: {json}");
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
						if (this.State == ConnectionState.Connected)
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

        public void WaitForConnection(CancellationToken cancelToken)
        {
            try
            {
                //Cancel if either DiscordClient.Disconnect is called, data socket errors or timeout is reached
                cancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, CancelToken).Token;
                _connectedEvent.Wait(cancelToken);
            }
            catch (OperationCanceledException)
            {
                _taskManager.ThrowException(); //Throws data socket's internal error if any occured
                throw;
            }
        }
	}
}
