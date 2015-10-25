using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public class User : CachedObject
	{
		internal static string GetId(string userId, string serverId) => (serverId ?? "Private") + '_' + userId;

		private ConcurrentDictionary<string, Channel> _channels;
		private ConcurrentDictionary<string, ChannelPermissions> _permissions;
		private ServerPermissions _serverPermissions;

		/// <summary> Returns a unique identifier combining this user's id with its server's. </summary>
		internal string UniqueId => GetId(Id, _server.Id);
		/// <summary> Returns the name of this user on this server. </summary>
		public string Name { get; private set; }
		/// <summary> Returns a by-name unique identifier separating this user from others with the same name. </summary>
		public string Discriminator { get; private set; }
		/// <summary> Returns the unique identifier for this user's current avatar. </summary>
		public string AvatarId { get; private set; }
		/// <summary> Returns the URL to this user's current avatar. </summary>
		public string AvatarUrl => API.Endpoints.UserAvatar(Id, AvatarId);
		/// <summary> Returns the datetime that this user joined this server. </summary>
		public DateTime JoinedAt { get; private set; }

		public bool IsSelfMuted { get; private set; }
		public bool IsSelfDeafened { get; private set; }
		public bool IsServerMuted { get; private set; }
		public bool IsServerDeafened { get; private set; }
		public bool IsServerSuppressed { get; private set; }
		public bool IsSpeaking { get; internal set; }

		public string SessionId { get; private set; }
		public string Token { get; private set; }

		/// <summary> Returns the id for the game this user is currently playing. </summary>
		public string GameId { get; private set; }
		/// <summary> Returns the current status for this user. </summary>
		public UserStatus Status { get; private set; }
		/// <summary> Returns the time this user last sent/edited a message, started typing or sent voice data in this server. </summary>
		public DateTime? LastActivityAt { get; private set; }
		/// <summary> Returns the time this user was last seen online in this server. </summary>
		public DateTime LastOnlineAt => Status != UserStatus.Offline ? DateTime.UtcNow : _lastOnline;
		private DateTime _lastOnline;

		/// <summary> Returns the private messaging channel with this user, if one exists. </summary>
		[JsonIgnore]
		public Channel PrivateChannel => GlobalUser.PrivateChannel;

		[JsonIgnore]
		internal GlobalUser GlobalUser => _globalUser.Value;
		private readonly Reference<GlobalUser> _globalUser;

		[JsonIgnore]
		public Server Server => _server.Value;
        private readonly Reference<Server> _server;

		[JsonIgnore]
		public Channel VoiceChannel { get; private set; }

		[JsonIgnore]
		public IEnumerable<Role> Roles => _roles.Select(x => x.Value);
		private Dictionary<string, Role> _roles;

		/// <summary> Returns a collection of all messages this user has sent on this server that are still in cache. </summary>
		[JsonIgnore]
		public IEnumerable<Message> Messages
		{
			get
			{
				if (_server.Id != null)
					return Server.Channels.SelectMany(x => x.Messages.Where(y => y.User.Id == Id));
				else
					return GlobalUser.PrivateChannel.Messages.Where(x => x.User.Id == Id);
            }
		}

		/// <summary> Returns a collection of all channels this user is a member of. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> Channels
		{
			get
			{
				if (_server.Id != null)
				{
					return _permissions
						.Where(x => x.Value.ReadMessages)
						.Select(x =>
						{
							Channel channel = null;
							_channels.TryGetValue(x.Key, out channel);
							return channel;
                        })
						.Where(x => x != null);
				}
				else
				{
					var privateChannel = PrivateChannel;
					if (privateChannel != null)
						return new Channel[] { privateChannel };
					else
						return new Channel[0];
				}
			}
		}

		internal User(DiscordClient client, string id, string serverId)
			: base(client, id)
		{
			_globalUser = new Reference<GlobalUser>(id, 
				x => _client.GlobalUsers.GetOrAdd(x), 
				x => x.AddUser(this), 
				x => x.RemoveUser(this));
			_server = new Reference<Server>(serverId, 
				x => _client.Servers[x], 
				x =>
				{
					x.AddMember(this);
					if (Id == _client.CurrentUserId)
						x.CurrentUser = this;
                }, 
				x =>
				{
					x.RemoveMember(this);
					if (Id == _client.CurrentUserId)
						x.CurrentUser = null;
				});
			Status = UserStatus.Offline;
			_channels = new ConcurrentDictionary<string, Channel>();
			if (serverId != null)
			{
				_permissions = new ConcurrentDictionary<string, ChannelPermissions>();
				_serverPermissions = new ServerPermissions();
			}

			if (serverId == null)
				UpdateRoles(null);
		}
		internal override void LoadReferences()
		{
			_globalUser.Load();
			_server.Load();
		}
		internal override void UnloadReferences()
		{
			_globalUser.Unload();
			_server.Unload();
		}

		public override string ToString() => Id;

		internal void Update(UserReference model)
		{
			if (model.Avatar != null)
				AvatarId = model.Avatar;
			if (model.Discriminator != null)
				Discriminator = model.Discriminator;
			if (model.Username != null)
				Name = model.Username;
		}
		internal void Update(MemberInfo model)
		{
			if (model.User != null)
				Update(model.User);

			if (model.JoinedAt.HasValue)
				JoinedAt = model.JoinedAt.Value;
			if (model.Roles != null)
				UpdateRoles(model.Roles.Select(x => _client.Roles[x]));

			UpdateServerPermissions();
        }
		internal void Update(ExtendedMemberInfo model)
		{
			Update(model as API.MemberInfo);

			if (model.IsServerDeafened != null)
				IsServerDeafened = model.IsServerDeafened.Value;
			if (model.IsServerMuted != null)
				IsServerMuted = model.IsServerMuted.Value;
		}
		internal void Update(PresenceInfo model)
		{
			if (model.User != null)
				Update(model.User as UserReference);

			if (model.Roles != null)
				UpdateRoles(model.Roles.Select(x => _client.Roles[x]));
			if (model.Status != null && Status != model.Status)
			{
				Status = UserStatus.FromString(model.Status);
				if (Status == UserStatus.Offline)
					_lastOnline = DateTime.UtcNow;
			}
			
			GameId = model.GameId; //Allows null
		}
		internal void Update(VoiceMemberInfo model)
		{
			if (model.IsServerDeafened != null)
				IsServerDeafened = model.IsServerDeafened.Value;
			if (model.IsServerMuted != null)
				IsServerMuted = model.IsServerMuted.Value;
			if (model.SessionId != null)
				SessionId = model.SessionId;
			if (model.Token != null)
				Token = model.Token;

			if (model.ChannelId != null)
				VoiceChannel = _client.Channels[model.ChannelId];
			if (model.IsSelfDeafened != null)
				IsSelfDeafened = model.IsSelfDeafened.Value;
			if (model.IsSelfMuted != null)
				IsSelfMuted = model.IsSelfMuted.Value;
			if (model.IsServerSuppressed != null)
				IsServerSuppressed = model.IsServerSuppressed.Value;
		}
		private void UpdateRoles(IEnumerable<Role> roles)
		{
			if (_server.Id != null)
			{
				Dictionary<string, Role> newRoles;
				if (roles != null)
					newRoles = roles.ToDictionary(x => x.Id, x => x);
				else
					newRoles = new Dictionary<string, Role>();

				var everyone = Server.EveryoneRole;
				newRoles.Add(everyone.Id, everyone);
				_roles = newRoles;
			}
			else
				_roles = new Dictionary<string, Role>();
		}

		internal void UpdateActivity(DateTime? activity = null)
		{
			if (LastActivityAt == null || activity > LastActivityAt.Value)
				LastActivityAt = activity ?? DateTime.UtcNow;
		}

		internal void UpdateServerPermissions()
		{
			if (_roles == null) return; // We don't have all our data processed yet, this will be called again soon

			var server = Server;
			if (server == null) return;

			uint newPermissions = 0x0;
			uint oldPermissions = _serverPermissions.RawValue;

			if (server.Owner == this)
				newPermissions = ServerPermissions.All.RawValue;
			else
			{
				//var roles = Roles.OrderBy(x => x.Id);
				var roles = Roles;
				foreach (var serverRole in roles)
					newPermissions |= serverRole.Permissions.RawValue;
			}

			if (BitHelper.GetBit(newPermissions, (int)PermissionsBits.ManageRolesOrPermissions))
				newPermissions = ServerPermissions.All.RawValue;

			if (newPermissions != oldPermissions)
			{
				_serverPermissions.SetRawValueInternal(newPermissions);
				foreach (var channel in _channels)
					UpdateChannelPermissions(channel.Value);
			}
		}
		internal void UpdateChannelPermissions(Channel channel)
		{
			if (_roles == null) return; // We don't have all our data processed yet, this will be called again soon

			var server = Server;
			if (server == null) return;
			if (channel.Server != server) throw new InvalidOperationException();

			ChannelPermissions permissions;
			if (!_permissions.TryGetValue(channel.Id, out permissions)) return;
			uint newPermissions = _serverPermissions.RawValue;
			uint oldPermissions = permissions.RawValue;
			
			if (server.Owner == this)
				newPermissions = ChannelPermissions.All(channel).RawValue;
			else
			{
				var channelOverwrites = channel.PermissionOverwrites;

				//var roles = Roles.OrderBy(x => x.Id);
				var roles = Roles;
				foreach (var denyRole in channelOverwrites.Where(x => x.TargetType == PermissionTarget.Role && x.Deny.RawValue != 0 && roles.Any(y => y.Id == x.TargetId)))
					newPermissions &= ~denyRole.Deny.RawValue;
				foreach (var allowRole in channelOverwrites.Where(x => x.TargetType == PermissionTarget.Role && x.Allow.RawValue != 0 && roles.Any(y => y.Id == x.TargetId)))
					newPermissions |= allowRole.Allow.RawValue;
				foreach (var denyUser in channelOverwrites.Where(x => x.TargetType == PermissionTarget.User && x.TargetId == Id && x.Deny.RawValue != 0))
					newPermissions &= ~denyUser.Deny.RawValue;
				foreach (var allowUser in channelOverwrites.Where(x => x.TargetType == PermissionTarget.User && x.TargetId == Id && x.Allow.RawValue != 0))
					newPermissions |= allowUser.Allow.RawValue;
			}

            if (BitHelper.GetBit(newPermissions, (int)PermissionsBits.ManageRolesOrPermissions))
				newPermissions = ChannelPermissions.All(channel).RawValue;

			if (newPermissions != oldPermissions)
			{
				permissions.SetRawValueInternal(newPermissions);
				channel.InvalidateMembersCache();
			}

			permissions.SetRawValueInternal(newPermissions);
		}

		public ServerPermissions GetServerPermissions() => _serverPermissions;
		public ChannelPermissions GetPermissions(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));

			//Return static permissions if this is a private chat
			if (_server.Id == null)
				return ChannelPermissions.PrivateOnly;

			ChannelPermissions perms;
			if (_permissions.TryGetValue(channel.Id, out perms))
				return perms;
			return null;
		}

		internal void AddChannel(Channel channel)
		{
			if (_server.Id != null)
			{
				var perms = new ChannelPermissions();
				perms.Lock();
				_channels.TryAdd(channel.Id, channel);
				_permissions.TryAdd(channel.Id, perms);
				UpdateChannelPermissions(channel);
			}
		}
		internal void RemoveChannel(Channel channel)
		{
			if (_server.Id != null)
			{
				ChannelPermissions ignored;
				_channels.TryRemove(channel.Id, out channel);
				_permissions.TryRemove(channel.Id, out ignored);
			}
		}

		public bool HasRole(Role role)
		{
			if (role == null) throw new ArgumentNullException(nameof(role));
			
			return _roles.ContainsKey(role.Id);
		}
	}
}