using Discord.API;
using Discord.Collections;
using Discord.Helpers;
using Discord.WebSockets.Data;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceWebSocket = Discord.WebSockets.Voice.VoiceWebSocket;

namespace Discord
{
	public enum DiscordClientState : byte
	{
		Disconnected,
		Connecting,
		Connected,
		Disconnecting
	}

	/// <summary> Provides a barebones connection to the Discord service </summary>
	public partial class DiscordBaseClient
	{
		internal readonly DataWebSocket _dataSocket;
		internal readonly VoiceWebSocket _voiceSocket;
		protected readonly ManualResetEvent _disconnectedEvent;
		protected readonly ManualResetEventSlim _connectedEvent;
		private Task _runTask;
		private string _gateway, _token;

		protected ExceptionDispatchInfo _disconnectReason;
		private bool _wasDisconnectUnexpected;

		/// <summary> Returns the id of the current logged-in user. </summary>
		public string CurrentUserId => _currentUserId;
		private string _currentUserId;
		/*/// <summary> Returns the server this user is currently connected to for voice. </summary>
		public string CurrentVoiceServerId => _voiceSocket.CurrentServerId;*/

		/// <summary> Returns the current connection state of this client. </summary>
		public DiscordClientState State => (DiscordClientState)_state;
		private int _state;

		/// <summary> Returns the configuration object used to make this client. Note that this object cannot be edited directly - to change the configuration of this client, use the DiscordClient(DiscordClientConfig config) constructor. </summary>
		public DiscordClientConfig Config => _config;
		protected readonly DiscordClientConfig _config;

		public CancellationToken CancelToken => _cancelToken;
		private CancellationTokenSource _cancelTokenSource;
		private CancellationToken _cancelToken;

		/// <summary> Initializes a new instance of the DiscordClient class. </summary>
		public DiscordBaseClient(DiscordClientConfig config = null)
		{
			_config = config ?? new DiscordClientConfig();
			_config.Lock();

			_state = (int)DiscordClientState.Disconnected;
			_cancelToken = new CancellationToken(true);
			_disconnectedEvent = new ManualResetEvent(true);
			_connectedEvent = new ManualResetEventSlim(false);

			_dataSocket = new DataWebSocket(this);
			_dataSocket.Connected += (s, e) => { if (_state == (int)DiscordClientState.Connecting) CompleteConnect(); };
			_dataSocket.Disconnected += async (s, e) => 
			{
				RaiseDisconnected(e);
				if (e.WasUnexpected)
					await _dataSocket.Reconnect(_token);
			};
			if (Config.VoiceMode != DiscordVoiceMode.Disabled)
			{
				_voiceSocket = new VoiceWebSocket(this);
				_voiceSocket.Connected += (s, e) => RaiseVoiceConnected();
				_voiceSocket.Disconnected += async (s, e) =>
				{
					RaiseVoiceDisconnected(e);
					if (e.WasUnexpected)
						await _voiceSocket.Reconnect();
				};
			}

			_dataSocket.LogMessage += (s, e) => RaiseOnLog(e.Severity, LogMessageSource.DataWebSocket, e.Message);
			if (_config.VoiceMode != DiscordVoiceMode.Disabled)
				_voiceSocket.LogMessage += (s, e) => RaiseOnLog(e.Severity, LogMessageSource.VoiceWebSocket, e.Message);
			if (_config.LogLevel >= LogMessageSeverity.Info)
			{
				_dataSocket.Connected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.DataWebSocket, "Connected");
				_dataSocket.Disconnected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.DataWebSocket, "Disconnected");
				if (_config.VoiceMode != DiscordVoiceMode.Disabled)
				{
					_voiceSocket.Connected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.VoiceWebSocket, "Connected");
					_voiceSocket.Disconnected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.VoiceWebSocket, "Disconnected");
				}
			}

			_dataSocket.ReceivedEvent += (s, e) => OnReceivedEvent(e);
		}

		//Connection
		protected async Task<string> Connect(string gateway, string token)
		{
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
				_token = token;
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
		protected Task DisconnectInternal(Exception ex = null, bool isUnexpected = true, bool skipAwait = false)
		{
			int oldState;
			bool hasWriterLock;

			//If in either connecting or connected state, get a lock by being the first to switch to disconnecting
			oldState = Interlocked.CompareExchange(ref _state, (int)DiscordClientState.Disconnecting, (int)DiscordClientState.Connecting);
			if (oldState == (int)DiscordClientState.Disconnected) return TaskHelper.CompletedTask; //Already disconnected
			hasWriterLock = oldState == (int)DiscordClientState.Connecting; //Caused state change
			if (!hasWriterLock)
			{
				oldState = Interlocked.CompareExchange(ref _state, (int)DiscordClientState.Disconnecting, (int)DiscordClientState.Connected);
				if (oldState == (int)DiscordClientState.Disconnected) return TaskHelper.CompletedTask; //Already disconnected
				hasWriterLock = oldState == (int)DiscordClientState.Connected; //Caused state change
			}

			if (hasWriterLock)
			{
				_wasDisconnectUnexpected = isUnexpected;
				_disconnectReason = ex != null ? ExceptionDispatchInfo.Capture(ex) : null;

				_cancelTokenSource.Cancel();
				/*if (_state == DiscordClientState.Connecting) //_runTask was never made
					await Cleanup().ConfigureAwait(false);*/
			}

			if (!skipAwait)
				return _runTask ?? TaskHelper.CompletedTask;
			else
				return TaskHelper.CompletedTask;
		}

		private async Task RunTasks()
		{
			Task[] tasks = Run();
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
		protected virtual Task[] Run()
		{
			return new Task[] { _cancelToken.Wait() };
		}

        protected virtual async Task Cleanup()
		{
			await _dataSocket.Disconnect().ConfigureAwait(false);
			if (_config.VoiceMode != DiscordVoiceMode.Disabled)
				await _voiceSocket.Disconnect().ConfigureAwait(false);
			
			_currentUserId = null;
			_gateway = null;
			_token = null;
		}

		//Helpers
		/// <summary> Blocking call that will not return until client has been stopped. This is mainly intended for use in console applications. </summary>
		public void Block()
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
			
			if (checkVoice && _config.VoiceMode == DiscordVoiceMode.Disabled)
				throw new InvalidOperationException("Voice is not enabled for this client.");
		}
		protected void RaiseEvent(string name, Action action)
		{
			try { action(); }
			catch (Exception ex)
			{
				RaiseOnLog(LogMessageSeverity.Error, LogMessageSource.Client,
					$"{name} event handler raised an exception: ${ex.GetBaseException().Message}");
			}
		}

		internal virtual Task OnReceivedEvent(WebSocketEventEventArgs e)
		{
			if (e.Type == "READY")
				_currentUserId = e.Payload["user"].Value<string>("id");
			return TaskHelper.CompletedTask;
		}
    }
}
