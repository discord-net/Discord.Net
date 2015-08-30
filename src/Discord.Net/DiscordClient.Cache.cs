using Discord.API.Models;
using Discord.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    public partial class DiscordClient
	{
		/// <summary> Returns a collection of all users the client can see across all servers. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		public IEnumerable<User> Users => _users;
		private AsyncCache<User, API.Models.UserReference> _users;

		/// <summary> Returns a collection of all servers the client is a member of. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		public IEnumerable<Server> Servers => _servers;
		private AsyncCache<Server, API.Models.ServerReference> _servers;

		/// <summary> Returns a collection of all channels the client can see across all servers. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		public IEnumerable<Channel> Channels => _channels;
		private AsyncCache<Channel, API.Models.ChannelReference> _channels;

		/// <summary> Returns a collection of all messages the client has in cache. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		public IEnumerable<Message> Messages => _messages;
		private AsyncCache<Message, API.Models.MessageReference> _messages;

		/// <summary> Returns a collection of all roles the client can see across all servers. </summary>
		/// <remarks> This collection does not guarantee any ordering. </remarks>
		public IEnumerable<Role> Roles => _roles;
		private AsyncCache<Role, API.Models.Role> _roles;

		private void CreateCaches()
		{
			_servers = new AsyncCache<Server, API.Models.ServerReference>(
				(key, parentKey) =>
				{
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Created server {key}.");
					return new Server(key, this);
				},
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
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Updated server {server.Name} ({server.Id}).");
				},
				server =>
				{
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Destroyed server {server.Name} ({server.Id}).");
				}
			);

			_channels = new AsyncCache<Channel, API.Models.ChannelReference>(
				(key, parentKey) =>
				{
					if (_isDebugMode)
					{
						if (parentKey != null)
							RaiseOnDebugMessage(DebugMessageType.Cache, $"Created channel {key} in server {parentKey}.");
						else
							RaiseOnDebugMessage(DebugMessageType.Cache, $"Created private channel {key}.");
					}
					return new Channel(key, parentKey, this);
				},
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
					if (_isDebugMode)
					{
						if (channel.IsPrivate)
							RaiseOnDebugMessage(DebugMessageType.Cache, $"Updated private channel {channel.Name} ({channel.Id}).");
						else
							RaiseOnDebugMessage(DebugMessageType.Cache, $"Updated channel {channel.Name} ({channel.Id}) in server {channel.Server?.Name} ({channel.ServerId}).");
					}
				},
				channel =>
				{
					if (channel.IsPrivate)
					{
						var user = channel.Recipient;
						if (user.PrivateChannelId == channel.Id)
							user.PrivateChannelId = null;
						if (_isDebugMode)
							RaiseOnDebugMessage(DebugMessageType.Cache, $"Destroyed private channel {channel.Name} ({channel.Id}).");
					}
					else
					{
						if (_isDebugMode)
							RaiseOnDebugMessage(DebugMessageType.Cache, $"Destroyed channel {channel.Name} ({channel.Id}) in server {channel.Server?.Name} ({channel.ServerId}).");
					}
				});

			_messages = new AsyncCache<Message, API.Models.MessageReference>(
				(key, parentKey) =>
				{
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Created message {key} in channel {parentKey}.");
					return new Message(key, parentKey, this);
				},
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
						message.IsMentioningMe = message.MentionIds.Contains(_myId);
						message.RawText = extendedModel.Content;
						message.Timestamp = extendedModel.Timestamp;
						message.EditedTimestamp = extendedModel.EditedTimestamp;
						if (extendedModel.Author != null)
							message.UserId = extendedModel.Author.Id;
					}
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Updated message {message.Id} in channel {message.Channel?.Name} ({message.ChannelId}).");
				},
				message =>
				{
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Destroyed message {message.Id} in channel {message.Channel?.Name} ({message.ChannelId}).");
				}
			);

			_roles = new AsyncCache<Role, API.Models.Role>(
				(key, parentKey) =>
				{
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Created role {key} in server {parentKey}.");
					return new Role(key, parentKey, this);
				},
				(role, model) =>
				{
					role.Name = model.Name;
					role.Permissions.RawValue = (uint)model.Permissions;
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Updated role {role.Name} ({role.Id}) in server {role.Server?.Name} ({role.ServerId}).");
				},
				role =>
				{
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Destroyed role {role.Name} ({role.Id}) in server {role.Server?.Name} ({role.ServerId}).");
				}
			);

			_users = new AsyncCache<User, API.Models.UserReference>(
				(key, parentKey) =>
				{
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Created user {key}.");
					return new User(key, this);
				},
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
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Updated user {user?.Name} ({user.Id}).");
				},
				user =>
				{
					if (_isDebugMode)
						RaiseOnDebugMessage(DebugMessageType.Cache, $"Destroyed user {user?.Name} ({user.Id}).");
				}
			);
		}

		/// <summary> Returns the user with the specified id, or null if none was found. </summary>
		public User GetUser(string id) 
			=> _users[id];
		/// <summary> Returns the user with the specified name and discriminator, or null if none was found. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public User GetUser(string name, string discriminator)
		{
			if (name == null || discriminator == null)
				return null;

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
			if (name == null)
				return new User[0];

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
			=> GetMember(_servers[serverId], user?.Id);
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Membership GetMember(string serverId, string userId)
			=> GetMember(_servers[serverId], userId);
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Membership GetMember(Server server, User user)
			=> GetMember(server, user?.Id);
		/// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
		public Membership GetMember(Server server, string userId)
		{
			if (server == null || userId == null)
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
			if (server == null || name == null)
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
		public Server GetServer(string id) 
			=> _servers[id];
		/// <summary> Returns all servers with the specified name. </summary>
		/// <remarks> Search is case-insensitive. </remarks>
		public IEnumerable<Server> FindServers(string name)
		{
			if (name == null)
				return new Server[0];
			return _servers.Where(x =>
				string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary> Returns the channel with the specified id, or null if none was found. </summary>
		public Channel GetChannel(string id) => _channels[id];
		/// <summary> Returns the private channel with the provided user, creating one if it does not currently exist. </summary>
		public Task<Channel> GetPMChannel(string userId)
			=> GetPMChannel(_users[userId]);
		/// <summary> Returns the private channel with the provided user, creating one if it does not currently exist. </summary>
		public async Task<Channel> GetPMChannel(User user)
		{
			CheckReady();
			if (user == null) throw new ArgumentNullException(nameof(user));

			var channel = user.PrivateChannel;
			if (channel != null)
				return channel;
			return await CreatePMChannel(user?.Id);
		}
		private async Task<Channel> CreatePMChannel(string userId)
		{
			CheckReady();
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			var response = await _api.CreatePMChannel(_myId, userId);
			return _channels.Update(response.Id, response);
		}

		/// <summary> Returns all channels with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and #Name. Search is case-insensitive. </remarks>
		public IEnumerable<Channel> FindChannels(Server server, string name)
			=> FindChannels(server?.Id, name);
		/// <summary> Returns all channels with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and #Name. Search is case-insensitive. </remarks>
		public IEnumerable<Channel> FindChannels(string serverId, string name)
		{
			if (serverId == null || name == null)
				return new Channel[0];

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
		public Role GetRole(string id) 
			=> _roles[id];
		/// <summary> Returns all roles with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<Role> FindRoles(Server server, string name)
			=> FindRoles(server?.Id, name);
		/// <summary> Returns all roles with the specified server and name. </summary>
		/// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
		public IEnumerable<Role> FindRoles(string serverId, string name)
		{
			if (serverId == null || name == null)
				return new Role[0];

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
		public Message GetMessage(string id) 
			=> _messages[id];
	}
}
