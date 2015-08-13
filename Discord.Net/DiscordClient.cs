using Discord.API;
using Discord.API.Models;
using Discord.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
	public partial class DiscordClient
	{
		public const int ReconnectDelay = 1000; //Time in milliseconds to wait after an unexpected disconnect before retrying
		public const int FailedReconnectDelay = 10000; //Time in milliseconds to wait after a failed reconnect attempt

		private DiscordWebSocket _webSocket;
		private HttpOptions _httpOptions;
		private bool _isClosing, _isReady;

		public string UserId { get; private set; }
		public User User { get { return _users[UserId]; } }

		public IEnumerable<User> Users { get { return _users; } }
		private AsyncCache<User, API.Models.UserReference> _users;

		public IEnumerable<Server> Servers { get { return _servers; } }
		private AsyncCache<Server, API.Models.ServerReference> _servers;

		public IEnumerable<Channel> Channels { get { return _channels; } }
		private AsyncCache<Channel, API.Models.ChannelReference> _channels;

		public IEnumerable<Message> Messages { get { return _messages; } }
		private AsyncCache<Message, API.Models.MessageReference> _messages;

		public IEnumerable<Role> Roles { get { return _roles; } }
		private AsyncCache<Role, API.Models.Role> _roles;

		public bool IsConnected { get { return _isReady; } }

		public DiscordClient()
		{
			string version = typeof(DiscordClient).GetTypeInfo().Assembly.GetName().Version.ToString(2);
			_httpOptions = new HttpOptions($"Discord.Net/{version} (https://github.com/RogueException/Discord.Net)");

			_servers = new AsyncCache<Server, API.Models.ServerReference>(
				(key, parentKey) => new Server(key, this),
				(server, model) =>
				{
					server.Name = model.Name;
					if (!server.Channels.Any()) //Assume a default channel exists with the same id as the server. Not sure if this is safe?
					{
						var defaultChannel = new ChannelReference() { Id = server.DefaultChannelId, GuildId = server.Id };
						_channels.Update(defaultChannel.Id, defaultChannel.GuildId, defaultChannel);
					}
					if (model is ExtendedServerInfo)
					{
						var extendedModel = model as ExtendedServerInfo;
						server.AFKChannelId = extendedModel.AFKChannelId;
						server.AFKTimeout = extendedModel.AFKTimeout;
						server.JoinedAt = extendedModel.JoinedAt ?? DateTime.MinValue;
						server.OwnerId = extendedModel.OwnerId;
						server.Presence = extendedModel.Presence;
						server.Region = extendedModel.Region;
						server.VoiceStates = extendedModel.VoiceStates;

						foreach (var role in extendedModel.Roles)
							_roles.Update(role.Id, model.Id, role);
						foreach (var channel in extendedModel.Channels)
						{
							_channels.Update(channel.Id, model.Id, channel);
							if (channel.Type == ChannelTypes.Text)
							{
								try
								{
									var messages = DiscordAPI.GetMessages(channel.Id, _httpOptions).Result.OrderBy(x => x.Timestamp);
									foreach (var message in messages)
									{
										var msg = _messages.Update(message.Id, message.ChannelId, message);
										if (msg.User != null)
											msg.User.UpdateActivity(message.Timestamp);
									}
								}
								catch { } //Bad Permissions?
							}
						}
						foreach (var membership in extendedModel.Members)
						{
							_users.Update(membership.User.Id, membership.User);
							server.AddMember(membership.User.Id);
						}
					}
				},
				server => { }
			);

			_channels = new AsyncCache<Channel, API.Models.ChannelReference>(
				(key, parentKey) => new Channel(key, parentKey, this),
				(channel, model) =>
				{
					channel.Name = model.Name;
					channel.Type = model.Type;
					if (model is ChannelInfo)
					{
						var extendedModel = model as ChannelInfo;
						channel.PermissionOverwrites = extendedModel.PermissionOverwrites;
						channel.RecipientId = extendedModel.Recipient?.Id;
					}
				},
				channel => { });
			_messages = new AsyncCache<Message, API.Models.MessageReference>(
				(key, parentKey) => new Message(key, parentKey, this),
				(message, model) =>
				{
					if (model is API.Models.Message)
					{
						var extendedModel = model as API.Models.Message;
						message.Attachments = extendedModel.Attachments;
						message.Text = extendedModel.Content;
						message.Embeds = extendedModel.Embeds;
						message.IsMentioningEveryone = extendedModel.IsMentioningEveryone;
						message.IsTTS = extendedModel.IsTextToSpeech;
						message.UserId = extendedModel.Author.Id;
						message.Timestamp = extendedModel.Timestamp;
					}
					if (model is WebSocketEvents.MessageUpdate)
					{
						var extendedModel = model as WebSocketEvents.MessageUpdate;
						message.Embeds = extendedModel.Embeds;
					}
				},
				message => { }
			);
			_roles = new AsyncCache<Role, API.Models.Role>(
				(key, parentKey) => new Role(key, parentKey, this),
				(role, model) =>
				{
					role.Permissions = model.Permissions;
				},
				role => { }
			);
			_users = new AsyncCache<User, API.Models.UserReference>(
				(key, parentKey) => new User(key, this),
				(user, model) =>
				{
					user.AvatarId = model.Avatar;
					user.Discriminator = model.Discriminator;
					user.Name = model.Username;
					if (model is SelfUserInfo)
					{
						var extendedModel = model as SelfUserInfo;
						user.Email = extendedModel.Email;
						user.IsVerified = extendedModel.IsVerified;
					}
					if (model is PresenceUserInfo)
					{
						var extendedModel = model as PresenceUserInfo;
						user.GameId = extendedModel.GameId;
						user.Status = extendedModel.Status;
					}
				},
				user => { }
			);

			_webSocket = new DiscordWebSocket();
			_webSocket.Connected += (s, e) => RaiseConnected();
			_webSocket.Disconnected += async (s, e) =>
			{
				//Reconnect if we didn't cause the disconnect
				RaiseDisconnected();
				while (!_isClosing)
				{
					try
					{
						await Task.Delay(ReconnectDelay);
						await _webSocket.ConnectAsync(Endpoints.WebSocket_Hub, _httpOptions);
						break;
					}
					catch (Exception)
					{
						//Net is down? We can keep trying to reconnect until the user runs Disconnect()
						await Task.Delay(FailedReconnectDelay);
					}
				}
			};
			_webSocket.GotEvent += (s, e) =>
			{
				switch (e.Type)
				{
					//Global
					case "READY": //Resync
						{
							var data = e.Event.ToObject<WebSocketEvents.Ready>();

							_servers.Clear();
							_channels.Clear();
							_users.Clear();

							UserId = data.User.Id;
							_users.Update(data.User.Id, data.User);
							foreach (var server in data.Guilds)
								_servers.Update(server.Id, server);
							foreach (var channel in data.PrivateChannels)
								_channels.Update(channel.Id, null, channel);
						}
						break;

					//Servers
					case "GUILD_CREATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildCreate>();
							var server = _servers.Update(data.Id, data);
							RaiseServerCreated(server);
						}
						break;
					case "GUILD_UPDATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildUpdate>();
							var server = _servers.Update(data.Id, data);
							RaiseServerUpdated(server);
						}
						break;
					case "GUILD_DELETE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildDelete>();
							var server = _servers.Remove(data.Id);
							if (server != null)
								RaiseServerDestroyed(server);
						}
						break;

					//Channels
					case "CHANNEL_CREATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.ChannelCreate>();
							var channel = _channels.Update(data.Id, data.GuildId, data);
							RaiseChannelCreated(channel);
						}
						break;
					case "CHANNEL_UPDATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.ChannelUpdate>();
							var channel = _channels.Update(data.Id, data.GuildId, data);
							RaiseChannelUpdated(channel);
						}
						break;
					case "CHANNEL_DELETE":
						{
							var data = e.Event.ToObject<WebSocketEvents.ChannelDelete>();
							var channel = _channels.Remove(data.Id);
							if (channel != null)
								RaiseChannelDestroyed(channel);
						}
						break;

					//Members
					case "GUILD_MEMBER_ADD":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildMemberAdd>();
							var user = _users.Update(data.User.Id, data.User);
							var server = _servers[data.GuildId];
							server._members[user.Id] = true;
							RaiseMemberAdded(user, server);
						}
						break;
					case "GUILD_MEMBER_UPDATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildMemberUpdate>();
							var user = _users.Update(data.User.Id, data.User);
							var server = _servers[data.GuildId];
							RaiseMemberUpdated(user, server);
						}
						break;
					case "GUILD_MEMBER_REMOVE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildMemberRemove>();
							var user = _users.Update(data.User.Id, data.User);
							var server = _servers[data.GuildId];
							if (server != null && server.RemoveMember(user.Id))
								RaiseMemberRemoved(user, server);
						}
						break;

					//Roles
					case "GUILD_ROLE_CREATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildRoleCreateUpdate>();
							var role = _roles.Update(data.Role.Id, data.Role);
							RaiseRoleCreated(role);
						}
						break;
					case "GUILD_ROLE_UPDATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildRoleCreateUpdate>();
							var role = _roles.Update(data.Role.Id, data.Role);
							RaiseRoleUpdated(role);
						}
						break;
					case "GUILD_ROLE_DELETE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildRoleDelete>();
							var role = _roles.Remove(data.RoleId);
							if (role != null)
								RaiseRoleDeleted(role);
						}
						break;

					//Bans
					case "GUILD_BAN_ADD":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildBanAddRemove>();
							var user = _users.Update(data.User.Id, data.User);
							var server = _servers[data.GuildId];
							RaiseBanAdded(user, server);
						}
						break;
					case "GUILD_BAN_REMOVE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildBanAddRemove>();
							var user = _users.Update(data.User.Id, data.User);
							var server = _servers[data.GuildId];
							if (server != null && server.RemoveBan(user.Id))
								RaiseBanRemoved(user, server);
						}
						break;

					//Messages
					case "MESSAGE_CREATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.MessageCreate>();
							var msg = _messages.Update(data.Id, data.ChannelId, data);
							msg.User.UpdateActivity(data.Timestamp);
							RaiseMessageCreated(msg);
						}
						break;
					case "MESSAGE_UPDATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.MessageUpdate>();
							var msg = _messages.Update(data.Id, data.ChannelId, data);
							RaiseMessageUpdated(msg);
						}
						break;
					case "MESSAGE_DELETE":
						{
							var data = e.Event.ToObject<WebSocketEvents.MessageDelete>();
							var msg = GetMessage(data.MessageId);
							if (msg != null)
								_messages.Remove(msg.Id);
						}
						break;
					case "MESSAGE_ACK":
						{
							var data = e.Event.ToObject<WebSocketEvents.MessageAck>();
							var msg = GetMessage(data.MessageId);
							RaiseMessageAcknowledged(msg);
						}
						break;

					//Statuses
					case "PRESENCE_UPDATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.PresenceUpdate>();
							var user = _users.Update(data.Id, data);
							RaisePresenceUpdated(user);
						}
						break;
					case "VOICE_STATE_UPDATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.VoiceStateUpdate>();
							var user = _users[data.UserId]; //TODO: Don't ignore this
							RaiseVoiceStateUpdated(user);
						}
						break;
					case "TYPING_START":
						{
							var data = e.Event.ToObject<WebSocketEvents.TypingStart>();
							var channel = _channels[data.ChannelId];
							var user = _users[data.UserId];
							RaiseUserTyping(user, channel);
						}
						break;

					//Voice
					case "VOICE_SERVER_UPDATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.VoiceServerUpdate>();
							var server = _servers[data.ServerId];
							RaiseVoiceServerUpdated(server, data.Endpoint);
						}
						break;

					//Others
					default:
						RaiseOnDebugMessage("Unknown WebSocket message type: " + e.Type);
						break;
				}
			};
			_webSocket.OnDebugMessage += (s, e) => RaiseOnDebugMessage(e.Message);
		}

		//Collections
		public User GetUser(string id) => _users[id];
		public User FindUser(string name)
		{
			return _users
				.Where(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase))
				.FirstOrDefault();
		}
		public User FindUser(string name, string discriminator)
		{
			return _users
				.Where(x => 
					string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase) &&
					x.Discriminator == discriminator
                )
				.FirstOrDefault();
		}
		public User FindChannelUser(Channel channel, string name)
			=> FindChannelUser(channel.Id, name);
        public User FindChannelUser(string channelId, string name)
		{
			return _users
				.Where(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase))
				.FirstOrDefault();
		}

		public Server GetServer(string id) => _servers[id];
		public Server FindServer(string name)
		{
			return _servers
				.Where(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase))
				.FirstOrDefault();
		}

		public Channel GetChannel(string id) => _channels[id];
		public Channel FindChannel(string name)
		{
			return _channels
				.Where(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase))
				.FirstOrDefault();
		}
		public Channel FindChannel(Server server, string name)
			=> FindChannel(server.Id, name);
        public Channel FindChannel(string serverId, string name)
		{
			return _channels
				.Where(x =>
					string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase) &&
					x.ServerId == serverId
				)
				.FirstOrDefault();
		}

		public Message GetMessage(string id) => _messages[id];
		public Role GetRole(string id) => _roles[id];

		//Auth
		public async Task Connect(string email, string password)
		{
            _isClosing = false;
			var response = await DiscordAPI.Login(email, password, _httpOptions);
			_httpOptions.Token = response.Token;
			await _webSocket.ConnectAsync(Endpoints.WebSocket_Hub, _httpOptions);
			_isReady = true;
        }
		public async Task ConnectAnonymous(string username)
		{
			_isClosing = false;
			var response = await DiscordAPI.LoginAnonymous(username, _httpOptions);
			_httpOptions.Token = response.Token;
			await _webSocket.ConnectAsync(Endpoints.WebSocket_Hub, _httpOptions);
			_isReady = true;
		}
		public async Task Disconnect()
		{
			_isReady = false;
			_isClosing = true;
			await _webSocket.DisconnectAsync();
			_isClosing = false;

			_channels.Clear();
			_messages.Clear();
			_roles.Clear();
			_servers.Clear();
			_users.Clear();
		}

		//Servers
		public async Task<Server> CreateServer(string name, string region)
		{
			CheckReady();
			var response = await DiscordAPI.CreateServer(name, region, _httpOptions);
			return _servers.Update(response.Id, response);
		}
		public Task<Server> LeaveServer(Server server)
			=> LeaveServer(server.Id);
		public async Task<Server> LeaveServer(string serverId)
		{
			CheckReady();
			try
			{
				await DiscordAPI.LeaveServer(serverId, _httpOptions);
			}
			catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) {}
			return _servers.Remove(serverId);
		}

		//Channels
		public Task<Channel> CreateChannel(Server server, string name, string region)
			=> CreateChannel(server.Id, name, region);
        public async Task<Channel> CreateChannel(string serverId, string name, string region)
		{
			CheckReady();
			var response = await DiscordAPI.CreateChannel(serverId, name, region, _httpOptions);
			return _channels.Update(response.Id, response);
		}
		public Task<Channel> CreatePMChannel(User user)
			=> CreatePMChannel(user.Id);
		public async Task<Channel> CreatePMChannel(string recipientId)
		{
			CheckReady();
			var response = await DiscordAPI.CreatePMChannel(UserId, recipientId, _httpOptions);
			return _channels.Update(response.Id, response);
		}
		public Task<Channel> DestroyChannel(Channel channel)
			=> DestroyChannel(channel.Id);
        public async Task<Channel> DestroyChannel(string channelId)
		{
			CheckReady();
			try
			{
				var response = await DiscordAPI.DestroyChannel(channelId, _httpOptions);
			}
			catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) { }
			return _channels.Remove(channelId);
		}

		//Bans
		public Task Ban(Server server, User user)
			=> Ban(server.Id, user.Id);
		public Task Ban(Server server, string userId)
			=> Ban(server.Id, userId);
		public Task Ban(string server, User user)
			=> Ban(server, user.Id);
		public Task Ban(string serverId, string userId)
		{
			CheckReady();
			return DiscordAPI.Ban(serverId, userId, _httpOptions);
		}
		public Task Unban(Server server, User user)
			=> Unban(server.Id, user.Id);
		public Task Unban(Server server, string userId)
			=> Unban(server.Id, userId);
		public Task Unban(string server, User user)
			=> Unban(server, user.Id);
		public async Task Unban(string serverId, string userId)
		{
			CheckReady();
			try
			{
				await DiscordAPI.Unban(serverId, userId, _httpOptions);
			}
			catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) { }
		}

		//Invites
		public Task<Invite> CreateInvite(Server server, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass)
		{
			return CreateInvite(server.DefaultChannelId, maxAge, maxUses, isTemporary, hasXkcdPass);
		}
		public Task<Invite> CreateInvite(Channel channel, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass)
		{
			return CreateInvite(channel, maxAge, maxUses, isTemporary, hasXkcdPass);
		}
        public async Task<Invite> CreateInvite(string channelId, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass)
		{
			CheckReady();
			var response = await DiscordAPI.CreateInvite(channelId, maxAge, maxUses, isTemporary, hasXkcdPass, _httpOptions);
			_channels.Update(response.Channel.Id, response.Server.Id, response.Channel);
			_servers.Update(response.Server.Id, response.Server);
			_users.Update(response.Inviter.Id, response.Inviter);
			return new Invite(response.Code, response.XkcdPass, this)
			{
				ChannelId = response.Channel.Id,
				InviterId = response.Inviter.Id,
				ServerId = response.Server.Id,
				IsRevoked = response.IsRevoked,
				IsTemporary = response.IsTemporary,
				MaxAge = response.MaxAge,
				MaxUses = response.MaxUses,
				Uses = response.Uses
			};
		}
        public async Task<Invite> GetInvite(string id)
		{
			CheckReady();
			var response = await DiscordAPI.GetInvite(id, _httpOptions);
			return new Invite(response.Code, response.XkcdPass, this)
			{
				ChannelId = response.Channel.Id,
				InviterId = response.Inviter.Id,
				ServerId = response.Server.Id
			};
		}
		public Task AcceptInvite(Invite invite)
		{
			CheckReady();
			return DiscordAPI.AcceptInvite(invite.Code, _httpOptions);
		}
		public async Task AcceptInvite(string id)
		{
			CheckReady();
			//Check if this is a human-readable link and get its ID
			var response = await DiscordAPI.GetInvite(id, _httpOptions);
			await DiscordAPI.AcceptInvite(response.Code, _httpOptions);
		}
		public async Task DeleteInvite(string id)
		{
			CheckReady();
			try
			{
				//Check if this is a human-readable link and get its ID
				var response = await DiscordAPI.GetInvite(id, _httpOptions);
				await DiscordAPI.DeleteInvite(response.Code, _httpOptions);
			}
			catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) { }
		}

		//Chat
		public Task SendMessage(Channel channel, string text)
			=> SendMessage(channel.Id, text, new string[0]);
		public Task SendMessage(string channelId, string text)
			=> SendMessage(channelId, text, new string[0]);
		public Task SendMessage(Channel channel, string text, string[] mentions)
			=> SendMessage(channel, text, mentions);
        public async Task SendMessage(string channelId, string text, string[] mentions)
		{
			CheckReady();
			if (text.Length <= 2000)
			{
				var msg = await DiscordAPI.SendMessage(channelId, text, mentions, _httpOptions);
				_messages.Update(msg.Id, channelId, msg);
            }
			else
			{
				int blockCount = (int)Math.Ceiling(text.Length / (double)DiscordAPI.MaxMessageSize);
				for (int i = 0; i < blockCount; i++)
				{
					int index = i * DiscordAPI.MaxMessageSize;
					var msg = await DiscordAPI.SendMessage(channelId, text.Substring(index, Math.Min(2000, text.Length - index)), mentions, _httpOptions);
					_messages.Update(msg.Id, channelId, msg);
					await Task.Delay(1000);
				}
			}
		}

		public Task DeleteMessage(Message msg)
			=> DeleteMessage(msg.ChannelId, msg.Id);
		public async Task DeleteMessage(string channelId, string msgId)
		{
			try
			{
				await DiscordAPI.DeleteMessage(channelId, msgId, _httpOptions);
			}
			catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) { }
		}

		//Voice
		public Task Mute(Server server, User user)
			=> Mute(server.Id, user.Id);
		public Task Mute(Server server, string userId)
			=> Mute(server.Id, userId);
		public Task Mute(string server, User user)
			=> Mute(server, user.Id);
		public Task Mute(string serverId, string userId)
		{
			CheckReady();
			return DiscordAPI.Mute(serverId, userId, _httpOptions);
		}

		public Task Unmute(Server server, User user)
			=> Unmute(server.Id, user.Id);
		public Task Unmute(Server server, string userId)
			=> Unmute(server.Id, userId);
		public Task Unmute(string server, User user)
			=> Unmute(server, user.Id);
		public Task Unmute(string serverId, string userId)
		{
			CheckReady();
			return DiscordAPI.Unmute(serverId, userId, _httpOptions);
		}

		public Task Deafen(Server server, User user)
			=> Deafen(server.Id, user.Id);
		public Task Deafen(Server server, string userId)
			=> Deafen(server.Id, userId);
		public Task Deafen(string server, User user)
			=> Deafen(server, user.Id);
		public Task Deafen(string serverId, string userId)
		{
			CheckReady();
			return DiscordAPI.Deafen(serverId, userId, _httpOptions);
		}

		public Task Undeafen(Server server, User user)
			=> Undeafen(server.Id, user.Id);
		public Task Undeafen(Server server, string userId)
			=> Undeafen(server.Id, userId);
		public Task Undeafen(string server, User user)
			=> Undeafen(server, user.Id);
		public Task Undeafen(string serverId, string userId)
		{
			CheckReady();
			return DiscordAPI.Undeafen(serverId, userId, _httpOptions);
		}

		private void CheckReady()
		{
			if (!_isReady)
				throw new InvalidOperationException("The client is not currently connected to Discord");
        }
		public void Block()
		{
			//Blocking call for console apps
			//TODO: Improve this
			while (!_isClosing)
				Thread.Sleep(1000);
		}
	}
}
