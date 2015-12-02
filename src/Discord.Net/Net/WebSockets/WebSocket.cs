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
	internal enum WebSocketState : byte
	{
		Disconnected,
		Connecting,
		Connected,
		Disconnecting
	}

	internal abstract partial class WebSocket
	{
		protected readonly IWebSocketEngine _engine;
		protected readonly DiscordWSClient _client;
		protected readonly LogMessageSeverity _logLevel;
		protected readonly ManualResetEventSlim _connectedEvent;

		protected ExceptionDispatchInfo _disconnectReason;
		protected bool _wasDisconnectUnexpected;
		protected WebSocketState _disconnectState;

		protected int _loginTimeout, _heartbeatInterval;
		private DateTime _lastHeartbeat;
		private Task _runTask;

		public CancellationToken? ParentCancelToken { get; set; }
		public CancellationToken CancelToken => _cancelToken;
		private CancellationTokenSource _cancelTokenSource;
		protected CancellationToken _cancelToken;

		public string Host { get; set; }

		public WebSocketState State => (WebSocketState)_state;
		protected int _state;

		public WebSocket(DiscordWSClient client)
		{
			_client = client;
			_logLevel = client.Config.LogLevel;

			_loginTimeout = client.Config.ConnectionTimeout;
			_cancelToken = new CancellationToken(true);
			_connectedEvent = new ManualResetEventSlim(false);

#if !DOTNET5_4
			_engine = new WebSocketSharpEngine(this, client.Config);
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
			_engine.TextMessage += (s, e) =>
			{
				/*await*/ ProcessMessage(e.Message).Wait();
			};
		}

		protected async Task BeginConnect()
		{
			try
			{
				await Disconnect().ConfigureAwait(false);

				if (ParentCancelToken == null)
					throw new InvalidOperationException("Parent cancel token was never set.");
				_cancelTokenSource = new CancellationTokenSource();
				_cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, ParentCancelToken.Value).Token;

				_state = (int)WebSocketState.Connecting;
			}
			catch (Exception ex)
			{
				await DisconnectInternal(ex, isUnexpected: false).ConfigureAwait(false);
				throw;
			}
		}

		protected virtual async Task Start()
		{
            try
			{
				if (_state != (int)WebSocketState.Connecting)
					throw new InvalidOperationException("Socket is in the wrong state.");

				_lastHeartbeat = DateTime.UtcNow;
				await _engine.Connect(Host, _cancelToken).ConfigureAwait(false);

				_runTask = RunTasks();
			}
			catch (Exception ex)
			{
				await DisconnectInternal(ex, isUnexpected: false).ConfigureAwait(false);
				throw;
			}
		}
		protected void EndConnect()
		{
			_state = (int)WebSocketState.Connected;
			_connectedEvent.Set();
			RaiseConnected();
        }

		public Task Disconnect() => DisconnectInternal(new Exception("Disconnect was requested by user."), isUnexpected: false);
		protected internal async Task DisconnectInternal(Exception ex = null, bool isUnexpected = true, bool skipAwait = false)
		{
			int oldState;
			bool hasWriterLock;

			//If in either connecting or connected state, get a lock by being the first to switch to disconnecting
			oldState = Interlocked.CompareExchange(ref _state, (int)WebSocketState.Disconnecting, (int)WebSocketState.Connecting);
			if (oldState == (int)WebSocketState.Disconnected) return; //Already disconnected
			hasWriterLock = oldState == (int)WebSocketState.Connecting; //Caused state change
			if (!hasWriterLock)
			{
				oldState = Interlocked.CompareExchange(ref _state, (int)WebSocketState.Disconnecting, (int)WebSocketState.Connected);
				if (oldState == (int)WebSocketState.Disconnected) return; //Already disconnected
				hasWriterLock = oldState == (int)WebSocketState.Connected; //Caused state change
			}

			if (hasWriterLock)
			{
				_wasDisconnectUnexpected = isUnexpected;
				_disconnectState = (WebSocketState)oldState;
				_disconnectReason = ex != null ? ExceptionDispatchInfo.Capture(ex) : null;

				_cancelTokenSource.Cancel();
				if (_disconnectState == WebSocketState.Connecting) //_runTask was never made
					await Cleanup().ConfigureAwait(false);
			}

			if (!skipAwait)
			{
				Task task = _runTask;
				if (_runTask != null)
					await task.ConfigureAwait(false);
			}
		}

		protected virtual async Task RunTasks()
		{
			Task[] tasks = GetTasks().ToArray();
			Task firstTask = Task.WhenAny(tasks);
			Task allTasks = Task.WhenAll(tasks);

			//Wait until the first task ends/errors and capture the error
            try {  await firstTask.ConfigureAwait(false); }
			catch (Exception ex) { await DisconnectInternal(ex: ex, skipAwait: true).ConfigureAwait(false); }

			//Ensure all other tasks are signaled to end.
			await DisconnectInternal(skipAwait: true).ConfigureAwait(false);

			//Wait for the remaining tasks to complete
			try { await allTasks.ConfigureAwait(false); }
			catch { }

			//Start cleanup
			await Cleanup().ConfigureAwait(false);
		}
		protected virtual IEnumerable<Task> GetTasks()
		{
			var cancelToken = _cancelToken;
			return _engine.GetTasks(cancelToken)
				.Concat(new Task[] { HeartbeatAsync(cancelToken) });
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
			if (_logLevel >= LogMessageSeverity.Debug)
				RaiseOnLog(LogMessageSeverity.Debug, $"In: {json}");
			return TaskHelper.CompletedTask;
		}
		protected abstract object GetKeepAlive();
		
		protected void QueueMessage(object message)
		{
			string json = JsonConvert.SerializeObject(message);
			if (_logLevel >= LogMessageSeverity.Debug)
				RaiseOnLog(LogMessageSeverity.Debug, $"Out: " + json);
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
							QueueMessage(GetKeepAlive());
							await Task.Delay(_heartbeatInterval, cancelToken).ConfigureAwait(false);
						}
						else
							await Task.Delay(100, cancelToken).ConfigureAwait(false);
					}
				}
				catch (OperationCanceledException) { }
			});
		}

		internal void ThrowError()
		{
			if (_wasDisconnectUnexpected)
			{
				var reason = _disconnectReason;
				_disconnectReason = null;
				reason.Throw();
			}
		}
	}
}
