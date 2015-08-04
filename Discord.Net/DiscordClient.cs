using Discord.API;
using Discord.API.Models;
using Discord.Helpers;
using Discord.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord
{
	public partial class DiscordClient
	{
		private const int MaxMessageSize = 2000;

        private DiscordWebSocket _webSocket;
		private HttpOptions _httpOptions;
		private bool _isClosing, _isReady;

		public string SelfId { get; private set; }
		public User Self { get { return GetUser(SelfId); } }

		public IEnumerable<User> Users { get { return _users.Values; } }
		private ConcurrentDictionary<string, User> _users;

		public IEnumerable<Server> Servers { get { return _servers.Values; } }
		private ConcurrentDictionary<string, Server> _servers;

		public IEnumerable<Channel> Channels { get { return _channels.Values; } }
		private ConcurrentDictionary<string, Channel> _channels;

		public DiscordClient()
		{
			string version = typeof(DiscordClient).GetTypeInfo().Assembly.GetName().Version.ToString(2);
			_httpOptions = new HttpOptions { UserAgent = $"Discord.Net/{version} (https://github.com/RogueException/Discord.Net)" };

			_users = new ConcurrentDictionary<string, User>();
			_servers = new ConcurrentDictionary<string, Server>();
			_channels = new ConcurrentDictionary<string, Channel>();

			_webSocket = new DiscordWebSocket();
			_webSocket.Connected += (s,e) => RaiseConnected();
			_webSocket.Disconnected += async (s,e) =>
			{
				//Reconnect if we didn't cause the disconnect
				RaiseDisconnected();
				if (!_isClosing)
				{
					await Task.Delay(1000);
					await _webSocket.ConnectAsync(Endpoints.WebSocket_Hub, _httpOptions);
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

							SelfId = data.User.Id;
							UpdateUser(data.User);
							foreach (var server in data.Guilds)
								UpdateServer(server);
							foreach (var channel in data.PrivateChannels)
								UpdateChannel(channel as ChannelInfo, null);

							RaiseLoggedIn();
						}
						break;

					//Servers
					case "GUILD_CREATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildCreate>();
							var server = UpdateServer(data);
							RaiseServerCreated(server);
						}
						break;
					case "GUILD_DELETE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildDelete>();
							Server server;
							if (_servers.TryRemove(data.Id, out server))
								RaiseServerDestroyed(server);
						}
						break;

					//Channels
					case "CHANNEL_CREATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.ChannelCreate>();
							var channel = UpdateChannel(data, null);
							RaiseChannelCreated(channel);
						}
						break;
					case "CHANNEL_DELETE":
						{
							var data = e.Event.ToObject<WebSocketEvents.ChannelDelete>();
							var channel = DeleteChannel(data.Id);
							RaiseChannelDestroyed(channel);
						}
						break;

					//Members
					case "GUILD_MEMBER_ADD":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildMemberAdd>();
                            var user = UpdateUser(data.User);
							var server = GetServer(data.GuildId);
							server._members[user.Id] = true;
                        }
						break;
					case "GUILD_MEMBER_REMOVE":
						{
							var data = e.Event.ToObject<WebSocketEvents.GuildMemberRemove>();
							var user = UpdateUser(data.User);
							var server = GetServer(data.GuildId);
							server._members[user.Id] = true;
						}
						break;

					//Users
					case "PRESENCE_UPDATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.PresenceUpdate>();
							var user = UpdateUser(data);
							RaisePresenceUpdated(user);
						}
						break;
					case "VOICE_STATE_UPDATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.VoiceStateUpdate>();
							var user = GetUser(data.UserId); //TODO: Don't ignore this
							RaiseVoiceStateUpdated(user);
						}
						break;

					//Messages
					case "MESSAGE_CREATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.MessageCreate>();
							var msg = UpdateMessage(data);
							msg.User.UpdateActivity(data.Timestamp);
							RaiseMessageCreated(msg);
						}
						break;
					case "MESSAGE_UPDATE":
						{
							var data = e.Event.ToObject<WebSocketEvents.MessageUpdate>();
							var msg = GetMessage(data.Id, data.ChannelId);
							RaiseMessageUpdated(msg);
						}
						break;
					case "MESSAGE_DELETE":
						{
							var data = e.Event.ToObject<WebSocketEvents.MessageDelete>();
							var msg = GetMessage(data.MessageId, data.ChannelId);
							RaiseMessageDeleted(msg);
						}
						break;
					case "MESSAGE_ACK":
						{
							var data = e.Event.ToObject<WebSocketEvents.MessageAck>();
							var msg = GetMessage(data.MessageId, data.ChannelId);
							RaiseMessageAcknowledged(msg);
						}
						break;
					case "TYPING_START":
						{
							var data = e.Event.ToObject<WebSocketEvents.TypingStart>();
							var channel = GetChannel(data.ChannelId);
							var user = GetUser(data.UserId);
							RaiseUserTyping(user, channel);
						}
						break;
					default:
						RaiseOnDebugMessage("Unknown WebSocket message type: " + e.Type);
						break;
				}
			};
			_webSocket.OnDebugMessage += (s, e) => RaiseOnDebugMessage(e.Message);
		}

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
		}

		public Task CreateServer(string name, Region region)
		{
			CheckReady();
			return DiscordAPI.CreateServer(name, region, _httpOptions);
		}
		public Task DeleteServer(string id)
		{
			CheckReady();
			return DiscordAPI.DeleteServer(id, _httpOptions);
		}

		public Task<GetInviteResponse> GetInvite(string id)
		{
			CheckReady();
			return DiscordAPI.GetInvite(id, _httpOptions);
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
			//Check if this is a human-readable link and get its ID
			var response = await DiscordAPI.GetInvite(id, _httpOptions);
			await DiscordAPI.DeleteInvite(response.Code, _httpOptions);
		}

		public Task SendMessage(string channelId, string text)
		{
			return SendMessage(channelId, text, new string[0]);
		}
		public async Task SendMessage(string channelId, string text, string[] mentions)
		{
			CheckReady();
			if (text.Length <= 2000)
				await DiscordAPI.SendMessage(channelId, text, mentions, _httpOptions);
			else
			{
				int blockCount = (int)Math.Ceiling(text.Length / (double)MaxMessageSize);
				for (int i = 0; i < blockCount; i++)
				{
					int index = i * MaxMessageSize;
                    await DiscordAPI.SendMessage(channelId, text.Substring(index, Math.Min(2000, text.Length - index)), mentions, _httpOptions);
					await Task.Delay(1000);
				}
			}
		}

		public User GetUser(string id)
		{
			if (id == null) return null;
			User user = null;
			_users.TryGetValue(id, out user);
			return user;
		}
		private User UpdateUser(UserInfo model)
		{
			var user = GetUser(model.Id) ?? new User(model.Id, this);
			
			user.Avatar = model.Avatar;
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

			_users[model.Id] = user;
			return user;
		}

		public Server GetServer(string id)
		{
			if (id == null) return null;
			Server server = null;
			_servers.TryGetValue(id, out server);
			return server;
		}
		private Server UpdateServer(ServerInfo model)
		{
			var server = GetServer(model.Id) ?? new Server(model.Id, this);
			
			server.Name = model.Name;
			if (model is ExtendedServerInfo)
			{
				var extendedModel = model as ExtendedServerInfo;
				server.AFKChannelId = extendedModel.AFKChannelId;
				server.AFKTimeout = extendedModel.AFKTimeout;
				server.JoinedAt = extendedModel.JoinedAt;
				server.OwnerId = extendedModel.OwnerId;
				server.Presence = extendedModel.Presence;
				server.Region = extendedModel.Region;
				server.Roles = extendedModel.Roles;
				server.VoiceStates = extendedModel.VoiceStates;

				foreach (var channel in extendedModel.Channels)
				{
					UpdateChannel(channel, model.Id);
					server._channels[channel.Id] = true;
				}
				foreach (var membership in extendedModel.Members)
				{
					UpdateUser(membership.User);
					server._members[membership.User.Id] = true;
                }
			}

			_servers[model.Id] = server;
			return server;
		}

		public Channel GetChannel(string id)
		{
			if (id == null) return null;
			Channel channel = null;
			_channels.TryGetValue(id, out channel);
			return channel;
		}
		private Channel UpdateChannel(ChannelInfo model, string serverId)
		{
			var channel = GetChannel(model.Id) ?? new Channel(model.Id, serverId, this);

			channel.Name = model.Name;
			channel.IsPrivate = model.IsPrivate;
			channel.PermissionOverwrites = model.PermissionOverwrites;
			channel.RecipientId = model.Recipient?.Id;
			channel.Type = model.Type;

			_channels[model.Id] = channel;
			return channel;
		}
		private Channel DeleteChannel(string id)
		{
			Channel channel = null;
			if (_channels.TryRemove(id, out channel))
			{
				bool ignored;
				channel.Server._channels.TryRemove(id, out ignored);
			}
			return channel;			
        }

		//TODO: Temporary measure, unsure if we want to store these or not.
		private ChatMessageReference GetMessage(string id, string channelId)
		{
			if (id == null || channelId == null) return null;
			var msg = new ChatMessageReference(id, this);

			msg.ChannelId = channelId;

			return msg;
		}
		private ChatMessage UpdateMessage(WebSocketEvents.MessageCreate model)
		{
			return new ChatMessage(model.Id, this)
			{
				Attachments = model.Attachments,
				ChannelId = model.ChannelId,
				Text = model.Content,
				Embeds = model.Embeds,
				IsMentioningEveryone = model.IsMentioningEveryone,
				IsTTS = model.IsTextToSpeech,
				UserId = model.Author.Id,
				Timestamp = model.Timestamp
			};
		}

		private void CheckReady()
		{
			if (!_isReady)
				throw new InvalidOperationException("The client is not currently connected to Discord");
        }
	}
}
