using Discord.Helpers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.WebSockets
{
	public enum WebSocketState : byte
	{
		Disconnected,
		Connecting,
		Connected,
		Disconnecting
	}

	public class WebSocketMessageEventArgs : EventArgs
	{
		public readonly string Message;
		public WebSocketMessageEventArgs(string msg) { Message = msg; }
	}
    internal interface IWebSocketEngine
	{
		event EventHandler<WebSocketMessageEventArgs> ProcessMessage;

		Task Connect(string host, CancellationToken cancelToken);
		Task Disconnect();
		void QueueMessage(string message);
        Task[] GetTasks(CancellationToken cancelToken);
    }

	internal abstract partial class WebSocket
	{
		protected readonly IWebSocketEngine _engine;
		protected readonly DiscordSimpleClient _client;
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

		public WebSocket(DiscordSimpleClient client)
		{
			_client = client;
			_logLevel = client.Config.LogLevel;

			_loginTimeout = client.Config.ConnectionTimeout;
			_cancelToken = new CancellationToken(true);
			_connectedEvent = new ManualResetEventSlim(false);

#if DNXCORE50
			_engine = new BuiltInWebSocketEngine(client.Config.WebSocketInterval);
#else
			_engine = new WSSharpWebSocketEngine(this, client.Config.UserAgent, client.Config.WebSocketInterval);
#endif
			_engine.ProcessMessage += async (s, e) =>
			{
				if (_logLevel >= LogMessageSeverity.Debug)
					RaiseOnLog(LogMessageSeverity.Debug, $"In:  {e.Message}");
				await ProcessMessage(e.Message);
			};
        }

		protected virtual async Task Connect()
		{
			if (_state != (int)WebSocketState.Disconnected)
				throw new InvalidOperationException("Client is already connected or connecting to the server.");

            try
			{
				await Disconnect().ConfigureAwait(false);

				if (ParentCancelToken == null)
					throw new InvalidOperationException("Parent cancel token was never set.");
				_cancelTokenSource = new CancellationTokenSource();
				_cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, ParentCancelToken.Value).Token;

				_lastHeartbeat = DateTime.UtcNow;
				await _engine.Connect(Host, _cancelToken).ConfigureAwait(false);

				_state = (int)WebSocketState.Connecting;
				_runTask = RunTasks();
			}
			catch (Exception ex)
			{
				await DisconnectInternal(ex, isUnexpected: false).ConfigureAwait(false);
				throw; //Dont handle this exception internally, send up it upwards
			}
		}
		protected void CompleteConnect()
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
			Task[] tasks = GetTasks();
			Task firstTask = Task.WhenAny(tasks);
			Task allTasks = Task.WhenAll(tasks);

			//Wait until the first task ends/errors and capture the error
            try {  await firstTask.ConfigureAwait(false); }
			catch (Exception ex) { await DisconnectInternal(ex: ex, skipAwait: true).ConfigureAwait(false); }

			//Ensure all other tasks are signaled to end.
			await DisconnectInternal(skipAwait: true);

			//Wait for the remaining tasks to complete
			try { await allTasks.ConfigureAwait(false); }
			catch { }

			//Start cleanup
			await Cleanup().ConfigureAwait(false);
		}
		protected virtual Task[] GetTasks()
		{
			var cancelToken = _cancelToken;
            return _engine.GetTasks(cancelToken)
				.Concat(new Task[] { HeartbeatAsync(cancelToken) })
				.ToArray();
		}
		protected virtual async Task Cleanup()
		{
			var disconnectState = _disconnectState;
			_disconnectState = WebSocketState.Disconnected;
			var wasDisconnectUnexpected = _wasDisconnectUnexpected;
			_wasDisconnectUnexpected = false;
			//Dont reset disconnectReason, we may called ThrowError() later

			await _engine.Disconnect();
			_cancelTokenSource = null;
			var oldState = _state;
            _state = (int)WebSocketState.Disconnected;
			_runTask = null;
			_connectedEvent.Reset();

			if (disconnectState == WebSocketState.Connected)
				RaiseDisconnected(wasDisconnectUnexpected, _disconnectReason?.SourceException);
		}

		protected abstract Task ProcessMessage(string json);
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
							await Task.Delay(100, cancelToken);
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
