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

		protected int _state;
		protected string _host;
		protected int _loginTimeout, _heartbeatInterval;
		private DateTime _lastHeartbeat;
		private Task _runTask;

		protected ExceptionDispatchInfo _disconnectReason;
		private bool _wasDisconnectUnexpected;

		public CancellationToken CancelToken => _cancelToken.Token;
		protected CancellationTokenSource _cancelToken;

		public WebSocket(DiscordClient client)
		{
			_client = client;
			_logLevel = client.Config.LogLevel;
			_loginTimeout = client.Config.ConnectionTimeout;
			_engine = new BuiltInWebSocketEngine(client.Config.WebSocketInterval);
			_engine.ProcessMessage += (s, e) =>
			{
				if (_logLevel >= LogMessageSeverity.Debug)
					RaiseOnLog(LogMessageSeverity.Debug, $"In:  " + e.Message);
				ProcessMessage(e.Message);
			};
        }

		protected virtual async Task Connect(string host)
		{
			if (_state != (int)WebSocketState.Disconnected)
				throw new InvalidOperationException("Client is already connected or connecting to the server.");

			try
			{
				await Disconnect().ConfigureAwait(false);
				
				_state = (int)WebSocketState.Connecting;

				_cancelToken = new CancellationTokenSource();

				await _engine.Connect(host, _cancelToken.Token).ConfigureAwait(false);
				_host = host;
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
		public Task Reconnect()
			=> Connect(_host);

		public Task Disconnect() => DisconnectInternal(new Exception("Disconnect was requested by user."), isUnexpected: false);
		protected async Task DisconnectInternal(Exception ex, bool isUnexpected = true, bool skipAwait = false)
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
				_disconnectReason = ExceptionDispatchInfo.Capture(ex);
				_cancelToken.Cancel();
			}

			if (!skipAwait)
			{
				Task task = _runTask;
				if (task != null)
					await task.ConfigureAwait(false);
			}

			if (hasWriterLock)
			{
				_state = (int)WebSocketState.Disconnected;
				RaiseDisconnected(isUnexpected, ex);
			}
		}

		protected virtual async Task RunTasks()
		{
			Task task = Task.WhenAll(Run());

            try
			{
				await task.ConfigureAwait(false);
			}
			catch (Exception ex) { await DisconnectInternal(ex, skipAwait: true).ConfigureAwait(false); }

			bool wasUnexpected = _wasDisconnectUnexpected;
			_wasDisconnectUnexpected = false;

			await _engine.Disconnect().ConfigureAwait(false);
			await Cleanup().ConfigureAwait(false);
			_runTask = null;
		}
		protected virtual Task[] Run()
		{
			var cancelToken = _cancelToken.Token;
            return _engine.RunTasks(cancelToken)
				.Concat(new Task[] { HeartbeatAsync(cancelToken) })
				.ToArray();
		}
		protected virtual Task Cleanup() { return TaskHelper.CompletedTask; }

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
						if (_heartbeatInterval > 0)
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
