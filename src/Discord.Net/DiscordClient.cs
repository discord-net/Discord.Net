using Discord.Collections;
using Discord.Helpers;
using Discord.Net;
using Discord.Net.API;
using Discord.Net.WebSockets;
using System;
using System.Collections.Concurrent;
using System.Net;
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

	/// <summary> Provides a connection to the DiscordApp service. </summary>
	public partial class DiscordClient
	{
		private readonly Random _rand;
		private readonly DiscordAPIClient _api;
		private readonly DataWebSocket _dataSocket;
		private readonly VoiceWebSocket _voiceSocket;
		private readonly ConcurrentQueue<Message> _pendingMessages;
		private readonly ManualResetEvent _disconnectedEvent;
		private readonly ManualResetEventSlim _connectedEvent;
		private Task _runTask;
		protected ExceptionDispatchInfo _disconnectReason;
		private bool _wasDisconnectUnexpected;

		/// <summary> Returns the id of the current logged-in user. </summary>
		public string CurrentUserId => _currentUserId;
		private string _currentUserId;
		/// <summary> Returns the current logged-in user. </summary>
		public User CurrentUser => _currentUser;
        private User _currentUser;

		public DiscordClientState State => (DiscordClientState)_state;
		private int _state;

		public DiscordClientConfig Config => _config;
		private readonly DiscordClientConfig _config;
		
		public Channels Channels => _channels;
		private readonly Channels _channels;
		public Members Members => _members;
		private readonly Members _members;
		public Messages Messages => _messages;
		private readonly Messages _messages;
		public Roles Roles => _roles;
		private readonly Roles _roles;
		public Servers Servers => _servers;
		private readonly Servers _servers;
		public Users Users => _users;
		private readonly Users _users;

		public CancellationToken CancelToken => _cancelToken.Token;
		private CancellationTokenSource _cancelToken;

		/// <summary> Initializes a new instance of the DiscordClient class. </summary>
		public DiscordClient(DiscordClientConfig config = null)
		{
			_config = config ?? new DiscordClientConfig();
			_config.Lock();

			_state = (int)DiscordClientState.Disconnected;
			_disconnectedEvent = new ManualResetEvent(true);
			_connectedEvent = new ManualResetEventSlim(false);
			_rand = new Random();

			_api = new DiscordAPIClient(_config.LogLevel);
			_dataSocket = new DataWebSocket(this);
			_dataSocket.Connected += (s, e) => { if (_state == (int)DiscordClientState.Connecting) CompleteConnect(); };
			_voiceSocket = new VoiceWebSocket(this);
						
			_channels = new Channels(this);
			_members = new Members(this);
			_messages = new Messages(this);
			_roles = new Roles(this);
			_servers = new Servers(this);
			_users = new Users(this);

			_dataSocket.LogMessage += (s, e) => RaiseOnLog(e.Severity, LogMessageSource.DataWebSocket, e.Message);
			_voiceSocket.LogMessage += (s, e) => RaiseOnLog(e.Severity, LogMessageSource.DataWebSocket, e.Message);
			if (_config.LogLevel >= LogMessageSeverity.Info)
			{
				_dataSocket.Connected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.DataWebSocket, "Connected");
				_dataSocket.Disconnected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.DataWebSocket, "Disconnected");
				_voiceSocket.Connected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.VoiceWebSocket, "Connected");
				_voiceSocket.Disconnected += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.VoiceWebSocket, "Disconnected");
			}
			if (_config.LogLevel >= LogMessageSeverity.Verbose)
			{
				_channels.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Created Channel {e.Item.ServerId}/{e.Item.Id}");
				_channels.ItemUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Updated Channel {e.Item.ServerId}/{e.Item.Id}");
				_channels.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Destroyed Channel {e.Item.ServerId}/{e.Item.Id}");
				_members.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Created Member {e.Item.ServerId}/{e.Item.UserId}");
				_members.ItemUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Updated Member {e.Item.ServerId}/{e.Item.UserId}");
				_members.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Destroyed Member {e.Item.ServerId}/{e.Item.UserId}");
				_messages.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Created Message {e.Item.ServerId}/{e.Item.ChannelId}/{e.Item.Id}");
				_messages.ItemUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Updated Message {e.Item.ServerId}/{e.Item.ChannelId}/{e.Item.Id}");
				_messages.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Destroyed Message {e.Item.ServerId}/{e.Item.ChannelId}/{e.Item.Id}");
				_roles.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Created Role {e.Item.ServerId}/{e.Item.Id}");
				_roles.ItemUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Updated Role {e.Item.ServerId}/{e.Item.Id}");
				_roles.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Destroyed Role {e.Item.ServerId}/{e.Item.Id}");
				_servers.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Created Server {e.Item.Id}");
				_servers.ItemUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Updated Server {e.Item.Id}");
				_servers.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Destroyed Server {e.Item.Id}");
				_users.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Created User {e.Item.Id}");
				_users.ItemUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Updated User {e.Item.Id}");
				_users.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"Destroyed User {e.Item.Id}");

				_api.RestClient.OnRequest += (s, e) =>
				{
					if (e.Payload != null)
						RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"{e.Method.Method} {e.Path}: {Math.Round(e.ElapsedMilliseconds, 2)} ({e.Payload})");
					else
						RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Cache, $"{e.Method.Method} {e.Path}: {Math.Round(e.ElapsedMilliseconds, 2)}");
				};
			}

			if (_config.UseMessageQueue)
				_pendingMessages = new ConcurrentQueue<Message>();
        }

		private void _dataSocket_Connected(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		//Connection
		/// <summary> Connects to the Discord server with the provided token. </summary>
		public async Task Connect(string token)
		{
			await Disconnect().ConfigureAwait(false);

			if (_config.LogLevel >= LogMessageSeverity.Verbose)
				RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Authentication, $"Using cached token.");

            await ConnectInternal(token).ConfigureAwait(false);
        }
		/// <summary> Connects to the Discord server with the provided email and password. </summary>
		/// <returns> Returns a token for future connections. </returns>
		public async Task<string> Connect(string email, string password)
		{
			await Disconnect().ConfigureAwait(false);
	
			var response = await _api.Login(email, password).ConfigureAwait(false);
			if (_config.LogLevel >= LogMessageSeverity.Verbose)
				RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Authentication, "Login successful, got token.");
			
			return await ConnectInternal(response.Token).ConfigureAwait(false);
		}
		private async Task<string> ConnectInternal(string token)
		{
			if (_state != (int)DiscordClientState.Disconnected)
				throw new InvalidOperationException("Client is already connected or connecting to the server.");

            try
			{
				_disconnectedEvent.Reset();
				_cancelToken = new CancellationTokenSource();
				_state = (int)DiscordClientState.Connecting;

				_api.Token = token;
				string url = (await _api.GetWebSocketEndpoint().ConfigureAwait(false)).Url;
				if (_config.LogLevel >= LogMessageSeverity.Verbose)
					RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Authentication, $"Websocket endpoint: {url}");
				
				await _dataSocket.Login(url, token).ConfigureAwait(false);				

				_runTask = RunTasks();

				try
				{
					if (!_connectedEvent.Wait(_config.ConnectionTimeout, CancellationTokenSource.CreateLinkedTokenSource(_cancelToken.Token, _dataSocket.CancelToken).Token)) 
						throw new Exception("Operation timed out.");
				}
				catch (OperationCanceledException)
				{
					_dataSocket.ThrowError();
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
			_state = (int)WebSocketState.Connected;
			_connectedEvent.Set();
		}

		/// <summary> Disconnects from the Discord server, canceling any pending requests. </summary>
		public Task Disconnect() => DisconnectInternal(new Exception("Disconnect was requested by user."));
		protected async Task DisconnectInternal(Exception ex, bool isUnexpected = true)
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
				_disconnectReason = ExceptionDispatchInfo.Capture(ex);
				_cancelToken.Cancel();
			}

			Task task = _runTask;
			if (task != null)
				await task.ConfigureAwait(false);

			if (hasWriterLock)
			{
				_state = (int)DiscordClientState.Disconnected;
				_disconnectedEvent.Set();
				_connectedEvent.Reset();
            }
		}

		private async Task RunTasks()
		{
			Task task;
			if (_config.UseMessageQueue)
				task = MessageQueueLoop();
			else
				task = _cancelToken.Wait();

			try
			{
				await task.ConfigureAwait(false);
			}
			catch (Exception ex) { await DisconnectInternal(ex).ConfigureAwait(false); }

			await Cleanup().ConfigureAwait(false);
			_runTask = null;
		}
		private async Task Cleanup()
		{
			_disconnectedEvent.Set();

			await _dataSocket.Disconnect().ConfigureAwait(false);
#if !DNXCORE50
			if (_config.EnableVoice)
				await _voiceSocket.Disconnect().ConfigureAwait(false);
#endif

			Message ignored;
			while (_pendingMessages.TryDequeue(out ignored)) { }
			
			_channels.Clear();
			_members.Clear();
			_messages.Clear();
			_roles.Clear();
			_servers.Clear();
			_users.Clear();

			_currentUser = null;
			_currentUserId = null;
		}

		//Helpers
		private void CheckReady(bool checkVoice = false)
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

#if !DNXCORE50
			if (checkVoice && !_config.EnableVoice)
#else
			if (checkVoice) //Always fail on DNXCORE50
#endif
				throw new InvalidOperationException("Voice is not enabled for this client.");
		}
		/// <summary> Blocking call that will not return until client has been stopped. This is mainly intended for use in console applications. </summary>
		public void Block()
		{
			_disconnectedEvent.WaitOne();
		}

		//Experimental
		private Task MessageQueueLoop()
		{
			var cancelToken = _cancelToken.Token;
			int interval = _config.MessageQueueInterval;

			return Task.Run(async () =>
			{
				Message msg;
				while (!cancelToken.IsCancellationRequested)
				{
					while (_pendingMessages.TryDequeue(out msg))
					{
						bool hasFailed = false;
						Responses.SendMessage response = null;
						try
						{
							response = await _api.SendMessage(msg.ChannelId, msg.RawText, msg.MentionIds, msg.Nonce).ConfigureAwait(false);
						}
						catch (WebException) { break; }
						catch (HttpException) { hasFailed = true; }

						if (!hasFailed)
						{
							_messages.Remap(msg.Id, response.Id);
							msg.Id = response.Id;
							msg.Update(response);
						}
						msg.IsQueued = false;
						msg.HasFailed = hasFailed;
						try { RaiseMessageSent(msg); } catch { }
					}
					await Task.Delay(interval).ConfigureAwait(false);
				}
			});
		}
		private string GenerateNonce()
		{
			lock (_rand)
				return _rand.Next().ToString();
		}
	}
}
