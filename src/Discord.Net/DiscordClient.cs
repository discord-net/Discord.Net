using Discord.API;
using Discord.Collections;
using Discord.Net;
using Discord.Net.Voice;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	/// <summary> Provides a connection to the DiscordApp service. </summary>
	public partial class DiscordClient : DiscordWebSocketClient
	{
		protected readonly DiscordAPIClient _api;
		private readonly Random _rand;
		private readonly JsonSerializer _serializer;
		private readonly ConcurrentQueue<Message> _pendingMessages;
		private readonly ConcurrentDictionary<string, DiscordWebSocketClient> _voiceClients;
		private bool _sentInitialLog;
		private uint _nextVoiceClientId;
		private string _status;
		private int? _gameId;

		public new DiscordClientConfig Config => _config as DiscordClientConfig;

		/// <summary> Returns the current logged-in user. </summary>
		public User CurrentUser => _currentUser;
        private User _currentUser;

		/// <summary> Returns a collection of all channels this client is a member of. </summary>
		public Channels Channels => _channels;
		private readonly Channels _channels;
		/// <summary> Returns a collection of all user-server pairs this client can currently see. </summary>
		public Members Members => _members;
		private readonly Members _members;
		/// <summary> Returns a collection of all messages this client has seen since logging in and currently has in cache. </summary>
		public Messages Messages => _messages;
		private readonly Messages _messages;
		//TODO: Do we need the roles cache?
		/// <summary> Returns a collection of all role-server pairs this client can currently see. </summary>
		public Roles Roles => _roles;
		private readonly Roles _roles;
		/// <summary> Returns a collection of all servers this client is a member of. </summary>
		public Servers Servers => _servers;
		private readonly Servers _servers;
		/// <summary> Returns a collection of all users this client can currently see. </summary>
		public Users Users => _users;
		private readonly Users _users;

		/// <summary> Initializes a new instance of the DiscordClient class. </summary>
		public DiscordClient(DiscordClientConfig config = null)
			: base(config ?? new DiscordClientConfig())
		{
			_rand = new Random();
			_api = new DiscordAPIClient(_config);
			if (Config.UseMessageQueue)
				_pendingMessages = new ConcurrentQueue<Message>();
			if (Config.EnableVoiceMultiserver)
				_voiceClients = new ConcurrentDictionary<string, DiscordWebSocketClient>();

			object cacheLock = new object();
			_channels = new Channels(this, cacheLock);
			_members = new Members(this, cacheLock);
			_messages = new Messages(this, cacheLock);
			_roles = new Roles(this, cacheLock);
			_servers = new Servers(this, cacheLock);
			_users = new Users(this, cacheLock);
			_status = UserStatus.Online;

			this.Connected += async (s, e) =>
			{
				_api.CancelToken = CancelToken;
				await SendStatus();
			};

			VoiceDisconnected += (s, e) =>
			{
				foreach (var member in _members)
				{
					if (member.ServerId == e.ServerId && member.IsSpeaking)
					{
						member.IsSpeaking = false;
						RaiseUserIsSpeaking(member, false);
					}
				}
			};

			bool showIDs = _config.LogLevel > LogMessageSeverity.Debug; //Hide this for now
			if (_config.LogLevel >= LogMessageSeverity.Info)
			{
				ServerCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Created Server: {e.Server?.Name}" +
					(showIDs ? $" ({e.ServerId})" : ""));
				ServerDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Destroyed Server: {e.Server?.Name}" +
					(showIDs ? $" ({e.ServerId})" : ""));
				ServerUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Updated Server: {e.Server?.Name}" +
					(showIDs ? $" ({e.ServerId})" : ""));
				ServerAvailable += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Server Unavailable: {e.Server?.Name}" +
					(showIDs ? $" ({e.ServerId})" : ""));
				ServerUnavailable += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Server Unavailable: {e.Server?.Name}" +
					(showIDs ? $" ({e.ServerId})" : ""));
				ChannelCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Created Channel: {e.Server?.Name ?? "[Private]"}/{e.Channel.Name}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.ChannelId})" : ""));
				ChannelDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Destroyed Channel: {e.Server?.Name ?? "[Private]"}/{e.Channel.Name}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.ChannelId})" : ""));
				ChannelUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Updated Channel: {e.Server?.Name ?? "[Private]"}/{e.Channel.Name}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.ChannelId})" : ""));
				MessageCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Created Message: {e.Server?.Name ?? "[Private]"}/{e.Channel.Name}/{e.MessageId}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.ChannelId}/{e.MessageId})" : ""));
				MessageDeleted += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Deleted Message: {e.Server?.Name ?? "[Private]"}/{e.Channel.Name}/{e.MessageId}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.ChannelId}/{e.MessageId})" : ""));
				MessageUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Updated Message: {e.Server?.Name ?? "[Private]"}/{e.Channel.Name}/{e.MessageId}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.ChannelId}/{e.MessageId})" : ""));
				RoleCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Created Role: {e.Server?.Name ?? "[Private]"}/{e.Role.Name}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.RoleId})." : ""));
				RoleUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Updated Role: {e.Server?.Name ?? "[Private]"}/{e.Role.Name}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.RoleId})." : ""));
				RoleDeleted += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Deleted Role: {e.Server?.Name ?? "[Private]"}/{e.Role.Name}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.RoleId})." : ""));
				BanAdded += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Added Ban: {e.Server?.Name ?? "[Private]"}/{e.User?.Name ?? "Unknown"}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.UserId})." : ""));
				BanRemoved += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Removed Ban: {e.Server?.Name ?? "[Private]"}/{e.User?.Name ?? "Unknown"}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.UserId})." : ""));
				UserAdded += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Added Member: {e.Server?.Name ?? "[Private]"}/{e.User.Name}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.UserId})." : ""));
				UserRemoved += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Removed Member: {e.Server?.Name ?? "[Private]"}/{e.User.Name}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.UserId})." : ""));
				MemberUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Updated Member: {e.Server?.Name ?? "[Private]"}/{e.User.Name}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.UserId})." : ""));
				UserVoiceStateUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Updated Member (Voice State): {e.Server?.Name ?? "[Private]"}/{e.User.Name}" +
					(showIDs ? $" ({e.ServerId ?? "0"}/{e.UserId})" : ""));
				UserUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Updated User: {e.User.Name}" +
					(showIDs ? $" ({e.UserId})." : ""));
			}
			if (_config.LogLevel >= LogMessageSeverity.Verbose)
			{
				UserIsTyping += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client,
					$"Updated User (Is Typing): {e.Server?.Name ?? "[Private]"}/{e.Channel.Name}/{e.User.Name}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.ChannelId}/{e.UserId})" : ""));
				MessageReadRemotely += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client, 
					$"Read Message (Remotely): {e.Server?.Name ?? "[Private]"}/{e.Channel.Name}/{e.MessageId}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.ChannelId}/{e.MessageId})" : ""));
				MessageSent += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client, 
					$"Sent Message: {e.Server?.Name ?? "[Private]"}/{e.Channel.Name}/{e.MessageId}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.ChannelId}/{e.MessageId})" : ""));
				UserPresenceUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client, 
					$"Updated Member (Presence): {e.Server?.Name ?? "[Private]"}/{e.User.Name}" +
					(showIDs ? $" ({e.ServerId ?? "[Private]"}/{e.UserId})" : ""));
				
				_api.RestClient.OnRequest += (s, e) =>
				{
					if (e.Payload != null)
						RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Rest, $"{e.Method.Method} {e.Path}: {Math.Round(e.ElapsedMilliseconds, 2)} ms ({e.Payload})");
					else
						RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Rest, $"{e.Method.Method} {e.Path}: {Math.Round(e.ElapsedMilliseconds, 2)} ms");
				};
			}
			if (_config.LogLevel >= LogMessageSeverity.Debug)
			{
				_channels.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created Channel {e.Item?.ServerId ?? "[Private]"}/{e.Item.Id}");
				_channels.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed Channel {e.Item.ServerId ?? "[Private]"}/{e.Item.Id}");
				_channels.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Channels");
				_members.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created Member {e.Item.ServerId ?? "[Private]"}/{e.Item.UserId}");
				_members.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed Member {e.Item.ServerId ?? "[Private]"}/{e.Item.UserId}");
				_members.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Members");
				_messages.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created Message {e.Item.ServerId ?? "[Private]"}/{e.Item.ChannelId}/{e.Item.Id}");
				_messages.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed Message {e.Item.ServerId ?? "[Private]"}/{e.Item.ChannelId}/{e.Item.Id}");
				_messages.ItemRemapped += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Remapped Message {e.Item.ServerId ?? "[Private]"}/{e.Item.ChannelId}/[{e.OldId} -> {e.NewId}]");
				_messages.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Messages");
				_roles.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created Role {e.Item.ServerId}/{e.Item.Id}");
				_roles.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed Role {e.Item.ServerId}/{e.Item.Id}");
				_roles.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Roles");
				_servers.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created Server {e.Item.Id}");
				_servers.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed Server {e.Item.Id}");
				_servers.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Servers");
				_users.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created User {e.Item.Id}");
				_users.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed User {e.Item.Id}");
				_users.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Users");
			}

			if (Config.UseMessageQueue)
				_pendingMessages = new ConcurrentQueue<Message>();

			_serializer = new JsonSerializer();
#if TEST_RESPONSES
			_serializer.CheckAdditionalContent = true;
			_serializer.MissingMemberHandling = MissingMemberHandling.Error;
#endif
		}

		internal override VoiceWebSocket CreateVoiceSocket()
		{
			var socket = base.CreateVoiceSocket();
			socket.IsSpeaking += (s, e) =>
			{
				if (_voiceSocket.State == WebSocketState.Connected)
				{
					var member = _members[e.UserId, socket.CurrentServerId];
					bool value = e.IsSpeaking;
					if (member.IsSpeaking != value)
					{
						member.IsSpeaking = value;
						RaiseUserIsSpeaking(member, value);
						if (Config.TrackActivity)
							member.UpdateActivity();
					}
				}
			};
			return socket;
		}

		/// <summary> Connects to the Discord server with the provided email and password. </summary>
		/// <returns> Returns a token for future connections. </returns>
		public new async Task<string> Connect(string email, string password)
		{
			if (!_sentInitialLog)
				SendInitialLog();

			if (State != DiscordClientState.Disconnected)
				await Disconnect().ConfigureAwait(false);
			
			string token;
			try
			{
				var response = await _api.Login(email, password)
					.Timeout(_config.APITimeout)
					.ConfigureAwait(false);
				token = response.Token;
				if (_config.LogLevel >= LogMessageSeverity.Verbose)
					RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client, "Login successful, got token.");
			}
			catch (TaskCanceledException) { throw new TimeoutException(); }

			await Connect(token);
			return token;
		}

		/// <summary> Connects to the Discord server with the provided token. </summary>
		public async Task Connect(string token)
		{
			if (!_sentInitialLog)
				SendInitialLog();

			if (State != (int)DiscordClientState.Disconnected)
				await Disconnect().ConfigureAwait(false);

			_api.Token = token;
			string gateway = (await _api.Gateway()
				.Timeout(_config.APITimeout)
				.ConfigureAwait(false)).Url;
			if (_config.LogLevel >= LogMessageSeverity.Verbose)
				RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client, $"Websocket endpoint: {gateway}");

			await base.Connect(gateway, token)
				.Timeout(_config.ConnectionTimeout)
				.ConfigureAwait(false);
		}

		protected override async Task Cleanup()
		{
			await base.Cleanup().ConfigureAwait(false);

			if (_config.VoiceMode != DiscordVoiceMode.Disabled)
			{
				if (Config.EnableVoiceMultiserver)
				{
					var tasks = _voiceClients
						.Select(x => x.Value.Disconnect())
						.ToArray();
					_voiceClients.Clear();
					await Task.WhenAll(tasks).ConfigureAwait(false);
				}
			}

			if (Config.UseMessageQueue)
			{
				Message ignored;
				while (_pendingMessages.TryDequeue(out ignored)) { }
			}

			_channels.Clear();
			_members.Clear();
			_messages.Clear();
			_roles.Clear();
			_servers.Clear();
			_users.Clear();

			_currentUser = null;
		}

		protected override IEnumerable<Task> GetTasks()
		{
			if (Config.UseMessageQueue)
				return base.GetTasks().Concat(new Task[] { MessageQueueLoop() });
			else
				return base.GetTasks();
		}
		
		private Task MessageQueueLoop()
		{
			var cancelToken = CancelToken;
			int interval = Config.MessageQueueInterval;

			return Task.Run(async () =>
			{
				Message msg;
				while (!cancelToken.IsCancellationRequested)
				{
					while (_pendingMessages.TryDequeue(out msg))
					{
						bool hasFailed = false;
						SendMessageResponse response = null;
						try
						{
							response = await _api.SendMessage(msg.ChannelId, msg.RawText, msg.MentionIds, msg.Nonce, msg.IsTTS).ConfigureAwait(false);
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
						RaiseMessageSent(msg);
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

		internal override async Task OnReceivedEvent(WebSocketEventEventArgs e)
		{
			try
			{
				await base.OnReceivedEvent(e);

				switch (e.Type)
				{
					//Global
					case "READY": //Resync
						{
							var data = e.Payload.ToObject<ReadyEvent>(_serializer);
							_currentUser = _users.GetOrAdd(data.User.Id);
							_currentUser.Update(data.User);
							foreach (var model in data.Guilds)
							{
								if (!model.Unavailable)
								{
									var server = _servers.GetOrAdd(model.Id);
									server.Update(model);
								}
							}
							foreach (var model in data.PrivateChannels)
							{
								var user = _users.GetOrAdd(model.Recipient.Id);
								user.Update(model.Recipient);
								var channel = _channels.GetOrAdd(model.Id, null, user.Id);
								channel.Update(model);
							}
						}
						break;

					//Servers
					case "GUILD_CREATE":
						{
							var data = e.Payload.ToObject<GuildCreateEvent>(_serializer);
							if (!data.Unavailable)
							{
								var server = _servers.GetOrAdd(data.Id);
								server.Update(data);
								if (data.Unavailable == false)
									RaiseServerAvailable(server);
								else
									RaiseServerCreated(server);
							}
						}
						break;
					case "GUILD_UPDATE":
						{
							var data = e.Payload.ToObject<GuildUpdateEvent>(_serializer);
							var server = _servers[data.Id];
							if (server != null)
							{
								server.Update(data);
								RaiseServerUpdated(server);
							}
						}
						break;
					case "GUILD_DELETE":
						{
							var data = e.Payload.ToObject<GuildDeleteEvent>(_serializer);
							var server = _servers.TryRemove(data.Id);
							if (server != null)
							{
								if (data.Unavailable == true)
									RaiseServerAvailable(server);
								else
									RaiseServerDestroyed(server);
							}
						}
						break;

					//Channels
					case "CHANNEL_CREATE":
						{
							var data = e.Payload.ToObject<ChannelCreateEvent>(_serializer);
							Channel channel;
							if (data.IsPrivate)
							{
								var user = _users.GetOrAdd(data.Recipient.Id);
								user.Update(data.Recipient);
								channel = _channels.GetOrAdd(data.Id, null, user.Id);
							}
							else
								channel = _channels.GetOrAdd(data.Id, data.GuildId, null);
							channel.Update(data);
							RaiseChannelCreated(channel);
						}
						break;
					case "CHANNEL_UPDATE":
						{
							var data = e.Payload.ToObject<ChannelUpdateEvent>(_serializer);
							var channel = _channels[data.Id];
							if (channel != null)
							{
								channel.Update(data);
								RaiseChannelUpdated(channel);
							}
						}
						break;
					case "CHANNEL_DELETE":
						{
							var data = e.Payload.ToObject<ChannelDeleteEvent>(_serializer);
							var channel = _channels.TryRemove(data.Id);
							if (channel != null)
								RaiseChannelDestroyed(channel);
						}
						break;

					//Members
					case "GUILD_MEMBER_ADD":
						{
							var data = e.Payload.ToObject<MemberAddEvent>(_serializer);
							var user = _users.GetOrAdd(data.User.Id);
							user.Update(data.User);
							var member = _members.GetOrAdd(data.User.Id, data.GuildId);
							member.Update(data);
							if (Config.TrackActivity)
								member.UpdateActivity();
							RaiseUserAdded(member);
						}
						break;
					case "GUILD_MEMBER_UPDATE":
						{
							var data = e.Payload.ToObject<MemberUpdateEvent>(_serializer);
							var member = _members[data.User.Id, data.GuildId];
							if (member != null)
							{
								member.Update(data);
								RaiseMemberUpdated(member);
							}
						}
						break;
					case "GUILD_MEMBER_REMOVE":
						{
							var data = e.Payload.ToObject<MemberRemoveEvent>(_serializer);
							var member = _members.TryRemove(data.UserId, data.GuildId);
							if (member != null)
								RaiseUserRemoved(member);
						}
						break;

					//Roles
					case "GUILD_ROLE_CREATE":
						{
							var data = e.Payload.ToObject<RoleCreateEvent>(_serializer);
							var role = _roles.GetOrAdd(data.Data.Id, data.GuildId, false);
							role.Update(data.Data);
							var server = _servers[data.GuildId];
							if (server != null)
								server.AddRole(data.Data.Id);
							RaiseRoleUpdated(role);
						}
						break;
					case "GUILD_ROLE_UPDATE":
						{
							var data = e.Payload.ToObject<RoleUpdateEvent>(_serializer);
							var role = _roles[data.Data.Id];
							if (role != null)
								role.Update(data.Data);
							RaiseRoleUpdated(role);
						}
						break;
					case "GUILD_ROLE_DELETE":
						{
							var data = e.Payload.ToObject<RoleDeleteEvent>(_serializer);
							var server = _servers[data.GuildId];
							if (server != null)
								server.RemoveRole(data.RoleId);
							var role = _roles.TryRemove(data.RoleId);
							if (role != null)
								RaiseRoleDeleted(role);
						}
						break;

					//Bans
					case "GUILD_BAN_ADD":
						{
							var data = e.Payload.ToObject<BanAddEvent>(_serializer);
							var server = _servers[data.GuildId];
							if (server != null)
							{
								server.AddBan(data.User?.Id);
								RaiseBanAdded(data.User?.Id, server);
							}
						}
						break;
					case "GUILD_BAN_REMOVE":
						{
							var data = e.Payload.ToObject<BanRemoveEvent>(_serializer);
							var server = _servers[data.GuildId];
							if (server != null && server.RemoveBan(data.User?.Id))
								RaiseBanRemoved(data.User?.Id, server);
						}
						break;

					//Messages
					case "MESSAGE_CREATE":
						{
							var data = e.Payload.ToObject<MessageCreateEvent>(_serializer);
							Message msg = null;

							bool isAuthor = data.Author.Id == CurrentUserId;
                            bool hasFinishedSending = false;
							if (Config.UseMessageQueue && isAuthor && data.Nonce != null)
							{
								msg = _messages.Remap("nonce" + data.Nonce, data.Id);
								if (msg != null)
								{
									msg.IsQueued = false;
									msg.Id = data.Id;
									hasFinishedSending = true;
                                }
							}

							if (msg == null)
								msg = _messages.GetOrAdd(data.Id, data.ChannelId, data.Author.Id);
							msg.Update(data);
							if (Config.TrackActivity)
							{
								var channel = msg.Channel;
								if (channel == null || channel.IsPrivate)
								{
									var user = msg.User;
									if (user != null)
										user.UpdateActivity(data.Timestamp);
								}
								else
								{
									var member = msg.Member;
									if (member != null)
										member.UpdateActivity(data.Timestamp);
								}
							}

							if (Config.AckMessages && isAuthor)
								await _api.AckMessage(data.Id, data.ChannelId);

							if (hasFinishedSending)
								RaiseMessageSent(msg);
							RaiseMessageCreated(msg);
						}
						break;
					case "MESSAGE_UPDATE":
						{
							var data = e.Payload.ToObject<MessageUpdateEvent>(_serializer);
							var msg = _messages[data.Id];
							if (msg != null)
							{
								msg.Update(data);
								RaiseMessageUpdated(msg);
							}
						}
						break;
					case "MESSAGE_DELETE":
						{
							var data = e.Payload.ToObject<MessageDeleteEvent>(_serializer);
							var msg = _messages.TryRemove(data.Id);
							if (msg != null)
								RaiseMessageDeleted(msg);
						}
						break;
					case "MESSAGE_ACK":
						{
							var data = e.Payload.ToObject<MessageAckEvent>(_serializer);
							var msg = GetMessage(data.MessageId);
							if (msg != null)
								RaiseMessageReadRemotely(msg);
						}
						break;

					//Statuses
					case "PRESENCE_UPDATE":
						{
							var data = e.Payload.ToObject<PresenceUpdateEvent>(_serializer);
							var member = _members[data.User.Id, data.GuildId];
							/*if (_config.TrackActivity)
							{
								var user = _users[data.User.Id];
								if (user != null)
									user.UpdateActivity(DateTime.UtcNow);
							}*/
							if (member != null)
							{
								member.Update(data);
								RaiseUserPresenceUpdated(member);
							}
						}
						break;
					case "TYPING_START":
						{
							var data = e.Payload.ToObject<TypingStartEvent>(_serializer);
							var channel = _channels[data.ChannelId];
							var user = _users[data.UserId];

							if (user != null)
							{
								if (channel != null)
									RaiseUserIsTyping(user, channel);
							}
							if (Config.TrackActivity)
							{
								if (channel.IsPrivate)
								{
									if (user != null)
										user.UpdateActivity();
								}
								else
								{
									var member = _members[data.UserId, channel.ServerId];
									if (member != null)
										member.UpdateActivity();
								}
							}
						}
						break;

					//Voice
					case "VOICE_STATE_UPDATE":
						{
							var data = e.Payload.ToObject<MemberVoiceStateUpdateEvent>(_serializer);
							var member = _members[data.UserId, data.GuildId];
							if (member != null)
							{
								if (data.ChannelId != member.VoiceChannelId && member.IsSpeaking)
								{
									member.IsSpeaking = false;
									RaiseUserIsSpeaking(member, false);
								}
								member.Update(data);
								RaiseUserVoiceStateUpdated(member);
							}
						}
						break;

					//Settings
					case "USER_UPDATE":
						{
							var data = e.Payload.ToObject<UserUpdateEvent>(_serializer);
							var user = _users[data.Id];
							if (user != null)
							{
								user.Update(data);
								RaiseUserUpdated(user);
							}
						}
						break;

					//Ignored
					case "USER_SETTINGS_UPDATE":
					case "GUILD_INTEGRATIONS_UPDATE":
                        break;

					//Internal (handled in DataWebSocket)
					case "RESUMED":
						break;

					//Internal (handled in DiscordSimpleClient)
					case "VOICE_SERVER_UPDATE":
						break;

					//Others
					default:
						RaiseOnLog(LogMessageSeverity.Warning, LogMessageSource.DataWebSocket, $"Unknown message type: {e.Type}");
						break;
				}
			}
			catch (Exception ex)
			{
				RaiseOnLog(LogMessageSeverity.Error, LogMessageSource.Client, $"Error handling {e.Type} event: {ex.GetBaseException().Message}");
			}
		}

		public IDiscordVoiceClient GetVoiceClient(Server server)
			=> GetVoiceClient(server.Id);
		public IDiscordVoiceClient GetVoiceClient(string serverId)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			if (!Config.EnableVoiceMultiserver)
			{
				if (serverId == _voiceServerId)
					return this;
				else
					return null;
			}

			DiscordWebSocketClient client;
			if (_voiceClients.TryGetValue(serverId, out client))
				return client;
			else
				return null;
		}
		private async Task<IDiscordVoiceClient> CreateVoiceClient(string serverId)
		{
            if (!Config.EnableVoiceMultiserver)
			{
				_voiceServerId = serverId;
				return this;
			}

			var client = _voiceClients.GetOrAdd(serverId, _ =>
			{
				var config = _config.Clone();
				config.LogLevel = _config.LogLevel;// (LogMessageSeverity)Math.Min((int)_config.LogLevel, (int)LogMessageSeverity.Warning);
				config.VoiceOnly = true;
				config.VoiceClientId = unchecked(++_nextVoiceClientId);
				return new DiscordWebSocketClient(config, serverId);
			});
			client.LogMessage += (s, e) => RaiseOnLog(e.Severity, e.Source, $"(#{client.Config.VoiceClientId}) {e.Message}");
            await client.Connect(_gateway, _token).ConfigureAwait(false);
			return client;
		}

		public Task<IDiscordVoiceClient> JoinVoiceServer(Channel channel)
			=> JoinVoiceServer(channel?.ServerId, channel?.Id);
		public Task<IDiscordVoiceClient> JoinVoiceServer(Server server, string channelId)
			=> JoinVoiceServer(server?.Id, channelId);
		public async Task<IDiscordVoiceClient> JoinVoiceServer(string serverId, string channelId)
		{
			CheckReady(); //checkVoice is done inside the voice client
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			var client = await CreateVoiceClient(serverId).ConfigureAwait(false);
			await client.JoinChannel(channelId).ConfigureAwait(false);
			return client;
		}

		public Task LeaveVoiceServer(Server server)
			=> LeaveVoiceServer(server?.Id);
        public async Task LeaveVoiceServer(string serverId)
		{
			CheckReady(); //checkVoice is done inside the voice client
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			if (Config.EnableVoiceMultiserver)
			{
				DiscordWebSocketClient client;
				if (_voiceClients.TryRemove(serverId, out client))
					await client.Disconnect();
			}
			else
			{
				_dataSocket.SendLeaveVoice(serverId);
				await _voiceSocket.Disconnect();
			}
        }

		private void SendInitialLog()
		{
			if (_config.LogLevel >= LogMessageSeverity.Verbose)
				RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client, $"Config: {JsonConvert.SerializeObject(_config)}");
			_sentInitialLog = true;
        }
	}
}