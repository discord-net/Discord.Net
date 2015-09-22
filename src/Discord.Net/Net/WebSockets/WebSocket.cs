using Discord.Helpers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
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
		void QueueMessage(byte[] message);
        Task[] RunTasks(CancellationToken cancelToken);
    }

	internal abstract partial class WebSocket
	{
		protected readonly IWebSocketEngine _engine;
		protected readonly DiscordClient _client;
		protected readonly LogMessageSeverity _logLevel;

		public string Host { get; set; }

		protected int _loginTimeout, _heartbeatInterval;
		private DateTime _lastHeartbeat;
		private Task _runTask;

		public WebSocketState State => (WebSocketState)_state;
		protected int _state;

		protected ExceptionDispatchInfo _disconnectReason;
		private bool _wasDisconnectUnexpected;

		public CancellationToken ParentCancelToken { get; set; }
		public CancellationToken CancelToken => _cancelToken;
		private CancellationTokenSource _cancelTokenSource;
		protected CancellationToken _cancelToken;

		public WebSocket(DiscordClient client)
		{
			_client = client;
			_logLevel = client.Config.LogLevel;
			_loginTimeout = client.Config.ConnectionTimeout;
			_cancelToken = new CancellationToken(true);

			_engine = new BuiltInWebSocketEngine(client.Config.WebSocketInterval);
			_engine.ProcessMessage += async (s, e) =>
			{
				if (_logLevel >= LogMessageSeverity.Debug)
					RaiseOnLog(LogMessageSeverity.Debug, $"In:  " + e.Message);
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
				
				_state = (int)WebSocketState.Connecting;

				_cancelTokenSource = new CancellationTokenSource();
				if (ParentCancelToken != null)
					_cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, ParentCancelToken).Token;
				else
					_cancelToken = _cancelTokenSource.Token;

				await _engine.Connect(Host, _cancelToken).ConfigureAwait(false);
				_lastHeartbeat = DateTime.UtcNow;

				_runTask = RunTasks();
			}
			catch
			{
				await Disconnect().ConfigureAwait(false);
				throw;
			}
		}
		protected void CompleteConnect()
		{
			_state = (int)WebSocketState.Connected;
			RaiseConnected();
        }
		/*public Task Reconnect(CancellationToken cancelToken)
			=> Connect(_host, _cancelToken);*/

		public Task Disconnect() => DisconnectInternal(new Exception("Disconnect was requested by user."), isUnexpected: false);
		protected Task DisconnectInternal(Exception ex = null, bool isUnexpected = true, bool skipAwait = false)
		{
			int oldState;
			bool hasWriterLock;

			//If in either connecting or connected state, get a lock by being the first to switch to disconnecting
			oldState = Interlocked.CompareExchange(ref _state, (int)WebSocketState.Disconnecting, (int)WebSocketState.Connecting);
			if (oldState == (int)WebSocketState.Disconnected) return TaskHelper.CompletedTask; //Already disconnected
			hasWriterLock = oldState == (int)WebSocketState.Connecting; //Caused state change
			if (!hasWriterLock)
			{
				oldState = Interlocked.CompareExchange(ref _state, (int)WebSocketState.Disconnecting, (int)WebSocketState.Connected);
				if (oldState == (int)WebSocketState.Disconnected) return TaskHelper.CompletedTask; //Already disconnected
				hasWriterLock = oldState == (int)WebSocketState.Connected; //Caused state change
			}

			if (hasWriterLock)
			{
				_wasDisconnectUnexpected = isUnexpected;
				_disconnectReason = ex != null ? ExceptionDispatchInfo.Capture(ex) : null;
				_cancelTokenSource.Cancel();
			}

			if (!skipAwait)
				return _runTask ?? TaskHelper.CompletedTask;
			else
				return TaskHelper.CompletedTask;
		}

		protected virtual async Task RunTasks()
		{
			Task[] tasks = Run();
			Task firstTask = Task.WhenAny(tasks);
			Task allTasks = Task.WhenAll(tasks);

            try {  await firstTask.ConfigureAwait(false); }
			catch (Exception ex) { await DisconnectInternal(ex: ex, skipAwait: true).ConfigureAwait(false); }

			//When the first task ends, make sure the rest do too
			await DisconnectInternal(skipAwait: true);
			try { await allTasks.ConfigureAwait(false); }
			catch { }

			bool wasUnexpected = _wasDisconnectUnexpected;
			_wasDisconnectUnexpected = false;
			
			await Cleanup(wasUnexpected).ConfigureAwait(false);
			_runTask = null;
		}
		protected virtual Task[] Run()
		{
			var cancelToken = _cancelToken;
            return _engine.RunTasks(cancelToken)
				.Concat(new Task[] { HeartbeatAsync(cancelToken) })
				.ToArray();
		}
		protected virtual Task Cleanup(bool wasUnexpected)
		{
			_cancelTokenSource = null;
			_state = (int)WebSocketState.Disconnected;
			RaiseDisconnected(wasUnexpected, _disconnectReason?.SourceException);
			return _engine.Disconnect();
		}

		protected abstract Task ProcessMessage(string json);
		protected abstract object GetKeepAlive();
		
		protected void QueueMessage(object message)
		{
			string json = JsonConvert.SerializeObject(message);
			if (_logLevel >= LogMessageSeverity.Debug)
				RaiseOnLog(LogMessageSeverity.Debug, $"Out: " + json);
			var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
			_engine.QueueMessage(bytes);
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
