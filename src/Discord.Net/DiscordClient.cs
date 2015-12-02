using Discord.API;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord
{
	/// <summary> Provides a connection to the DiscordApp service. </summary>
	public sealed partial class DiscordClient : DiscordWSClient
	{
		public static readonly string Version = typeof(DiscordClientConfig).GetTypeInfo().Assembly.GetName().Version.ToString(3);

		private readonly DiscordAPIClient _api;
		private readonly Random _rand;
		private readonly JsonSerializer _messageImporter;
		private readonly ConcurrentQueue<Message> _pendingMessages;
		private readonly ConcurrentDictionary<long, DiscordWSClient> _voiceClients;
		private readonly Dictionary<Type, object> _singletons;
		private bool _sentInitialLog;
		private uint _nextVoiceClientId;
		private UserStatus _status;
		private int? _gameId;

		/// <summary> Returns the configuration object used to make this client. Note that this object cannot be edited directly - to change the configuration of this client, use the DiscordClient(DiscordClientConfig config) constructor. </summary>
		public new DiscordClientConfig Config => _config as DiscordClientConfig;

		/// <summary> Gives direct access to the underlying DiscordAPIClient. This can be used to modify objects not in cache. </summary>
		public DiscordAPIClient API => _api;
		
		/// <summary> Initializes a new instance of the DiscordClient class. </summary>
		public DiscordClient(DiscordClientConfig config = null)
			: base(config ?? new DiscordClientConfig())
		{
			_rand = new Random();
			_api = new DiscordAPIClient(_config);
			if (Config.UseMessageQueue)
				_pendingMessages = new ConcurrentQueue<Message>();
			if (Config.EnableVoiceMultiserver)
				_voiceClients = new ConcurrentDictionary<long, DiscordWSClient>();

			object cacheLock = new object();
			_channels = new Channels(this, cacheLock);
			_users = new Users(this, cacheLock);
			_messages = new Messages(this, cacheLock, Config.MessageCacheLength > 0);
			_roles = new Roles(this, cacheLock);
			_servers = new Servers(this, cacheLock);
			_globalUsers = new GlobalUsers(this, cacheLock);
			_singletons = new Dictionary<Type, object>();

			_status = UserStatus.Online;

			this.Connected += async (s, e) =>
			{
				_api.CancelToken = _cancelToken;
				await SendStatus().ConfigureAwait(false);
			};

			VoiceDisconnected += (s, e) =>
			{
				var server = _servers[e.ServerId];
				if (server != null)
				{
					foreach (var member in server.Members)
					{
						if (member.IsSpeaking)
						{
							member.IsSpeaking = false;
							RaiseUserIsSpeaking(member, _channels[_voiceSocket.CurrentChannelId], false);
						}
					}
				}
			};
			
			if (_config.LogLevel >= LogMessageSeverity.Info)
			{
				JoinedServer += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Server Created: {e.Server?.Name ?? "[Private]"}");
				LeftServer += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Server Destroyed: {e.Server?.Name ?? "[Private]"}");
				ServerUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Server Updated: {e.Server?.Name ?? "[Private]"}");
				ServerAvailable += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Server Available: {e.Server?.Name ?? "[Private]"}");
				ServerUnavailable += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Server Unavailable: {e.Server?.Name ?? "[Private]"}");
				ChannelCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Channel Created: {e.Server?.Name ?? "[Private]"}/{e.Channel?.Name}");
				ChannelDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Channel Destroyed: {e.Server?.Name ?? "[Private]"}/{e.Channel?.Name}");
				ChannelUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Channel Updated: {e.Server?.Name ?? "[Private]"}/{e.Channel?.Name}");
				MessageReceived += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Message Received: {e.Server?.Name ?? "[Private]"}/{e.Channel?.Name}/{e.Message?.Id}");
				MessageDeleted += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Message Deleted: {e.Server?.Name ?? "[Private]"}/{e.Channel?.Name}/{e.Message?.Id}");
				MessageUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Message Update: {e.Server?.Name ?? "[Private]"}/{e.Channel?.Name}/{e.Message?.Id}");
				RoleCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Role Created: {e.Server?.Name ?? "[Private]"}/{e.Role?.Name}");
				RoleUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Role Updated: {e.Server?.Name ?? "[Private]"}/{e.Role?.Name}");
				RoleDeleted += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Role Deleted: {e.Server?.Name ?? "[Private]"}/{e.Role?.Name}");
				UserBanned += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Banned User: {e.Server?.Name ?? "[Private]" }/{e.UserId}");
				UserUnbanned += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"Unbanned User: {e.Server?.Name ?? "[Private]"}/{e.UserId}");
				UserJoined += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"User Joined: {e.Server?.Name ?? "[Private]"}/{e.User.Name}");
				UserLeft += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"User Left: {e.Server?.Name ?? "[Private]"}/{e.User.Name}");
				UserUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"User Updated: {e.Server?.Name ?? "[Private]"}/{e.User.Name}");
				UserVoiceStateUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					$"User Updated (Voice State): {e.Server?.Name ?? "[Private]"}/{e.User.Name}");
				ProfileUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Info, LogMessageSource.Client,
					"Profile Updated");
			}
			if (_config.LogLevel >= LogMessageSeverity.Verbose)
			{
				UserIsTypingUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client,
					$"User Updated (Is Typing): {e.Server?.Name ?? "[Private]"}/{e.Channel?.Name}/{e.User?.Name}");
				MessageReadRemotely += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client, 
					$"Read Message (Remotely): {e.Server?.Name ?? "[Private]"}/{e.Channel?.Name}/{e.Message?.Id}");
				MessageSent += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client, 
					$"Sent Message: {e.Server?.Name ?? "[Private]"}/{e.Channel?.Name}/{e.Message?.Id}");
				UserPresenceUpdated += (s, e) => RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client, 
					$"User Updated (Presence): {e.Server?.Name ?? "[Private]"}/{e.User?.Name}");
				
				_api.RestClient.OnRequest += (s, e) =>
				{
                    if (e.Payload != null)
						RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Rest, $"{e.Method} {e.Path}: {Math.Round(e.ElapsedMilliseconds, 2)} ms ({e.Payload})");
					else
						RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Rest, $"{e.Method} {e.Path}: {Math.Round(e.ElapsedMilliseconds, 2)} ms");
				};
			}
			if (_config.LogLevel >= LogMessageSeverity.Debug)
			{
                _channels.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created Channel {IdConvert.ToString(e.Item.Server?.Id) ?? "[Private]"}/{e.Item.Id}");
				_channels.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed Channel {IdConvert.ToString(e.Item.Server?.Id) ?? "[Private]"}/{e.Item.Id}");
				_channels.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Channels");
				_users.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created User {IdConvert.ToString(e.Item.Server?.Id) ?? "[Private]"}/{e.Item.Id}");
				_users.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed User {IdConvert.ToString(e.Item.Server?.Id) ?? "[Private]"}/{e.Item.Id}");
				_users.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Users");
				_messages.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created Message {IdConvert.ToString(e.Item.Server?.Id) ?? "[Private]"}/{e.Item.Channel.Id}/{e.Item.Id}");
				_messages.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed Message {IdConvert.ToString(e.Item.Server?.Id) ?? "[Private]"}/{e.Item.Channel.Id}/{e.Item.Id}");
				_messages.ItemRemapped += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Remapped Message {IdConvert.ToString(e.Item.Server?.Id) ?? "[Private]"}/{e.Item.Channel.Id}/[{e.OldId} -> {e.NewId}]");
				_messages.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Messages");
				_roles.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created Role {IdConvert.ToString(e.Item.Server?.Id) ?? "[Private]"}/{e.Item.Id}");
				_roles.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed Role {IdConvert.ToString(e.Item.Server?.Id) ?? "[Private]"}/{e.Item.Id}");
				_roles.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Roles");
				_servers.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created Server {e.Item.Id}");
				_servers.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed Server {e.Item.Id}");
				_servers.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Servers");
				_globalUsers.ItemCreated += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Created User {e.Item.Id}");
				_globalUsers.ItemDestroyed += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Destroyed User {e.Item.Id}");
				_globalUsers.Cleared += (s, e) => RaiseOnLog(LogMessageSeverity.Debug, LogMessageSource.Cache, $"Cleared Users");
			}

			if (Config.UseMessageQueue)
				_pendingMessages = new ConcurrentQueue<Message>();
			
			_messageImporter = new JsonSerializer();
			_messageImporter.ContractResolver = new Message.ImportResolver();
        }
		internal override VoiceWebSocket CreateVoiceSocket()
		{
			var socket = base.CreateVoiceSocket();
			socket.IsSpeaking += (s, e) =>
			{
				if (_voiceSocket.State == WebSocketState.Connected)
				{
					var user = _users[e.UserId, socket.CurrentServerId];
					bool value = e.IsSpeaking;
					if (user.IsSpeaking != value)
					{
						user.IsSpeaking = value;
						var channel = _channels[_voiceSocket.CurrentChannelId];
						RaiseUserIsSpeaking(user, channel, value);
						if (Config.TrackActivity)
							user.UpdateActivity();
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

			await Connect(token).ConfigureAwait(false);
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
					.ConfigureAwait(false)
				).Url;
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

			await _api.Logout().ConfigureAwait(false);

			_channels.Clear();
			_users.Clear();
			_messages.Clear();
			_roles.Clear();
			_servers.Clear();
			_globalUsers.Clear();

			_privateUser = null;
		}

		public T AddSingleton<T>(T obj)
			where T : class
		{
			_singletons.Add(typeof(T), obj);
			return obj;
		}
        public T GetSingleton<T>(bool required = true)
			where T : class
		{
			object singleton;
			T singletonT = null;
			if (_singletons.TryGetValue(typeof(T), out singleton))
				singletonT = singleton as T;

			if (singletonT == null && required)
				throw new InvalidOperationException($"This operation requires {typeof(T).Name} to be added to {nameof(DiscordClient)}.");
			return singletonT;
		}
		public T AddService<T>(T obj)
			where T : class, IService
		{
			AddSingleton(obj);
			obj.Install(this);
			return obj;
		}
		public T GetService<T>(bool required = true)
			where T : class, IService
			=> GetSingleton<T>(required);

		protected override IEnumerable<Task> GetTasks()
		{
			if (Config.UseMessageQueue)
				return base.GetTasks().Concat(new Task[] { MessageQueueLoop() });
			else
				return base.GetTasks();
		}
		
		internal override async Task OnReceivedEvent(WebSocketEventEventArgs e)
		{
			try
			{
				switch (e.Type)
				{
					//Global
					case "READY": //Resync 
						{
							base.OnReceivedEvent(e).Wait(); //This cannot be an await, or we'll get later messages before we're ready
							var data = e.Payload.ToObject<ReadyEvent>(_dataSocketSerializer);
							_privateUser = _users.GetOrAdd(data.User.Id, null);
							_privateUser.Update(data.User);
							_privateUser.Global.Update(data.User);
                            foreach (var model in data.Guilds)
							{
								if (model.Unavailable != true)
								{
									var server = _servers.GetOrAdd(model.Id);
									server.Update(model);
								}
							}
							foreach (var model in data.PrivateChannels)
							{
								var user = _users.GetOrAdd(model.Recipient.Id, null);
								user.Update(model.Recipient);
								var channel = _channels.GetOrAdd(model.Id, null, user.Id);
								channel.Update(model);
							}
						}
						break;

					//Servers
					case "GUILD_CREATE":
						{
							var data = e.Payload.ToObject<GuildCreateEvent>(_dataSocketSerializer);
							if (data.Unavailable != true)
							{
								var server = _servers.GetOrAdd(data.Id);
								server.Update(data);
								if (data.Unavailable == false)
									RaiseServerAvailable(server);
								else
									RaiseJoinedServer(server);
							}
						}
						break;
					case "GUILD_UPDATE":
						{
							var data = e.Payload.ToObject<GuildUpdateEvent>(_dataSocketSerializer);
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
							var data = e.Payload.ToObject<GuildDeleteEvent>(_dataSocketSerializer);
							var server = _servers.TryRemove(data.Id);
							if (server != null)
							{
								if (data.Unavailable == true)
									RaiseServerUnavailable(server);
								else
									RaiseLeftServer(server);
							}
						}
						break;

					//Channels
					case "CHANNEL_CREATE":
						{
							var data = e.Payload.ToObject<ChannelCreateEvent>(_dataSocketSerializer);
							Channel channel;
							if (data.IsPrivate)
							{
								var user = _users.GetOrAdd(data.Recipient.Id, null);
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
							var data = e.Payload.ToObject<ChannelUpdateEvent>(_dataSocketSerializer);
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
							var data = e.Payload.ToObject<ChannelDeleteEvent>(_dataSocketSerializer);
							var channel = _channels.TryRemove(data.Id);
							if (channel != null)
								RaiseChannelDestroyed(channel);
						}
						break;

					//Members
					case "GUILD_MEMBER_ADD":
						{
							var data = e.Payload.ToObject<MemberAddEvent>(_dataSocketSerializer);
							var user = _users.GetOrAdd(data.User.Id, data.GuildId);
							user.Update(data);
							if (Config.TrackActivity)
								user.UpdateActivity();
							RaiseUserJoined(user);
						}
						break;
					case "GUILD_MEMBER_UPDATE":
						{
							var data = e.Payload.ToObject<MemberUpdateEvent>(_dataSocketSerializer);
							var user = _users[data.User.Id, data.GuildId];
							if (user != null)
							{
								user.Update(data);
								RaiseUserUpdated(user);
							}
						}
						break;
					case "GUILD_MEMBER_REMOVE":
						{
							var data = e.Payload.ToObject<MemberRemoveEvent>(_dataSocketSerializer);
							var user = _users.TryRemove(data.UserId, data.GuildId);
							if (user != null)
								RaiseUserLeft(user);
						}
						break;
					case "GUILD_MEMBERS_CHUNK":
						{
							var data = e.Payload.ToObject<MembersChunkEvent>(_dataSocketSerializer);
							foreach (var memberData in data.Members)
							{
								var user = _users.GetOrAdd(memberData.User.Id, memberData.GuildId);
								user.Update(memberData);
								//RaiseUserAdded(user);
							}
						}
						break;

					//Roles
					case "GUILD_ROLE_CREATE":
						{
							var data = e.Payload.ToObject<RoleCreateEvent>(_dataSocketSerializer);
							var role = _roles.GetOrAdd(data.Data.Id, data.GuildId);
							role.Update(data.Data);
							var server = _servers[data.GuildId];
							if (server != null)
								server.AddRole(role);
							RaiseRoleUpdated(role);
						}
						break;
					case "GUILD_ROLE_UPDATE":
						{
							var data = e.Payload.ToObject<RoleUpdateEvent>(_dataSocketSerializer);
							var role = _roles[data.Data.Id];
							if (role != null)
							{
								role.Update(data.Data);
								RaiseRoleUpdated(role);
							}
						}
						break;
					case "GUILD_ROLE_DELETE":
						{
							var data = e.Payload.ToObject<RoleDeleteEvent>(_dataSocketSerializer);
							var role = _roles.TryRemove(data.RoleId);
							if (role != null)
							{
								RaiseRoleDeleted(role);
								var server = _servers[data.GuildId];
								if (server != null)
									server.RemoveRole(role);
							}
						}
						break;

					//Bans
					case "GUILD_BAN_ADD":
						{
							var data = e.Payload.ToObject<BanAddEvent>(_dataSocketSerializer);
							var server = _servers[data.GuildId];
							if (server != null)
							{
								var id = data.User?.Id ?? data.UserId;
                                server.AddBan(id);
								RaiseUserBanned(id, server);
							}
						}
						break;
					case "GUILD_BAN_REMOVE":
						{
							var data = e.Payload.ToObject<BanRemoveEvent>(_dataSocketSerializer);
							var server = _servers[data.GuildId];
							if (server != null)
							{
								var id = data.User?.Id ?? data.UserId;
								if (server.RemoveBan(id))
									RaiseUserUnbanned(id, server);
							}
						}
						break;

					//Messages
					case "MESSAGE_CREATE":
						{
							var data = e.Payload.ToObject<MessageCreateEvent>(_dataSocketSerializer);
							Message msg = null;

							bool isAuthor = data.Author.Id == _userId;

							if (msg == null)
								msg = _messages.GetOrAdd(data.Id, data.ChannelId, data.Author.Id);
							msg.Update(data);
							if (Config.TrackActivity)
							{
								var channel = msg.Channel;
								if (channel?.IsPrivate == false)
								{
									var user = msg.User;
									if (user != null)
										user.UpdateActivity(data.Timestamp);
								}
							}

							RaiseMessageReceived(msg);

							if (Config.AckMessages && !isAuthor)
								await _api.AckMessage(data.Id, data.ChannelId).ConfigureAwait(false);
						}
						break;
					case "MESSAGE_UPDATE":
						{
							var data = e.Payload.ToObject<MessageUpdateEvent>(_dataSocketSerializer);
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
							var data = e.Payload.ToObject<MessageDeleteEvent>(_dataSocketSerializer);
							var msg = _messages.TryRemove(data.Id);
							if (msg != null)
								RaiseMessageDeleted(msg);
						}
						break;
					case "MESSAGE_ACK":
						{
							var data = e.Payload.ToObject<MessageAckEvent>(_dataSocketSerializer);
							var msg = GetMessage(data.MessageId);
							if (msg != null)
								RaiseMessageReadRemotely(msg);
						}
						break;

					//Statuses
					case "PRESENCE_UPDATE":
						{
							var data = e.Payload.ToObject<PresenceUpdateEvent>(_dataSocketSerializer);
							var user = _users.GetOrAdd(data.User.Id, data.GuildId);
							if (user != null)
							{
								user.Update(data);
								RaiseUserPresenceUpdated(user);
							}
						}
						break;
					case "TYPING_START":
						{
							var data = e.Payload.ToObject<TypingStartEvent>(_dataSocketSerializer);
							var channel = _channels[data.ChannelId];
							if (channel != null)
							{
								var user = _users[data.UserId, channel.Server?.Id];
								if (user != null)
								{
									if (channel != null)
										RaiseUserIsTyping(user, channel);
								}

								if (Config.TrackActivity)
								{
									if (!channel.IsPrivate)
									{
										if (user != null)
											user.UpdateActivity();
									}
								}
							}
						}
						break;

					//Voice
					case "VOICE_STATE_UPDATE":
						{
							var data = e.Payload.ToObject<MemberVoiceStateUpdateEvent>(_dataSocketSerializer);
							var user = _users[data.UserId, data.GuildId];
							if (user != null)
							{
								var voiceChannel = user.VoiceChannel;
                                if (voiceChannel != null && data.ChannelId != voiceChannel.Id && user.IsSpeaking)
								{
									user.IsSpeaking = false;
									RaiseUserIsSpeaking(user, _channels[voiceChannel.Id], false);
								}
								user.Update(data);
								RaiseUserVoiceStateUpdated(user);
							}
						}
						break;

					//Settings
					case "USER_UPDATE":
						{
							var data = e.Payload.ToObject<UserUpdateEvent>(_dataSocketSerializer);
							var user = _globalUsers[data.Id];
							if (user != null)
							{
								user.Update(data);
								RaiseProfileUpdated();
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

					//Pass to DiscordWSClient
					case "VOICE_SERVER_UPDATE":
						await base.OnReceivedEvent(e).ConfigureAwait(false);
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

		private void SendInitialLog()
		{
			if (_config.LogLevel >= LogMessageSeverity.Verbose)
				RaiseOnLog(LogMessageSeverity.Verbose, LogMessageSource.Client, $"Config: {JsonConvert.SerializeObject(_config)}");
			_sentInitialLog = true;
        }
		
		public void GetCacheStats(out int serverCount, out int channelCount, out int userCount, out int uniqueUserCount, out int messageCount, out int roleCount)
		{
			serverCount = _servers.Count;
			channelCount = _channels.Count;
			userCount = _users.Count;
			uniqueUserCount = _globalUsers.Count;
			messageCount = _messages.Count;
			roleCount = _roles.Count;
		}
	}
}