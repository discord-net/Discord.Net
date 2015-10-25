using Discord.Net;
using Discord.Net.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
	public enum DiscordClientState : byte
	{
		Disconnected,
		Connecting,
		Connected,
		Disconnecting
	}

	/// <summary> Provides a minimalistic websocket connection to the Discord service. </summary>
	public partial class DiscordWSClient
	{
		protected readonly DiscordWSClientConfig _config;
		protected readonly ManualResetEvent _disconnectedEvent;
		protected readonly ManualResetEventSlim _connectedEvent;
		internal readonly DataWebSocket _dataSocket;
		internal readonly VoiceWebSocket _voiceSocket;
		protected ExceptionDispatchInfo _disconnectReason;
		protected string _gateway, _token;
		protected string _userId, _voiceServerId;
		private Task _runTask;
		private bool _wasDisconnectUnexpected;

		public string CurrentUserId => _userId;

		/// <summary> Returns the configuration object used to make this client. Note that this object cannot be edited directly - to change the configuration of this client, use the DiscordClient(DiscordClientConfig config) constructor. </summary>
		public DiscordWSClientConfig Config => _config;		

		/// <summary> Returns the current connection state of this client. </summary>
		public DiscordClientState State => (DiscordClientState)_state;
		private int _state;

		public CancellationToken CancelToken => _cancelToken;
		private CancellationTokenSource _cancelTokenSource;
		private CancellationToken _cancelToken;

		/// <summary> Initializes a new instance of the DiscordClient class. </summary>
		public DiscordWSClient(DiscordWSClientConfig config = null)
		{
			_config = config ?? new DiscordWSClientConfig();
			_config.Lock();

			_state = (int)DiscordClientState.Disconnected;
			_cancelToken = new CancellationToken(true);
			_disconnectedEvent = new ManualResetEvent(true);
			_connectedEvent = new ManualResetEventSlim(false);

			_dataSocket = CreateDataSocket();
			if (_config.EnableVoice)
				_voiceSocket = CreateVoiceSocket();
		}
		internal DiscordWSClient(DiscordWSClientConfig config = null, string voiceServerId = null)
			: this(config)
		{
			_voiceServerId = voiceServerId;
		}

		internal virtual DataWebSocket CreateDataSocket()
		{
			var socket = new DataWebSocket(this);
			socket.Connected += (s, e) => 
			{
				if (_state == (int)DiscordClientState.Connecting)
					CompleteConnect(); }
			;
			socket.Disconnected += async (s, e) =>
			{
				RaiseDisconnected(e);
				if (e.WasUnexpected)
					await socket.Reconnect(_token);
			};

			if (!_config.VoiceOnly)
			{
				socket.LogMessage += (s, e) => RaiseOnLog(e.Severity, LogMessageSource.DataWebSocket, e.Message);
				if (_config.LogLevel >= LogMessageSeverity.Info)
				{
					socket.Connected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.DataWebSocket, "Connected");
					socket.Disconnected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.DataWebSocket, "Disconnected");
				}
			}

			socket.ReceivedEvent += async (s, e) => await OnReceivedEvent(e);
			return socket;
		}
		internal virtual VoiceWebSocket CreateVoiceSocket()
		{
			var socket = new VoiceWebSocket(this);
			socket.LogMessage += (s, e) => RaiseOnLog(e.Severity, LogMessageSource.VoiceWebSocket, e.Message);
			socket.Connected += (s, e) => RaiseVoiceConnected();
			socket.Disconnected += async (s, e) =>
			{
				RaiseVoiceDisconnected(socket.CurrentServerId, e);				
				if (e.WasUnexpected)
					await socket.Reconnect();
			};
			if (_config.LogLevel >= LogMessageSeverity.Info)
			{
				socket.Connected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.VoiceWebSocket, "Connected");
				socket.Disconnected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.VoiceWebSocket, "Disconnected");
			}
			return socket;
		}

		//Connection
		public async Task<string> Connect(string gateway, string token)
		{
			if (gateway == null) throw new ArgumentNullException(nameof(gateway));
			if (token == null) throw new ArgumentNullException(nameof(token));

			try
			{
				_state = (int)DiscordClientState.Connecting;
				_disconnectedEvent.Reset();

				_gateway = gateway;
				_token = token;

				_cancelTokenSource = new CancellationTokenSource();
				_cancelToken = _cancelTokenSource.Token;

				_dataSocket.Host = gateway;
				_dataSocket.ParentCancelToken = _cancelToken;
				if (_config.EnableVoice)
					_voiceSocket.ParentCancelToken = _cancelToken;
				await _dataSocket.Login(token).ConfigureAwait(false);				

				_runTask = RunTasks();

				try
				{
					//Cancel if either Disconnect is called, data socket errors or timeout is reached
					var cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelToken, _dataSocket.CancelToken).Token;
					_connectedEvent.Wait(cancelToken);
				}
				catch (OperationCanceledException)
				{
					_dataSocket.ThrowError(); //Throws data socket's internal error if any occured
					throw;
				}

				//_state = (int)DiscordClientState.Connected;
				return token;
			}
			catch
			{
				await Disconnect().ConfigureAwait(false);
				throw;
			}
		}
		protected void CompleteConnect()
		{
			_state = (int)DiscordClientState.Connected;
			_connectedEvent.Set();
			RaiseConnected();
		}

		/// <summary> Disconnects from the Discord server, canceling any pending requests. </summary>
		public Task Disconnect() => DisconnectInternal(new Exception("Disconnect was requested by user."), isUnexpected: false);
		protected async Task DisconnectInternal(Exception ex = null, bool isUnexpected = true, bool skipAwait = false)
		{
			int oldState;
			bool hasWriterLock;

			//If in either connecting or connected state, get a lock by being the first to switch to disconnecting
			oldState = Interlocked.CompareExchange(ref _state, (int)DiscordClientState.Disconnecting, (int)DiscordClientState.Connecting);
			if (oldState == (int)DiscordClientState.Disconnected) return; //Already disconnected
			hasWriterLock = oldState == (int)DiscordClientState.Connecting; //Caused state change
			if (!hasWriterLock)
			{
				oldState = Interlocked.CompareExchange(ref _state, (int)DiscordClientState.Disconnecting, (int)DiscordClientState.Connected);
				if (oldState == (int)DiscordClientState.Disconnected) return; //Already disconnected
				hasWriterLock = oldState == (int)DiscordClientState.Connected; //Caused state change
			}

			if (hasWriterLock)
			{
				_wasDisconnectUnexpected = isUnexpected;
				_disconnectReason = ex != null ? ExceptionDispatchInfo.Capture(ex) : null;

				_cancelTokenSource.Cancel();
				/*if (_disconnectState == DiscordClientState.Connecting) //_runTask was never made
					await Cleanup().ConfigureAwait(false);*/
			}

			if (!skipAwait)
			{
				Task task = _runTask;
				if (_runTask != null)
					await task.ConfigureAwait(false);
			}
		}

		private async Task RunTasks()
		{
			Task[] tasks = GetTasks().ToArray();
			Task firstTask = Task.WhenAny(tasks);
			Task allTasks = Task.WhenAll(tasks);

			//Wait until the first task ends/errors and capture the error
			try { await firstTask.ConfigureAwait(false); }
			catch (Exception ex) { await DisconnectInternal(ex: ex, skipAwait: true).ConfigureAwait(false); }

			//Ensure all other tasks are signaled to end.
			await DisconnectInternal(skipAwait: true);

			//Wait for the remaining tasks to complete
			try { await allTasks.ConfigureAwait(false); }
			catch { }

			//Start cleanup
			var wasDisconnectUnexpected = _wasDisconnectUnexpected;
			_wasDisconnectUnexpected = false;

			await Cleanup().ConfigureAwait(false);

			if (!wasDisconnectUnexpected)
			{
				_state = (int)DiscordClientState.Disconnected;
				_disconnectedEvent.Set();
			}
			_connectedEvent.Reset();
			_runTask = null;
		}
		protected virtual IEnumerable<Task> GetTasks()
		{
			return new Task[] { _cancelToken.Wait() };
		}

        protected virtual async Task Cleanup()
		{
			if (_config.EnableVoice)
			{
				string voiceServerId = _voiceSocket.CurrentServerId;
                if (voiceServerId != null)
					_dataSocket.SendLeaveVoice(voiceServerId);
				await _voiceSocket.Disconnect().ConfigureAwait(false);
			}
			await _dataSocket.Disconnect().ConfigureAwait(false);

			_userId = null;
			_gateway = null;
			_token = null;
		}

		//Helpers
		/// <summary> Blocking call that will not return until client has been stopped. This is mainly intended for use in console applications. </summary>
		public void Run(Func<Task> asyncAction)
		{
			asyncAction().Wait();
			_disconnectedEvent.WaitOne();
		}
		/// <summary> Blocking call that will not return until client has been stopped. This is mainly intended for use in console applications. </summary>
		public void Run()
		{
			_disconnectedEvent.WaitOne();
		}

		protected void CheckReady(bool checkVoice = false)
		{
			switch (_state)
			{
				case (int)DiscordClientState.Disconnecting:
					throw new InvalidOperationException("The client is disconnecting.");
				case (int)DiscordClientState.Disconnected:
					throw new InvalidOperationException("The client is not connected to Discord");
				case (int)DiscordClientState.Connecting:
					throw new InvalidOperationException("The client is connecting.");
			}
			
			if (checkVoice && !_config.EnableVoice)
				throw new InvalidOperationException("Voice is not enabled for this client.");
		}
		protected void RaiseEvent(string name, Action action)
		{
			try { action(); }
			catch (Exception ex)
			{
				var ex2 = ex.GetBaseException();
				RaiseOnLog(LogMessageSeverity.Error, LogMessageSource.Client,
					$"{name}'s handler raised {ex2.GetType().Name}: ${ex2.Message}");
			}
		}

		internal virtual async Task OnReceivedEvent(WebSocketEventEventArgs e)
		{
			try
			{
				switch (e.Type)
				{
					case "READY":
						_userId = e.Payload["user"].Value<string>("id");
						break;
					case "VOICE_SERVER_UPDATE":
						{
							string guildId = e.Payload.Value<string>("guild_id");

							if (_config.EnableVoice && guildId == _voiceSocket.CurrentServerId)
							{
								string token = e.Payload.Value<string>("token");
								_voiceSocket.Host = "wss://" + e.Payload.Value<string>("endpoint").Split(':')[0];
								await _voiceSocket.Login(_userId, _dataSocket.SessionId, token, CancelToken);
							}
						}
						break;
				}
			}
			catch (Exception ex)
			{
				RaiseOnLog(LogMessageSeverity.Error, LogMessageSource.Client, $"Error handling {e.Type} event: {ex.GetBaseException().Message}");
			}
		}
    }
}