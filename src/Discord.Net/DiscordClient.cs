using Discord.API;
using Discord.API.Models;
using Discord.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
	/// <summary> Provides a connection to the DiscordApp service. </summary>
	public partial class DiscordClient
	{
		private readonly DiscordClientConfig _config;
		private readonly DiscordTextWebSocket _webSocket;
#if !DNXCORE50
		private readonly DiscordVoiceSocket _voiceWebSocket;
#endif
		private readonly ManualResetEventSlim _blockEvent;
		private readonly Regex _userRegex, _channelRegex;
		private readonly MatchEvaluator _userRegexEvaluator, _channelRegexEvaluator;
		private readonly JsonSerializer _serializer;
		private readonly Random _rand;

		private volatile Task _tasks;		

		/// <summary> Returns the User object for the current logged in user. </summary>
		public User User { get; private set; }
		/// <summary> Returns the id of the current logged in user. </summary>
		public string UserId { get; private set; }
		public string SessionId { get; private set; }

		/// <summary> Returns a collection of all users the client can see across all servers. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		public IEnumerable<User> Users => _users;
		private readonly AsyncCache<User, API.Models.UserReference> _users;

		/// <summary> Returns a collection of all servers the client is a member of. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		public IEnumerable<Server> Servers => _servers;
		private readonly AsyncCache<Server, API.Models.ServerReference> _servers;

		/// <summary> Returns a collection of all channels the client can see across all servers. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		public IEnumerable<Channel> Channels => _channels;
		private readonly AsyncCache<Channel, API.Models.ChannelReference> _channels;

		/// <summary> Returns a collection of all messages the client has in cache. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		public IEnumerable<Message> Messages => _messages;
		private readonly AsyncCache<Message, API.Models.MessageReference> _messages;
		private readonly ConcurrentQueue<Message> _pendingMessages;

		/// <summary> Returns a collection of all roles the client can see across all servers. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		public IEnumerable<Role> Roles => _roles;
		private readonly AsyncCache<Role, API.Models.Role> _roles;

#if !DNXCORE50
		private string _currentVoiceServerId, _currentVoiceEndpoint, _currentVoiceToken;
		public string CurrentVoiceServerId { get { return _currentVoiceEndpoint != null ? _currentVoiceToken : null; } }
		public Server CurrentVoiceServer => _servers[CurrentVoiceServerId];
#endif
		/// <summary> Returns true if the user has successfully logged in and the websocket connection has been established. </summary>
		public bool IsConnected => _isConnected;
		private bool _isConnected;

		private volatile CancellationTokenSource _disconnectToken;
		/// <summary> Returns true if this client was requested to disconnect. </summary>
		public bool IsClosing => _disconnectToken.IsCancellationRequested;
		/// <summary> Returns a cancel token that is triggered when a disconnect is requested. </summary>
		public CancellationToken CloseToken => _disconnectToken.Token;

		/// <summary> Initializes a new instance of the DiscordClient class. </summary>
		public DiscordClient(DiscordClientConfig config = null)
		{
			_blockEvent = new ManualResetEventSlim(true);

			_config = config ?? new DiscordClientConfig();
            _rand = new Random();

			_serializer = new JsonSerializer();
#if TEST_RESPONSES
			_serializer.CheckAdditionalContent = true;
			_serializer.MissingMemberHandling = MissingMemberHandling.Error;
#endif

			_userRegex = new Regex(@"<@\d+?>", RegexOptions.Compiled);
			_channelRegex = new Regex(@"<#\d+?>", RegexOptions.Compiled);
			_userRegexEvaluator = new MatchEvaluator(e =>
			{
				string id = e.Value.Substring(2, e.Value.Length - 3);
				var user = _users[id];
				if (user != null)
					return '@' + user.Name;
				else //User not found
					return e.Value;
			});
			_channelRegexEvaluator = new MatchEvaluator(e =>
			{
				string id = e.Value.Substring(2, e.Value.Length - 3);
				var channel = _channels[id];
				if (channel != null)
					return channel.Name;
				else //Channel not found
					return e.Value;
			});

			_servers = new AsyncCache<Server, API.Models.ServerReference>(
				(key, parentKey) => new Server(key, this),
				(server, model) =>
				{
					server.Name = model.Name;
					_channels.Update(server.DefaultChannelId, server.Id, null);
					if (model is ExtendedServerInfo)
					{
						var extendedModel = model as ExtendedServerInfo;
						server.AFKChannelId = extendedModel.AFKChannelId;
						server.AFKTimeout = extendedModel.AFKTimeout;
						server.JoinedAt = extendedModel.JoinedAt ?? DateTime.MinValue;
						server.OwnerId = extendedModel.OwnerId;
						server.Region = extendedModel.Region;

						foreach (var role in extendedModel.Roles)
							_roles.Update(role.Id, model.Id, role);
						foreach (var channel in extendedModel.Channels)
							_channels.Update(channel.Id, model.Id, channel);
						foreach (var membership in extendedModel.Members)
						{
							_users.Update(membership.User.Id, membership.User);
							server.UpdateMember(membership);
						}
						foreach (var membership in extendedModel.VoiceStates)
							server.UpdateMember(membership);
						foreach (var membership in extendedModel.Presences)
							server.UpdateMember(membership);
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
						channel.Position = extendedModel.Position;

						if (extendedModel.IsPrivate)
						{
							var user = _users.Update(extendedModel.Recipient.Id, extendedModel.Recipient);
							channel.RecipientId = user.Id;
							user.PrivateChannelId = channel.Id;
						}

						if (extendedModel.PermissionOverwrites != null)
						{
							channel.PermissionOverwrites = extendedModel.PermissionOverwrites.Select(x => new Channel.PermissionOverwrite
							{
								Type = x.Type,
								Id = x.Id,
								Deny = new PackedPermissions(x.Deny),
								Allow = new PackedPermissions(x.Allow)
							}).ToArray();
						}
						else
							channel.PermissionOverwrites = null;
					}
				},
				channel =>
				{
					if (channel.IsPrivate)
					{
						var user = channel.Recipient;
						if (user.PrivateChannelId == channel.Id)
							user.PrivateChannelId = null;
					}
				});
			_messages = new AsyncCache<Message, API.Models.MessageReference>(
				(key, parentKey) => new Message(key, parentKey, this),
				(message, model) =>
				{
					if (model is API.Models.Message)
					{
						var extendedModel = model as API.Models.Message;
						if (extendedModel.Attachments != null)
						{
							message.Attachments = extendedModel.Attachments.Select(x => new Message.Attachment
							{
								Id = x.Id,
								Url = x.Url,
								ProxyUrl = x.ProxyUrl,
								Size = x.Size,
								Filename = x.Filename,
								Width = x.Width,
								Height = x.Height
							}).ToArray();
						}
						else
							message.Attachments = new Message.Attachment[0];
						if (extendedModel.Embeds != null)
						{
							message.Embeds = extendedModel.Embeds.Select(x =>
							{
								var embed = new Message.Embed
								{
									Url = x.Url,
									Type = x.Type,
									Description = x.Description,
									Title = x.Title
								};
								if (x.Provider != null)
								{
									embed.Provider = new Message.EmbedReference
									{
										Url = x.Provider.Url,
										Name = x.Provider.Name
									};
								}
								if (x.Author != null)
								{
									embed.Author = new Message.EmbedReference
									{
										Url = x.Author.Url,
										Name = x.Author.Name
									};
								}
								if (x.Thumbnail != null)
								{
									embed.Thumbnail = new Message.File
									{
										Url = x.Thumbnail.Url,
										ProxyUrl = x.Thumbnail.ProxyUrl,
										Width = x.Thumbnail.Width,
										Height = x.Thumbnail.Height
									};
								}
								return embed;
							}).ToArray();
						}
						else
							message.Embeds = new Message.Embed[0];
						message.IsMentioningEveryone = extendedModel.IsMentioningEveryone;
						message.IsTTS = extendedModel.IsTextToSpeech;
						message.MentionIds = extendedModel.Mentions?.Select(x => x.Id)?.ToArray() ?? new string[0];
						message.IsMentioningMe = message.MentionIds.Contains(UserId);
						message.RawText = extendedModel.Content;
						message.Timestamp = extendedModel.Timestamp;
						message.EditedTimestamp = extendedModel.EditedTimestamp;
						if (extendedModel.Author != null)
							message.UserId = extendedModel.Author.Id;
					}
				},
				message => { }
			);
			_pendingMessages = new ConcurrentQueue<Message>();
			_roles = new AsyncCache<Role, API.Models.Role>(
				(key, parentKey) => new Role(key, parentKey, this),
				(role, model) =>
				{
					role.Name = model.Name;
					role.Permissions.RawValue = (uint)model.Permissions;
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
				},
				user => { }
			);

			_webSocket = new DiscordTextWebSocket(this, _config.ConnectionTimeout, _config.WebSocketInterval);
			_webSocket.Connected += (s, e) => RaiseConnected();
			_webSocket.Disconnected += async (s, e) => 
			{
				//Reconnect if we didn't cause the disconnect
				RaiseDisconnected();
				while (!_disconnectToken.IsCancellationRequested)
				{
					try
					{
						await Task.Delay(_config.ReconnectDelay);
						await _webSocket.ConnectAsync(Endpoints.WebSocket_Hub);
						await _webSocket.Login();
						break;
					}
					catch (Exception ex)
					{
						RaiseOnDebugMessage($"Reconnect Failed: {ex.Message}");
						//Net is down? We can keep trying to reconnect until the user runs Disconnect()
						await Task.Delay(_config.FailedReconnectDelay);
					}
				}
			};
			_webSocket.OnDebugMessage += (s, e) => RaiseOnDebugMessage(e.Message);

#if !DNXCORE50
			if (_config.EnableVoice)
			{
				_voiceWebSocket = new DiscordVoiceSocket(this, _config.VoiceConnectionTimeout, _config.WebSocketInterval);
				_voiceWebSocket.Connected += (s, e) => RaiseVoiceConnected();
				_voiceWebSocket.Disconnected += (s, e) =>
				{
				//TODO: Reconnect if we didn't cause the disconnect
				RaiseVoiceDisconnected();
				};
				_voiceWebSocket.OnDebugMessage += (s, e) => RaiseOnVoiceDebugMessage(e.Message);
			}
#endif

#pragma warning disable CS1998 //Disable unused async keyword warning
			_webSocket.GotEvent += async (s, e) =>
			{
				switch (e.Type)
				{
					//Global
					case "READY": //Resync
						{
							var data = e.Event.ToObject<TextWebSocketEvents.Ready>(_serializer);

							_servers.Clear();
							_channels.Clear();
							_users.Clear();

							UserId = data.User.Id;
							SessionId = data.SessionId;
							User = _users.Update(data.User.Id, data.User);
							foreach (var server in data.Guilds)
								_servers.Update(server.Id, server);
							foreach (var channel in data.PrivateChannels)
								_channels.Update(channel.Id, null, channel);
						}
						break;

					//Servers
					case "GUILD_CREATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.GuildCreate>(_serializer);
							var server = _servers.Update(data.Id, data);
							try { RaiseServerCreated(server); } catch { }
						}
						break;
					case "GUILD_UPDATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.GuildUpdate>(_serializer);
							var server = _servers.Update(data.Id, data);
							try { RaiseServerUpdated(server); } catch { }
						}
						break;
					case "GUILD_DELETE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.GuildDelete>(_serializer);
							var server = _servers.Remove(data.Id);
							if (server != null)
								try { RaiseServerDestroyed(server); } catch { }
						}
						break;

					//Channels
					case "CHANNEL_CREATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.ChannelCreate>(_serializer);
							var channel = _channels.Update(data.Id, data.GuildId, data);
							try { RaiseChannelCreated(channel); } catch { }
						}
						break;
					case "CHANNEL_UPDATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.ChannelUpdate>(_serializer);
							var channel = _channels.Update(data.Id, data.GuildId, data);
							try { RaiseChannelUpdated(channel); } catch { }
						}
						break;
					case "CHANNEL_DELETE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.ChannelDelete>(_serializer);
							var channel = _channels.Remove(data.Id);
							if (channel != null)
								try { RaiseChannelDestroyed(channel); } catch { }
						}
						break;

					//Members
					case "GUILD_MEMBER_ADD":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.GuildMemberAdd>(_serializer);
							var user = _users.Update(data.User.Id, data.User);
							var server = _servers[data.ServerId];
							if (server != null)
							{
								var member = server.UpdateMember(data);
								try { RaiseMemberAdded(member); } catch { }
							}
						}
						break;
					case "GUILD_MEMBER_UPDATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.GuildMemberUpdate>(_serializer);
							var user = _users.Update(data.User.Id, data.User);
							var server = _servers[data.ServerId];
							if (server != null)
							{
								var member = server.UpdateMember(data);
								try { RaiseMemberUpdated(member); } catch { }
							}
						}
						break;
					case "GUILD_MEMBER_REMOVE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.GuildMemberRemove>(_serializer);
							var server = _servers[data.ServerId];
							if (server != null)
							{
								var member = server.RemoveMember(data.User.Id);
								if (member != null)
									try { RaiseMemberRemoved(member); } catch { }
							}
						}
						break;

					//Roles
					case "GUILD_ROLE_CREATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.GuildRoleCreateUpdate>(_serializer);
							var role = _roles.Update(data.Role.Id, data.ServerId, data.Role);
							try { RaiseRoleCreated(role); } catch { }
						}
						break;
					case "GUILD_ROLE_UPDATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.GuildRoleCreateUpdate>(_serializer);
							var role = _roles.Update(data.Role.Id, data.ServerId, data.Role);
							try { RaiseRoleUpdated(role); } catch { }
						}
						break;
					case "GUILD_ROLE_DELETE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.GuildRoleDelete>(_serializer);
							var role = _roles.Remove(data.RoleId);
							if (role != null)
								try { RaiseRoleDeleted(role); } catch { }
						}
						break;

					//Bans
					case "GUILD_BAN_ADD":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.GuildBanAddRemove>(_serializer);
							var user = _users.Update(data.User.Id, data.User);
							var server = _servers[data.ServerId];
							try { RaiseBanAdded(user, server); } catch { }
						}
						break;
					case "GUILD_BAN_REMOVE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.GuildBanAddRemove>(_serializer);
							var user = _users.Update(data.User.Id, data.User);
							var server = _servers[data.ServerId];
							if (server != null && server.RemoveBan(user.Id))
							{
								try { RaiseBanRemoved(user, server); } catch { }
							}
						}
						break;

					//Messages
					case "MESSAGE_CREATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.MessageCreate>(_serializer);
							Message msg = null;
							bool wasLocal = _config.UseMessageQueue && data.Author.Id == UserId && data.Nonce != null;
                            if (wasLocal)
							{
								msg = _messages.Remap("nonce" + data.Nonce, data.Id);
								if (msg != null)
								{
									msg.IsQueued = false;
									msg.Id = data.Id;
								}
							}
							msg = _messages.Update(data.Id, data.ChannelId, data);
							msg.User.UpdateActivity(data.Timestamp);
							if (wasLocal)
							{
								try { RaiseMessageSent(msg); } catch { }
							}
							try { RaiseMessageCreated(msg); } catch { }
						}
						break;
					case "MESSAGE_UPDATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.MessageUpdate>(_serializer);
							var msg = _messages.Update(data.Id, data.ChannelId, data);
							try { RaiseMessageUpdated(msg); } catch { }
						}
						break;
					case "MESSAGE_DELETE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.MessageDelete>(_serializer);
							var msg = GetMessage(data.MessageId);
							if (msg != null)
							{
								_messages.Remove(msg.Id);
								try { RaiseMessageDeleted(msg); } catch { }
							}
						}
						break;
					case "MESSAGE_ACK":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.MessageAck>(_serializer);
							var msg = GetMessage(data.MessageId);
							if (msg != null)
								try { RaiseMessageRead(msg); } catch { }
						}
						break;

					//Statuses
					case "PRESENCE_UPDATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.PresenceUpdate>(_serializer);
							var user = _users.Update(data.User.Id, data.User);
							var server = _servers[data.ServerId];
							if (server != null)
							{
								var member = server.UpdateMember(data);
								try { RaisePresenceUpdated(member); } catch { }
							}
						}
						break;
					case "VOICE_STATE_UPDATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.VoiceStateUpdate>(_serializer);
							var server = _servers[data.ServerId];
							if (server != null)
							{
								var member = server.UpdateMember(data);
								if (member != null)
									try { RaiseVoiceStateUpdated(member); } catch { }
							}
						}
						break;
					case "TYPING_START":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.TypingStart>(_serializer);
							var channel = _channels[data.ChannelId];
							var user = _users[data.UserId];
							if (user != null)
							{
								user.UpdateActivity(DateTime.UtcNow);
								if (channel != null)
									try { RaiseUserTyping(user, channel); } catch { }
							}
						}
						break;

					//Voice
					case "VOICE_SERVER_UPDATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.VoiceServerUpdate>(_serializer);
							var server = _servers[data.ServerId];
							server.VoiceServer = data.Endpoint;
                            try { RaiseVoiceServerUpdated(server, data.Endpoint); } catch { }

#if !DNXCORE50
							if (_config.EnableVoice && data.ServerId == _currentVoiceServerId)
							{
								_currentVoiceEndpoint = data.Endpoint.Split(':')[0];
								_currentVoiceToken = data.Token;
								await _voiceWebSocket.ConnectAsync(_currentVoiceEndpoint);
								await _voiceWebSocket.Login(_currentVoiceServerId, UserId, SessionId, _currentVoiceToken);
							}
#endif
						}
						break;

					//Settings
					case "USER_UPDATE":
						{
							var data = e.Event.ToObject<TextWebSocketEvents.UserUpdate>(_serializer);
							var user = _users.Update(data.Id, data);
							try { RaiseUserUpdated(user); } catch { }
						}
						break;
					case "USER_SETTINGS_UPDATE":
						{
							//TODO: Process this
						}
						break;

					//Others
					default:
						RaiseOnDebugMessage("Unknown WebSocket message type: " + e.Type);
						break;
				}
			};
		}
#pragma warning restore CS1998 //Restore unused async keyword warning

		private async Task SendAsync()
		{
			var cancelToken = _disconnectToken.Token;
			try
			{
				Message msg;
				while (!cancelToken.IsCancellationRequested)
				{
					while (_pendingMessages.TryDequeue(out msg))
					{
						bool hasFailed = false;
						APIResponses.SendMessage apiMsg = null;
                        try
						{
							apiMsg = await DiscordAPI.SendMessage(msg.ChannelId, msg.RawText, msg.MentionIds, msg.Nonce);
						}
						catch (WebException) { break; }
						catch (HttpException) { hasFailed = true; }

						if (!hasFailed)
						{
							_messages.Remap("nonce_", apiMsg.Id);
							_messages.Update(msg.Id, msg.ChannelId, apiMsg);
						}
						msg.IsQueued = false;
						msg.HasFailed = hasFailed;
						try { RaiseMessageSent(msg); } catch { }
					}
					await Task.Delay(_config.MessageQueueInterval);
				}
			}
			catch { }
			finally { _disconnectToken.Cancel(); }
		}
		private async Task EmptyAsync()
		{
			var cancelToken = _disconnectToken.Token;
			try
			{
				await Task.Delay(-1, cancelToken);
			}
			catch { }
		}

		/// <summary> Returns the user with the specified id, or null if none was found. </summary>
		public User GetUser(string id) => _users[id];
		/// <summary> Returns the user with the specified name and discriminator, or null if none was found. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public User GetUser(string name, string discriminator)
		{
			if (name.StartsWith("@"))
				name = name.Substring(1);

			return _users
				.Where(x =>
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) &&
					x.Discriminator == discriminator
				)
				.FirstOrDefault();
		}
		/// <summary> Returns all users with the specified name across all servers. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<User> FindUsers(string name)
		{
			if (name.StartsWith("@"))
			{
				string name2 = name.Substring(1);
				return _users.Where(x => 
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				return _users.Where(x => 
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			}
		}

		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Membership GetMember(string serverId, User user)
			=> GetMember(_servers[serverId], user.Id);
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Membership GetMember(string serverId, string userId)
			=> GetMember(_servers[serverId], userId);
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Membership GetMember(Server server, User user)
			=> GetMember(server, user.Id);
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Membership GetMember(Server server, string userId)
		{
			if (server == null)
				return null;
			return server.GetMember(userId);
		}

		/// <summary> Returns all users in with the specified server and name, along with their server-specific data. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive.</remarks>
		public IEnumerable<Membership> FindMembers(string serverId, string name)
			=> FindMembers(GetServer(serverId), name);
		/// <summary> Returns all users in with the specified server and name, along with their server-specific data. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive.</remarks>
		public IEnumerable<Membership> FindMembers(Server server, string name)
		{
			if (server == null)
				return new Membership[0];

			if (name.StartsWith("@"))
			{
				string name2 = name.Substring(1);
				return server.Members.Where(x =>
				{
					var user = x.User;
					return string.Equals(user.Name, name, StringComparison.OrdinalIgnoreCase) || string.Equals(user.Name, name2, StringComparison.OrdinalIgnoreCase);
				});
			}
			else
			{
				return server.Members.Where(x =>
					string.Equals(x.User.Name, name, StringComparison.OrdinalIgnoreCase));
			}
		}

		/// <summary> Returns the server with the specified id, or null if none was found. </summary>
		public Server GetServer(string id) => _servers[id];
		/// <summary> Returns all servers with the specified name. </summary>
		/// <remarks> Search is case-insensitive. </remarks>
		public IEnumerable<Server> FindServers(string name)
		{
			return _servers.Where(x => 
				string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary> Returns the channel with the specified id, or null if none was found. </summary>
		public Channel GetChannel(string id) => _channels[id];
		/// <summary> Returns a private channel with the provided user. </summary>
		public Task<Channel> GetPMChannel(string userId, bool createIfNotExists = false)
			=> GetPMChannel(_users[userId], createIfNotExists);
		/// <summary> Returns a private channel with the provided user. </summary>
		public async Task<Channel> GetPMChannel(User user, bool createIfNotExists = false)
		{
			var channel = user.PrivateChannel;
			if (channel == null && createIfNotExists)
				await CreatePMChannel(user);
            return channel;
		}
		/// <summary> Returns all channels with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and #Name. Search is case-insensitive. </remarks>
		public IEnumerable<Channel> FindChannels(Server server, string name)
			=> FindChannels(server.Id, name);
		/// <summary> Returns all channels with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and #Name. Search is case-insensitive. </remarks>
		public IEnumerable<Channel> FindChannels(string serverId, string name)
		{
			if (name.StartsWith("#"))
			{
				string name2 = name.Substring(1);
				return _channels.Where(x => x.ServerId == serverId &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				return _channels.Where(x => x.ServerId == serverId &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			}
		}

		/// <summary> Returns the role with the specified id, or null if none was found. </summary>
		public Role GetRole(string id) => _roles[id];
		/// <summary> Returns all roles with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<Role> FindRoles(Server server, string name)
			=> FindRoles(server.Id, name);
		/// <summary> Returns all roles with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<Role> FindRoles(string serverId, string name)
		{
			if (name.StartsWith("@"))
			{
				string name2 = name.Substring(1);
				return _roles.Where(x => x.ServerId == serverId &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				return _roles.Where(x => x.ServerId == serverId &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			}
		}

		/// <summary> Returns the message with the specified id, or null if none was found. </summary>
		public Message GetMessage(string id) => _messages[id];

		/// <summary> Downloads last count messages from the server, starting at beforeMessageId if it's provided. </summary>
		public Task<Message[]> DownloadMessages(Channel channel, int count, string beforeMessageId = null)
			=> DownloadMessages(channel.Id, count);
		/// <summary> Downloads last count messages from the server, starting at beforeMessageId if it's provided. </summary>
		public async Task<Message[]> DownloadMessages(string channelId, int count, string beforeMessageId = null)
		{
			Channel channel = GetChannel(channelId);
			if (channel != null && channel.Type == ChannelTypes.Text)
			{
				try
				{
					var msgs = await DiscordAPI.GetMessages(channel.Id, count);
					return msgs.OrderBy(x => x.Timestamp)
						.Select(x =>
						{
							var msg = _messages.Update(x.Id, x.ChannelId, x);
							var user = msg.User;
							if (user != null)
								user.UpdateActivity(x.Timestamp);
							return msg;
						})
						.ToArray();
				}
				catch (HttpException) { } //Bad Permissions?
			}
			return null;
		}

		//Auth
		/// <summary> Connects to the Discord server with the provided token. </summary>
		public Task<string> Connect(string token)
			=> ConnectInternal(null, null, token);
		/// <summary> Connects to the Discord server with the provided email and password. </summary>
		/// <returns> Returns a token for future connections. </returns>
		public Task<string> Connect(string email, string password)
			=> ConnectInternal(email, password, null);
		/// <summary> Connects to the Discord server with the provided token, and will fall back to username and password. </summary>
		/// <returns> Returns a token for future connections. </returns>
		public Task<string> Connect(string email, string password, string token)
			=> ConnectInternal(email, password, token);
		/// <summary> Connects to the Discord server as an anonymous user with the provided username. </summary>
		/// <returns> Returns a token for future connections. </returns>
		public Task<string> ConnectAnonymous(string username)
			=> ConnectInternal(username, null, null);
        public async Task<string> ConnectInternal(string emailOrUsername, string password, string token)
		{
			bool success = false;
			await Disconnect();
			_blockEvent.Reset();
			_disconnectToken = new CancellationTokenSource();

			//Connect by Token
			if (token != null)
			{
				try
				{
					await _webSocket.ConnectAsync(Endpoints.WebSocket_Hub);
					Http.Token = token;
					await _webSocket.Login();
					success = true;
				}
				catch (InvalidOperationException) //Bad Token
				{
					if (password == null) //If we don't have an alternate login, throw this error
						throw;
				}
			}
			if (!success && password != null) //Email/Password login
			{
				//Open websocket while we wait for login response
				Task socketTask = _webSocket.ConnectAsync(Endpoints.WebSocket_Hub);
				var response = await DiscordAPI.Login(emailOrUsername, password);
				await socketTask;

				//Wait for websocket to finish connecting, then send token
				token = response.Token;
                Http.Token = token;
				await _webSocket.Login();
				success = true;
			}
			if (!success && password == null) //Anonymous login
			{
				//Open websocket while we wait for login response
				Task socketTask = _webSocket.ConnectAsync(Endpoints.WebSocket_Hub);
				var response = await DiscordAPI.LoginAnonymous(emailOrUsername);
				await socketTask;

				//Wait for websocket to finish connecting, then send token
				token = response.Token;
				Http.Token = token;
				await _webSocket.Login();
				success = true;
			}
			if (success)
			{
				var cancelToken = _disconnectToken.Token;
				if (_config.UseMessageQueue)
					_tasks = Task.WhenAll(await Task.Factory.StartNew(SendAsync, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default));
				else
					_tasks = Task.WhenAll(await Task.Factory.StartNew(EmptyAsync, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default));
				_tasks = _tasks.ContinueWith(async x =>
				{
					await _webSocket.DisconnectAsync();
#if !DNXCORE50
					if (_config.EnableVoice)
						await _voiceWebSocket.DisconnectAsync();
#endif

					//Clear send queue
					Message ignored;
					while (_pendingMessages.TryDequeue(out ignored)) { }

					_channels.Clear();
					_messages.Clear();
					_roles.Clear();
					_servers.Clear();
					_users.Clear();

					_blockEvent.Set();
					_tasks = null;
				}).Unwrap();
				_isConnected = true;
			}
			else
				token = null;
            return token;
        }
		/// <summary> Disconnects from the Discord server, canceling any pending requests. </summary>
		public async Task Disconnect()
		{
			if (_tasks != null)
			{
				try { _disconnectToken.Cancel(); } catch (NullReferenceException) { }
				try { await _tasks; } catch (NullReferenceException) { }
			}
		}

		//Servers
		/// <summary> Creates a new server with the provided name and region (see Regions). </summary>
		public async Task<Server> CreateServer(string name, string region)
		{
			CheckReady();
			var response = await DiscordAPI.CreateServer(name, region);
			return _servers.Update(response.Id, response);
		}
		/// <summary> Leaves the provided server, destroying it if you are the owner. </summary>
		public Task<Server> LeaveServer(Server server)
			=> LeaveServer(server.Id);
		/// <summary> Leaves the provided server, destroying it if you are the owner. </summary>
		public async Task<Server> LeaveServer(string serverId)
		{
			CheckReady();
			try
			{
				await DiscordAPI.LeaveServer(serverId);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) {}
			return _servers.Remove(serverId);
		}

		//Channels
		/// <summary> Creates a new channel with the provided name and type (see ChannelTypes). </summary>
		public Task<Channel> CreateChannel(Server server, string name, string type)
			=> CreateChannel(server.Id, name, type);
		/// <summary> Creates a new channel with the provided name and type (see ChannelTypes). </summary>
		public async Task<Channel> CreateChannel(string serverId, string name, string type)
		{
			CheckReady();
			var response = await DiscordAPI.CreateChannel(serverId, name, type);
			return _channels.Update(response.Id, serverId, response);
		}
		/// <summary> Creates a new private channel with the provided user. </summary>
		public Task<Channel> CreatePMChannel(User user)
			=> CreatePMChannel(user.Id);
		/// <summary> Creates a new private channel with the provided user. </summary>
		public async Task<Channel> CreatePMChannel(string userId)
		{
			CheckReady();
			var response = await DiscordAPI.CreatePMChannel(UserId, userId);
			return _channels.Update(response.Id, response);
		}
		/// <summary> Destroys the provided channel. </summary>
		public Task<Channel> DestroyChannel(Channel channel)
			=> DestroyChannel(channel.Id);
		/// <summary> Destroys the provided channel. </summary>
		public async Task<Channel> DestroyChannel(string channelId)
		{
			CheckReady();
			try
			{
				var response = await DiscordAPI.DestroyChannel(channelId);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			return _channels.Remove(channelId);
		}

		//Bans
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(Server server, User user)
			=> Ban(server.Id, user.Id);
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(Server server, string userId)
			=> Ban(server.Id, userId);
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(string server, User user)
			=> Ban(server, user.Id);
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(string serverId, string userId)
		{
			CheckReady();
			return DiscordAPI.Ban(serverId, userId);
		}
		/// <summary> Unbans a user from the provided server. </summary>
		public Task Unban(Server server, User user)
			=> Unban(server.Id, user.Id);
		/// <summary> Unbans a user from the provided server. </summary>
		public Task Unban(Server server, string userId)
			=> Unban(server.Id, userId);
		/// <summary> Unbans a user from the provided server. </summary>
		public Task Unban(string server, User user)
			=> Unban(server, user.Id);
		/// <summary> Unbans a user from the provided server. </summary>
		public async Task Unban(string serverId, string userId)
		{
			CheckReady();
			try
			{
				await DiscordAPI.Unban(serverId, userId);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}

		//Invites
		/// <summary> Creates a new invite to the default channel of the provided server. </summary>
		/// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
		/// <param name="isTemporary"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
		/// <param name="hasXkcdPass"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
		/// <param name="maxUses"> The max amount  of times this invite may be used. </param>
		public Task<Invite> CreateInvite(Server server, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass)
		{
			return CreateInvite(server.DefaultChannelId, maxAge, maxUses, isTemporary, hasXkcdPass);
		}
		/// <summary> Creates a new invite to the provided channel. </summary>
		/// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
		/// <param name="isTemporary"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
		/// <param name="hasXkcdPass"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
		/// <param name="maxUses"> The max amount  of times this invite may be used. </param>
		public Task<Invite> CreateInvite(Channel channel, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass)
		{
			return CreateInvite(channel, maxAge, maxUses, isTemporary, hasXkcdPass);
		}
		/// <summary> Creates a new invite to the provided channel. </summary>
		/// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
		/// <param name="isTemporary"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
		/// <param name="hasXkcdPass"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
		/// <param name="maxUses"> The max amount  of times this invite may be used. </param>
		public async Task<Invite> CreateInvite(string channelId, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass)
		{
			CheckReady();
			var response = await DiscordAPI.CreateInvite(channelId, maxAge, maxUses, isTemporary, hasXkcdPass);
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
		/// <summary> Gets more info about the provided invite. </summary>
		/// <remarks> Supported formats: inviteCode, xkcdCode, https://discord.gg/inviteCode, https://discord.gg/xkcdCode </remarks>
		public async Task<Invite> GetInvite(string id)
		{
			CheckReady();
			var response = await DiscordAPI.GetInvite(id);
			return new Invite(response.Code, response.XkcdPass, this)
			{
				ChannelId = response.Channel.Id,
				InviterId = response.Inviter.Id,
				ServerId = response.Server.Id
			};
		}
		/// <summary> Accepts the provided invite. </summary>
		public Task AcceptInvite(Invite invite)
		{
			CheckReady();
			return DiscordAPI.AcceptInvite(invite.Id);
		}
		/// <summary> Accepts the provided invite. </summary>
		public async Task AcceptInvite(string id)
		{
			CheckReady();
			
			//Remove Url Parts
			if (id.StartsWith(Endpoints.BaseShortHttps))
				id = id.Substring(Endpoints.BaseShortHttps.Length);
			if (id.Length > 0 && id[0] == '/')
				id = id.Substring(1);
			if (id.Length > 0 && id[id.Length - 1] == '/')
				id = id.Substring(0, id.Length - 1);

			//Check if this is a human-readable link and get its ID
			var response = await DiscordAPI.GetInvite(id);
			await DiscordAPI.AcceptInvite(response.Code);
		}
		/// <summary> Deletes the provided invite. </summary>
		public async Task DeleteInvite(string id)
		{
			CheckReady();
			try
			{
				//Check if this is a human-readable link and get its ID
				var response = await DiscordAPI.GetInvite(id);
				await DiscordAPI.DeleteInvite(response.Code);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}

		//Chat
		/// <summary> Sends a message to the provided channel. </summary>
		public Task<Message[]> SendMessage(Channel channel, string text)
			=> SendMessage(channel.Id, text, new string[0]);
		/// <summary> Sends a message to the provided channel. </summary>
		public Task<Message[]> SendMessage(string channelId, string text)
			=> SendMessage(channelId, text, new string[0]);
		/// <summary> Sends a message to the provided channel, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public Task<Message[]> SendMessage(Channel channel, string text, string[] mentions)
			=> SendMessage(channel.Id, text, mentions);
		/// <summary> Sends a message to the provided channel, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public async Task<Message[]> SendMessage(string channelId, string text, string[] mentions)
		{
			CheckReady();
			
			int blockCount = (int)Math.Ceiling(text.Length / (double)DiscordAPI.MaxMessageSize);
			Message[] result = new Message[blockCount];
			for (int i = 0; i < blockCount; i++)
			{
				int index = i * DiscordAPI.MaxMessageSize;
				string blockText = text.Substring(index, Math.Min(2000, text.Length - index));
                var nonce = GenerateNonce();
				if (_config.UseMessageQueue)
				{
					var msg = _messages.Update("nonce_" + nonce, channelId, new API.Models.Message
					{
						Content = blockText,
						Timestamp = DateTime.UtcNow,
						Author = new UserReference { Avatar = User.AvatarId, Discriminator = User.Discriminator, Id = User.Id, Username = User.Name },
						ChannelId = channelId
					});
					msg.IsQueued = true;
					msg.Nonce = nonce;
					result[i] = msg;
                    _pendingMessages.Enqueue(msg);
				}
				else
				{
					var msg = await DiscordAPI.SendMessage(channelId, blockText, mentions, nonce);
					result[i] = _messages.Update(msg.Id, channelId, msg);
					result[i].Nonce = nonce;
					try { RaiseMessageSent(result[i]); } catch { }
				}
				await Task.Delay(1000);
			}
			return result;
		}

		/// <summary> Edits a message the provided message. </summary>
		public Task EditMessage(Message message, string text)
			=> EditMessage(message.ChannelId, message.Id, text, new string[0]);
		/// <summary> Edits a message the provided message. </summary>
		public Task EditMessage(Channel channel, string messageId, string text)
			=> EditMessage(channel.Id, messageId, text, new string[0]);
		/// <summary> Edits a message the provided message. </summary>
		public Task EditMessage(string channelId, string messageId, string text)
			=> EditMessage(channelId, messageId, text, new string[0]);
		/// <summary> Edits a message the provided message, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public Task EditMessage(Message message, string text, string[] mentions)
			=> EditMessage(message.ChannelId, message.Id, text, mentions);
		/// <summary> Edits a message the provided message, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public Task EditMessage(Channel channel, string messageId, string text, string[] mentions)
			=> EditMessage(channel.Id, messageId, text, mentions);
		/// <summary> Edits a message the provided message, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public async Task EditMessage(string channelId, string messageId, string text, string[] mentions)
		{
			CheckReady();
			if (text.Length > DiscordAPI.MaxMessageSize)
				text = text.Substring(0, DiscordAPI.MaxMessageSize);

			var msg = await DiscordAPI.EditMessage(channelId, messageId, text, mentions);
			_messages.Update(msg.Id, channelId, msg);
		}

		/// <summary> Deletes the provided message. </summary>
		public Task DeleteMessage(Message msg)
			=> DeleteMessage(msg.ChannelId, msg.Id);
		/// <summary> Deletes the provided message. </summary>
		public async Task<Message> DeleteMessage(string channelId, string msgId)
		{
			try
			{
				await DiscordAPI.DeleteMessage(channelId, msgId);
				return _messages.Remove(msgId);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.InternalServerError) { } //TODO: Remove me - temporary fix for deleting nonexisting messages
			return null;
		}

		/// <summary> Sends a file to the provided channel. </summary>
		public Task SendFile(Channel channel, string path)
			=> SendFile(channel.Id, path);
		/// <summary> Sends a file to the provided channel. </summary>
		public Task SendFile(string channelId, string path)
			=> SendFile(channelId, File.OpenRead(path), Path.GetFileName(path));
		/// <summary> Reads a stream and sends it to the provided channel as a file. </summary>
		/// <remarks> It is highly recommended that this stream be cached in memory or on disk, or the request may time out. </remarks>
		public Task SendFile(Channel channel, Stream stream, string filename = null)
			=> SendFile(channel.Id, stream, filename);
		/// <summary> Reads a stream and sends it to the provided channel as a file. </summary>
		/// <remarks> It is highly recommended that this stream be cached in memory or on disk, or the request may time out. </remarks>
		public Task SendFile(string channelId, Stream stream, string filename = null)
		{
			return DiscordAPI.SendFile(channelId, stream, filename);
		}


		//Voice
		/// <summary> Mutes a user on the provided server. </summary>
		public Task Mute(Server server, User user)
			=> Mute(server.Id, user.Id);
		/// <summary> Mutes a user on the provided server. </summary>
		public Task Mute(Server server, string userId)
			=> Mute(server.Id, userId);
		/// <summary> Mutes a user on the provided server. </summary>
		public Task Mute(string server, User user)
			=> Mute(server, user.Id);
		/// <summary> Mutes a user on the provided server. </summary>
		public Task Mute(string serverId, string userId)
		{
			CheckReady();
			return DiscordAPI.Mute(serverId, userId);
		}

		/// <summary> Unmutes a user on the provided server. </summary>
		public Task Unmute(Server server, User user)
			=> Unmute(server.Id, user.Id);
		/// <summary> Unmutes a user on the provided server. </summary>
		public Task Unmute(Server server, string userId)
			=> Unmute(server.Id, userId);
		/// <summary> Unmutes a user on the provided server. </summary>
		public Task Unmute(string server, User user)
			=> Unmute(server, user.Id);
		/// <summary> Unmutes a user on the provided server. </summary>
		public Task Unmute(string serverId, string userId)
		{
			CheckReady();
			return DiscordAPI.Unmute(serverId, userId);
		}

		/// <summary> Deafens a user on the provided server. </summary>
		public Task Deafen(Server server, User user)
			=> Deafen(server.Id, user.Id);
		/// <summary> Deafens a user on the provided server. </summary>
		public Task Deafen(Server server, string userId)
			=> Deafen(server.Id, userId);
		/// <summary> Deafens a user on the provided server. </summary>
		public Task Deafen(string server, User user)
			=> Deafen(server, user.Id);
		/// <summary> Deafens a user on the provided server. </summary>
		public Task Deafen(string serverId, string userId)
		{
			CheckReady();
			return DiscordAPI.Deafen(serverId, userId);
		}

		/// <summary> Undeafens a user on the provided server. </summary>
		public Task Undeafen(Server server, User user)
			=> Undeafen(server.Id, user.Id);
		/// <summary> Undeafens a user on the provided server. </summary>
		public Task Undeafen(Server server, string userId)
			=> Undeafen(server.Id, userId);
		/// <summary> Undeafens a user on the provided server. </summary>
		public Task Undeafen(string server, User user)
			=> Undeafen(server, user.Id);
		/// <summary> Undeafens a user on the provided server. </summary>
		public Task Undeafen(string serverId, string userId)
		{
			CheckReady();
			return DiscordAPI.Undeafen(serverId, userId);
		}

#if !DNXCORE50
		public Task JoinVoiceServer(Server server, Channel channel)
			=> JoinVoiceServer(server.Id, channel.Id);
		public Task JoinVoiceServer(Server server, string channelId)
			=> JoinVoiceServer(server.Id, channelId);
		public Task JoinVoiceServer(string serverId, Channel channel)
			=> JoinVoiceServer(serverId, channel.Id);
		public async Task JoinVoiceServer(string serverId, string channelId)
		{
			if (!_config.EnableVoice)
				throw new InvalidOperationException("Voice is not enabled for this client.");

			await LeaveVoiceServer();
			_currentVoiceServerId = serverId;
			_webSocket.JoinVoice(serverId, channelId);
		}

		public async Task LeaveVoiceServer()
		{
			if (!_config.EnableVoice)
				throw new InvalidOperationException("Voice is not enabled for this client.");

			await _voiceWebSocket.DisconnectAsync();
			if (_currentVoiceEndpoint != null)
				_webSocket.LeaveVoice();
			_currentVoiceEndpoint = null;
			_currentVoiceServerId = null;
			_currentVoiceToken = null;
		}

		/// <summary> Sends a PCM frame to the voice server. </summary>
		/// <param name="data">PCM frame to send.</param>
		/// <param name="count">Number of bytes in this frame. </param>
		public void SendVoicePCM(byte[] data, int count)
		{
			if (!_config.EnableVoice)
				throw new InvalidOperationException("Voice is not enabled for this client.");

			_voiceWebSocket.SendPCMFrame(data, count);
		}

		/// <summary> Clears the PCM buffer. </summary>
		public void ClearVoicePCM()
		{
			if (!_config.EnableVoice)
				throw new InvalidOperationException("Voice is not enabled for this client.");

			_voiceWebSocket.ClearPCMFrames();
		}
#endif

		//Profile
		/// <summary> Changes your username to newName. </summary>
		public async Task ChangeUsername(string newName, string currentEmail, string currentPassword)
		{
			CheckReady();
			var response = await DiscordAPI.ChangeUsername(newName, currentEmail, currentPassword);
			_users.Update(response.Id, response);
        }
		/// <summary> Changes your email to newEmail. </summary>
		public async Task ChangeEmail(string newEmail, string currentPassword)
		{
			CheckReady();
			var response = await DiscordAPI.ChangeEmail(newEmail, currentPassword);
			_users.Update(response.Id, response);
		}
		/// <summary> Changes your password to newPassword. </summary>
		public async Task ChangePassword(string newPassword, string currentEmail, string currentPassword)
		{
			CheckReady();
			var response = await DiscordAPI.ChangePassword(newPassword, currentEmail, currentPassword);
			_users.Update(response.Id, response);
		}

		//Helpers
		private void CheckReady()
		{
			if (_blockEvent.IsSet)
				throw new InvalidOperationException("The client is currently disconnecting.");
			else if (!_isConnected)
				throw new InvalidOperationException("The client is not currently connected to Discord");
		}
		internal string CleanMessageText(string text)
		{
			text = _userRegex.Replace(text, _userRegexEvaluator);
			text = _channelRegex.Replace(text, _channelRegexEvaluator);
			return text;
        }
		private string GenerateNonce()
		{
			lock (_rand)
				return _rand.Next().ToString();
		}

		/// <summary> Blocking call that will not return until client has been stopped. This is mainly intended for use in console applications. </summary>
		public void Block()
		{
			_blockEvent.Wait();
		}
	}
}
