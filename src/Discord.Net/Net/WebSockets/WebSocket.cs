using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
	public enum WebSocketState : byte
	{
		Disconnected,
		Connecting,
		Connected,
		Disconnecting
	}

	public abstract partial class WebSocket
	{
		protected readonly IWebSocketEngine _engine;
		protected readonly DiscordConfig _config;
		protected readonly ManualResetEventSlim _connectedEvent;

		protected ExceptionDispatchInfo _disconnectReason;
		protected bool _wasDisconnectUnexpected;
		protected WebSocketState _disconnectState;

		protected int _heartbeatInterval;
		private DateTime _lastHeartbeat;
		private Task _runTask;

		public CancellationToken? ParentCancelToken { get; set; }
		public CancellationToken CancelToken => _cancelToken;
		private CancellationTokenSource _cancelTokenSource;
		protected CancellationToken _cancelToken;

		internal JsonSerializer Serializer => _serializer;
		protected JsonSerializer _serializer;

		public Logger Logger => _logger;
		protected readonly Logger _logger;

		public string Host { get { return _host; } set { _host = value; } }
		private string _host;

		public WebSocketState State => (WebSocketState)_state;
		protected int _state;

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

		public WebSocket(DiscordConfig config, Logger logger)
		{
			_config = config;
			_logger = logger;
			
			_cancelToken = new CancellationToken(true);
			_connectedEvent = new ManualResetEventSlim(false);

#if !DOTNET5_4
			_engine = new WS4NetEngine(this, _config, _logger);
#else
			//_engine = new BuiltInWebSocketEngine(this, _config, _logger);
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
			_engine.TextMessage += (s, e) =>
			{
				/*await*/ ProcessMessage(e.Message).Wait();
			};

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
				_state = (int)WebSocketState.Connecting;

				if (ParentCancelToken == null)
					throw new InvalidOperationException("Parent cancel token was never set.");
				_cancelTokenSource = new CancellationTokenSource();
				_cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, ParentCancelToken.Value).Token;

				if (_state != (int)WebSocketState.Connecting)
					throw new InvalidOperationException("Socket is in the wrong state.");

				_lastHeartbeat = DateTime.UtcNow;
				await _engine.Connect(Host, _cancelToken).ConfigureAwait(false);

				_runTask = Run();
			}
			catch (Exception ex)
			{
				await SignalDisconnect(ex, isUnexpected: false).ConfigureAwait(false);
				throw;
			}
		}
		protected async Task EndConnect()
		{
			try
			{
				_state = (int)WebSocketState.Connected;

				_connectedEvent.Set();
				RaiseConnected();
			}
			catch (Exception ex)
			{
				await SignalDisconnect(ex, isUnexpected: false).ConfigureAwait(false);
				throw;
			}
		}
		
		protected internal async Task SignalDisconnect(Exception ex = null, bool isUnexpected = false, bool wait = false)
		{
			//If in either connecting or connected state, get a lock by being the first to switch to disconnecting
			int oldState = Interlocked.CompareExchange(ref _state, (int)WebSocketState.Disconnecting, (int)WebSocketState.Connecting);
			if (oldState == (int)WebSocketState.Disconnected) return; //Already disconnected
			bool hasWriterLock = oldState == (int)WebSocketState.Connecting; //Caused state change
			if (!hasWriterLock)
			{
				oldState = Interlocked.CompareExchange(ref _state, (int)WebSocketState.Disconnecting, (int)WebSocketState.Connected);
				if (oldState == (int)WebSocketState.Disconnected) return; //Already disconnected
				hasWriterLock = oldState == (int)WebSocketState.Connected; //Caused state change
			}

			if (hasWriterLock)
            {
                if (ex != null)
                    _logger.Log(LogSeverity.Error, "Error", ex);
                CaptureError(ex ?? new Exception("Disconnect was requested."), isUnexpected);
				_cancelTokenSource.Cancel();
				if (_disconnectState == WebSocketState.Connecting) //_runTask was never made
					await Cleanup().ConfigureAwait(false);
			}

			if (wait)
			{
				Task task = _runTask;
				if (_runTask != null)
					await task.ConfigureAwait(false);
			}
		}
		private void CaptureError(Exception ex, bool isUnexpected)
		{
			_disconnectReason = ExceptionDispatchInfo.Capture(ex);
			_wasDisconnectUnexpected = isUnexpected;
		}

		protected abstract Task Run();
		protected async Task RunTasks(params Task[] tasks)
		{
			//Get all async tasks
			tasks = tasks
				.Concat(_engine.GetTasks(_cancelToken))
				.Concat(new Task[] { HeartbeatAsync(_cancelToken) })
				.ToArray();

			//Create group tasks
			Task firstTask = Task.WhenAny(tasks);
			Task allTasks = Task.WhenAll(tasks);

			//Wait until the first task ends/errors and capture the error
			Exception ex = null;
			try { await firstTask.ConfigureAwait(false); }
			catch (Exception ex2) { ex = ex2; }

			//Ensure all other tasks are signaled to end.
			await SignalDisconnect(ex, ex != null, true).ConfigureAwait(false);

			//Wait for the remaining tasks to complete
			try { await allTasks.ConfigureAwait(false); }
			catch { }

			//Start cleanup
			await Cleanup().ConfigureAwait(false);
		}

		protected virtual async Task Cleanup()
		{
			var disconnectState = _disconnectState;
			_disconnectState = WebSocketState.Disconnected;
			var wasDisconnectUnexpected = _wasDisconnectUnexpected;
			_wasDisconnectUnexpected = false;
			//Dont reset disconnectReason, we may called ThrowError() later

			await _engine.Disconnect().ConfigureAwait(false);
			_cancelTokenSource = null;
			var oldState = _state;
            _state = (int)WebSocketState.Disconnected;
			_runTask = null;
			_connectedEvent.Reset();

			if (disconnectState == WebSocketState.Connected)
				RaiseDisconnected(wasDisconnectUnexpected, _disconnectReason?.SourceException);
		}

		protected virtual Task ProcessMessage(string json)
		{
			if (_logger.Level >= LogSeverity.Debug)
				_logger.Debug( $"In: {json}");
			return TaskHelper.CompletedTask;
		}
		
		protected void QueueMessage(object message)
		{
			string json = JsonConvert.SerializeObject(message);
			if (_logger.Level >= LogSeverity.Debug)
				_logger.Debug( $"Out: " + json);
			_engine.QueueMessage(json);
		}

		private Task HeartbeatAsync(CancellationToken cancelToken)
		{
			return Task.Run(async () =>
			{
				try
				{
					while (!cancelToken.IsCancellationRequested)
					{
						if (_state == (int)WebSocketState.Connected)
						{
							SendHeartbeat();
							await Task.Delay(_heartbeatInterval, cancelToken).ConfigureAwait(false);
						}
						else
							await Task.Delay(100, cancelToken).ConfigureAwait(false);
					}
				}
				catch (OperationCanceledException) { }
			});
		}

		protected internal void ThrowError()
		{
			if (_wasDisconnectUnexpected)
			{
				var reason = _disconnectReason;
				_disconnectReason = null;
				reason.Throw();
			}
		}

		public abstract void SendHeartbeat();
	}
}
