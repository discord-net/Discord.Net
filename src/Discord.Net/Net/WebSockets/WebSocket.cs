using Discord.API.Client;
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

		protected int _heartbeatInterval;
		private DateTime _lastHeartbeat;

		public CancellationToken? ParentCancelToken { get; set; }
		public CancellationToken CancelToken => _cancelToken;
        protected CancellationTokenSource _cancelTokenSource;
		protected CancellationToken _cancelToken;

		public JsonSerializer Serializer => _serializer;
		protected JsonSerializer _serializer;

        internal TaskManager TaskManager => _taskManager;
        protected readonly TaskManager _taskManager;

        public Logger Logger => _logger;
		protected readonly Logger _logger;

		public string Host { get { return _host; } set { _host = value; } }
		private string _host;

		public ConnectionState State => _state;
		protected ConnectionState _state;

		public event EventHandler Connected;
		private void RaiseConnected()
		{
			if (_logger.Level >= LogSeverity.Info)
				_logger.Info( "Connected");
			if (Connected != null)
				Connected(this, EventArgs.Empty);
		}
		public event EventHandler<DisconnectedEventArgs> Disconnected;
		private void RaiseDisconnected(bool wasUnexpected, Exception error)
		{
			if (_logger.Level >= LogSeverity.Info)
				_logger.Info( "Disconnected");
			if (Disconnected != null)
				Disconnected(this, new DisconnectedEventArgs(wasUnexpected, error));
		}

		public WebSocket(DiscordClient client, Logger logger)
		{
            _client = client;
			_logger = logger;

            _lock = new Semaphore(1, 1);
            _taskManager = new TaskManager(Cleanup);
            _cancelToken = new CancellationToken(true);
			_connectedEvent = new ManualResetEventSlim(false);

#if !DOTNET5_4
			_engine = new WS4NetEngine(this, client.Config, _logger);
#else
			//_engine = new BuiltInWebSocketEngine(this, client.Config, _logger);
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

			_serializer = new JsonSerializer();
			_serializer.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
#if TEST_RESPONSES
			_serializer.CheckAdditionalContent = true;
			_serializer.MissingMemberHandling = MissingMemberHandling.Error;
#else
			_serializer.Error += (s, e) =>
			{
				e.ErrorContext.Handled = true;
				_logger.Log(LogSeverity.Error, "Serialization Failed", e.ErrorContext.Error);
			};
#endif
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
                    _state = ConnectionState.Connecting;
                    
                    _cancelTokenSource = new CancellationTokenSource();
                    _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, ParentCancelToken.Value).Token;
                    _lastHeartbeat = DateTime.UtcNow;

                    await _engine.Connect(Host, _cancelToken).ConfigureAwait(false);
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
                _state = ConnectionState.Connected;

				_connectedEvent.Set();
				RaiseConnected();
			}
			catch (Exception ex)
            {
                _taskManager.SignalError(ex, true);
            }
		}

		protected abstract Task Run();
		protected virtual async Task Cleanup()
		{
            var oldState = _state;
            _state = ConnectionState.Disconnecting;

            await _engine.Disconnect().ConfigureAwait(false);
			_cancelTokenSource = null;
			_connectedEvent.Reset();

            if (oldState == ConnectionState.Connected)
                RaiseDisconnected(_taskManager.WasUnexpected, _taskManager.Exception);
            _state = ConnectionState.Disconnected;
        }

		protected virtual Task ProcessMessage(string json)
		{
			if (_logger.Level >= LogSeverity.Debug)
				_logger.Debug( $"In: {json}");
			return TaskHelper.CompletedTask;
		}		
		protected void QueueMessage(IWebSocketMessage message)
		{
			string json = JsonConvert.SerializeObject(new WebSocketMessage(message));
			if (_logger.Level >= LogSeverity.Debug)
				_logger.Debug( $"Out: " + json);
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
						if (_state == ConnectionState.Connected)
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
	}
}
