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

		protected string _host;
		protected int _loginTimeout, _heartbeatInterval;
		private DateTime _lastHeartbeat;
		private Task _runTask;

		public WebSocketState State => (WebSocketState)_state;
		protected int _state;

		protected ExceptionDispatchInfo _disconnectReason;
		private bool _wasDisconnectUnexpected;

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

		public async Task Reconnect(CancellationToken cancelToken)
		{
			try
			{
				await Task.Delay(_client.Config.ReconnectDelay, cancelToken).ConfigureAwait(false);
				while (!cancelToken.IsCancellationRequested)
				{
					try
					{
						await Connect(_host, cancelToken).ConfigureAwait(false);
						break;
					}
					catch (OperationCanceledException) { throw; }
					catch (Exception ex)
					{
						RaiseOnLog(LogMessageSeverity.Error, $"DataSocket reconnect failed: {ex.GetBaseException().Message}");
						//Net is down? We can keep trying to reconnect until the user runs Disconnect()
						await Task.Delay(_client.Config.FailedReconnectDelay, cancelToken).ConfigureAwait(false);
					}
				}
			}
			catch (OperationCanceledException) { }
		}
		protected virtual async Task Connect(string host, CancellationToken cancelToken)
		{
			if (_state != (int)WebSocketState.Disconnected)
				throw new InvalidOperationException("Client is already connected or connecting to the server.");

			try
			{
				await Disconnect().ConfigureAwait(false);
				
				_state = (int)WebSocketState.Connecting;

				_cancelTokenSource = new CancellationTokenSource();
				_cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, cancelToken).Token;

				await _engine.Connect(host, _cancelToken).ConfigureAwait(false);
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
		/*public Task Reconnect(CancellationToken cancelToken)
			=> Connect(_host, _cancelToken);*/

		public Task Disconnect() => DisconnectInternal(new Exception("Disconnect was requested by user."), isUnexpected: false);
		protected Task DisconnectInternal(Exception ex, bool isUnexpected = true, bool skipAwait = false)
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
				_disconnectReason = ExceptionDispatchInfo.Capture(ex);
				_cancelTokenSource.Cancel();
			}

			if (!skipAwait)
				return _runTask ?? TaskHelper.CompletedTask;
			else
				return TaskHelper.CompletedTask;
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
