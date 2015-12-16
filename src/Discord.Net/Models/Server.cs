using Discord.API.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using APIGuild = Discord.API.Client.Guild;

namespace Discord
{
	public sealed class Server : CachedObject<ulong>
	{
		private struct ServerMember
		{
			public readonly User User;
			public readonly ServerPermissions Permissions;

			public ServerMember(User user)
			{
				User = user;
				Permissions = new ServerPermissions();
				Permissions.Lock();
			}
		}
		
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get; private set; }
		/// <summary> Returns the current logged-in user's data for this server. </summary>
		public User CurrentUser { get; internal set; }

		/// <summary> Returns the amount of time (in seconds) a user must be inactive for until they are automatically moved to the AFK channel (see AFKChannel). </summary>
		public int AFKTimeout { get; private set; }
		/// <summary> Returns the date and time your joined this server. </summary>
		public DateTime JoinedAt { get; private set; }
		/// <summary> Returns the region for this server (see Regions). </summary>
		public string Region { get; private set; }
		/// <summary> Returns the unique identifier for this user's current avatar. </summary>
		public string IconId { get; private set; }
		/// <summary> Returns the URL to this user's current avatar. </summary>
		public string IconUrl => IconId != null ? $"{DiscordConfig.CDNUrl}/icons/{Id}/{IconId}.jpg" : null;

		/// <summary> Returns the user that first created this server. </summary>
		[JsonIgnore]
		public User Owner => _owner.Value;
		[JsonProperty]
		internal ulong? OwnerId => _owner.Id;
		private Reference<User> _owner;

		/// <summary> Returns the AFK voice channel for this server (see AFKTimeout). </summary>
		[JsonIgnore]
		public Channel AFKChannel => _afkChannel.Value;
		[JsonProperty]
		private ulong? AFKChannelId => _afkChannel.Id;
        private Reference<Channel> _afkChannel;

		/// <summary> Returns the default channel for this server. </summary>
		[JsonIgnore]
		public Channel DefaultChannel { get; private set; }

		/// <summary> Returns a collection of the ids of all users banned on this server. </summary>
		public IEnumerable<ulong> BannedUserIds => _bans.Select(x => x.Key);
		private ConcurrentDictionary<ulong, bool> _bans;
		
		/// <summary> Returns a collection of all channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> Channels => _channels.Select(x => x.Value);
		/// <summary> Returns a collection of all text channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> TextChannels => _channels.Select(x => x.Value).Where(x => x.Type == ChannelType.Text);
		/// <summary> Returns a collection of all voice channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> VoiceChannels => _channels.Select(x => x.Value).Where(x => x.Type == ChannelType.Voice);
		[JsonProperty]
		private IEnumerable<ulong> ChannelIds => Channels.Select(x => x.Id);
		private ConcurrentDictionary<ulong, Channel> _channels;

		/// <summary> Returns a collection of all users within this server with their server-specific data. </summary>
		[JsonIgnore]
		public IEnumerable<User> Members => _members.Select(x => x.Value.User);
		[JsonProperty]
		private IEnumerable<ulong> MemberIds => Members.Select(x => x.Id);
		private ConcurrentDictionary<ulong, ServerMember> _members;

		/// <summary> Return the the role representing all users in a server. </summary>
		[JsonIgnore]
		public Role EveryoneRole { get; private set; }
		/// <summary> Returns a collection of all roles within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Role> Roles => _roles.Select(x => x.Value);
		[JsonProperty]
		private IEnumerable<ulong> RoleIds => Roles.Select(x => x.Id);
		private ConcurrentDictionary<ulong, Role> _roles;

		internal Server(DiscordClient client, ulong id)
			: base(client, id)
		{
			_owner = new Reference<User>(x => _client.Users[x, Id]);
			_afkChannel = new Reference<Channel>(x => _client.Channels[x]);

			//Global Cache
			_channels = new ConcurrentDictionary<ulong, Channel>();
			_roles = new ConcurrentDictionary<ulong, Role>();
			_members = new ConcurrentDictionary<ulong, ServerMember>();

			//Local Cache
			_bans = new ConcurrentDictionary<ulong, bool>();            
            EveryoneRole = _client.Roles.GetOrAdd(id, id);
        }
		internal override bool LoadReferences()
		{
			_afkChannel.Load();
            _owner.Load();
			return true;
        }
		internal override void UnloadReferences()
		{
			//Global Cache
			var globalChannels = _client.Channels;
			var channels = _channels;
			foreach (var channel in channels)
				globalChannels.TryRemove(channel.Key);
			channels.Clear();

			var globalUsers = _client.Users;
			var members = _members;
			foreach (var member in members)
				globalUsers.TryRemove(member.Key, Id);
			members.Clear();

			var globalRoles = _client.Roles;
			var roles = _roles;
			foreach (var role in roles)
				globalRoles.TryRemove(role.Key);
			roles.Clear();

			//Local Cache
			_bans.Clear();

			_afkChannel.Unload();
        }

		internal void Update(GuildReference model)
		{
			if (model.Name != null)
				Name = model.Name;
		}

        internal void Update(Guild model)
		{
			Update(model as GuildReference);

			if (model.AFKTimeout != null)
				AFKTimeout = model.AFKTimeout.Value;
			if (model.AFKChannelId != null)
			if (model.JoinedAt != null)
				JoinedAt = model.JoinedAt.Value;
			if (model.OwnerId != null)
				_owner.Id = model.OwnerId.Value;
			if (model.Region != null)
				Region = model.Region;
			if (model.Icon != null)
				IconId = model.Icon;

			if (model.Roles != null)
			{
				var roleCache = _client.Roles;
				foreach (var x in model.Roles)
				{
					var role = roleCache.GetOrAdd(x.Id, Id);
					role.Update(x);
                }
            }
			
			_afkChannel.Id = model.AFKChannelId; //Can be null
		}
		internal void Update(ExtendedGuild model)
		{
			Update(model as APIGuild);
			
			var channels = _client.Channels;
			foreach (var subModel in model.Channels)
			{
				var channel = channels.GetOrAdd(subModel.Id, Id);
				channel.Update(subModel);
			}
			
			var usersCache = _client.Users;
            foreach (var subModel in model.Members)
			{
				var user = usersCache.GetOrAdd(subModel.User.Id, Id);
				user.Update(subModel);
			}
			foreach (var subModel in model.VoiceStates)
			{
				var user = usersCache[subModel.UserId, Id];
				if (user != null)
					user.Update(subModel);
			}
			foreach (var subModel in model.Presences)
			{
				var user = usersCache[subModel.User.Id, Id];
				if (user != null)
					user.Update(subModel);
			}
		}

		internal void AddBan(ulong banId)
		{
			_bans.TryAdd(banId, true);
		}
		internal bool RemoveBan(ulong banId)
		{
			bool ignored;
			return _bans.TryRemove(banId, out ignored);
		}

		internal void AddChannel(Channel channel)
		{
			if (_channels.TryAdd(channel.Id, channel))
			{
				if (channel.Id == Id)
					DefaultChannel = channel;
			}
		}
		internal void RemoveChannel(Channel channel)
		{
			_channels.TryRemove(channel.Id, out channel);
		}

		internal void AddMember(User user)
		{
			if (_members.TryAdd(user.Id, new ServerMember(user)))
			{
				foreach (var channel in Channels)
					channel.AddMember(user);
			}
        }
		internal void RemoveMember(User user)
		{
			ServerMember ignored;
			if (_members.TryRemove(user.Id, out ignored))
			{
				foreach (var channel in Channels)
					channel.RemoveMember(user);
			}
		}
		internal void HasMember(User user) => _members.ContainsKey(user.Id);

		internal void AddRole(Role role)
		{
			if (_roles.TryAdd(role.Id, role))
			{
				if (role.Id == Id)
					EveryoneRole = role;
			}
		}
		internal void RemoveRole(Role role)
		{
            _roles.TryRemove(role.Id, out role);
		}

		internal ServerPermissions GetPermissions(User user)
		{
			ServerMember member;
			if (_members.TryGetValue(user.Id, out member))
				return member.Permissions;
			else
				return null;
		}
		internal void UpdatePermissions(User user)
		{
			ServerMember member;
			if (_members.TryGetValue(user.Id, out member))
				UpdatePermissions(member.User, member.Permissions);
		}
        private void UpdatePermissions(User user, ServerPermissions permissions)
		{
			uint newPermissions = 0;

			if (user.IsOwner)
				newPermissions = ServerPermissions.All.RawValue;
			else
			{
				foreach (var serverRole in user.Roles)
					newPermissions |= serverRole.Permissions.RawValue;
			}

			if (BitHelper.GetBit(newPermissions, (int)PermissionsBits.ManageRolesOrPermissions))
				newPermissions = ServerPermissions.All.RawValue;

			if (newPermissions != permissions.RawValue)
			{
				permissions.SetRawValueInternal(newPermissions);
				foreach (var channel in _channels)
					channel.Value.UpdatePermissions(user);
			}
		}

		public override bool Equals(object obj) => obj is Server && (obj as Server).Id == Id;
		public override int GetHashCode() => unchecked(Id.GetHashCode() + 5175);
		public override string ToString() => Name ?? IdConvert.ToString(Id);
	}
}
