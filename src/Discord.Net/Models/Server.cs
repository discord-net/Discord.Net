using Discord.API.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
    /// <summary> Represents a Discord server (also known as a guild). </summary>
	public sealed class Server
    {
        private struct Member
        {
            public readonly User User;
            public readonly ServerPermissions Permissions;
            public Member(User user)
            {
                User = user;
                Permissions = new ServerPermissions();
                Permissions.Lock();
            }
        }

        private readonly ConcurrentDictionary<ulong, Role> _roles;
        private readonly ConcurrentDictionary<ulong, Member> _users;
        private readonly ConcurrentDictionary<ulong, Channel> _channels;
        private readonly ConcurrentDictionary<ulong, bool> _bans;
        private ulong _ownerId;
        private ulong? _afkChannelId;

        /// <summary> Gets the client that generated this server object. </summary>
        internal DiscordClient Client { get; }
        /// <summary> Gets the unique identifier for this server. </summary>
        public ulong Id { get; }
        /// <summary> Gets the default channel for this server. </summary>
        public Channel DefaultChannel { get; }
        /// <summary> Gets the the role representing all users in a server. </summary>
        public Role EveryoneRole { get; }

        /// <summary> Gets the name of this server. </summary>
        public string Name { get; private set; }

        /// <summary> Gets the amount of time (in seconds) a user must be inactive for until they are automatically moved to the AFK channel, if one is set. </summary>
        public int AFKTimeout { get; private set; }
        /// <summary> Gets the date and time you joined this server. </summary>
        public DateTime JoinedAt { get; private set; }
        /// <summary> Gets the voice region for this server. </summary>
        public Region Region { get; private set; }
        /// <summary> Gets the unique identifier for this user's current avatar. </summary>
        public string IconId { get; private set; }
        /// <summary> Gets the URL to this user's current avatar. </summary>
        public string IconUrl => GetIconUrl(Id, IconId);
        internal static string GetIconUrl(ulong serverId, string iconId) 
            => iconId != null ? $"{DiscordConfig.CDNUrl}/icons/{serverId}/{iconId}.jpg" : null;

        /// <summary> Gets the user that created this server. </summary>
        public User Owner => GetUser(_ownerId);
        /// <summary> Gets the AFK voice channel for this server. </summary>
        public Channel AFKChannel => _afkChannelId != null ? GetChannel(_afkChannelId.Value) : null;
        /// <summary> Gets the current user in this server. </summary>
        public User CurrentUser => GetUser(Client.CurrentUser.Id);

        /// <summary> Gets a collection of the ids of all users banned on this server. </summary>
        public IEnumerable<ulong> BannedUserIds => _bans.Select(x => x.Key);
        /// <summary> Gets a collection of all channels within this server. </summary>
        public IEnumerable<Channel> Channels => _channels.Select(x => x.Value);
        /// <summary> Gets a collection of all users within this server with their server-specific data. </summary>
        public IEnumerable<User> Users => _users.Select(x => x.Value.User);
        /// <summary> Gets a collection of all roles within this server. </summary>
        public IEnumerable<Role> Roles => _roles.Select(x => x.Value);

        internal Server(DiscordClient client, ulong id)
        {
            Client = client;
            Id = id;
            _channels = new ConcurrentDictionary<ulong, Channel>();
            _roles = new ConcurrentDictionary<ulong, Role>();
            _users = new ConcurrentDictionary<ulong, Member>();
            _bans = new ConcurrentDictionary<ulong, bool>();
            DefaultChannel = AddChannel(id);
            EveryoneRole = AddRole(id);
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
            _afkChannelId = model.AFKChannelId.Value; //Can be null
            if (model.JoinedAt != null)
                JoinedAt = model.JoinedAt.Value;
            if (model.OwnerId != null)
                _ownerId = model.OwnerId.Value;
            if (model.Region != null)
                Region = Client.GetRegion(model.Region);
            if (model.Icon != null)
                IconId = model.Icon;

            if (model.Roles != null)
            {
                foreach (var x in model.Roles)
                    AddRole(x.Id).Update(x);
            }
        }
        internal void Update(ExtendedGuild model)
        {
            Update(model as Guild);

            if (model.Channels != null)
            {
                foreach (var subModel in model.Channels)
                    AddChannel(subModel.Id).Update(subModel);
            }
            if (model.Members != null)
            {
                foreach (var subModel in model.Members)
                    AddMember(subModel.User.Id).Update(subModel);
            }
            if (model.VoiceStates != null)
            {
                foreach (var subModel in model.VoiceStates)
                    GetUser(subModel.UserId)?.Update(subModel);
            }
            if (model.Presences != null)
            {
                foreach (var subModel in model.Presences)
                    GetUser(subModel.User.Id)?.Update(subModel);
            }
        }

        //Bans
        internal void AddBan(ulong banId)
            => _bans.TryAdd(banId, true);
        internal bool RemoveBan(ulong banId)
        {
            bool ignored;
            return _bans.TryRemove(banId, out ignored);
        }

        //Channels
        internal Channel AddChannel(ulong id)
            => _channels.GetOrAdd(id, x => new Channel(Client, x, this));
        internal Channel RemoveChannel(ulong id)
        {
            Channel channel;
            _channels.TryRemove(id, out channel);
            return channel;
        }
        public Channel GetChannel(ulong id)
        {
            Channel result;
            _channels.TryGetValue(id, out result);
            return result;
        }

        //Members
        internal User AddMember(ulong id)
        {
            User newUser = null;
            var user = _users.GetOrAdd(id, x => new Member(new User(id, this)));
			if (user.User == newUser)
			{
				foreach (var channel in Channels)
					channel.AddUser(newUser);
			}
            return user.User;
        }
		internal User RemoveMember(ulong id)
		{
            Member member;
			if (_users.TryRemove(id, out member))
			{
				foreach (var channel in Channels)
					channel.RemoveUser(id);
			}
            return member.User;
        }
        public User GetUser(ulong id)
        {
            Member result;
            _users.TryGetValue(id, out result);
            return result.User;
        }

        //Roles
        internal Role AddRole(ulong id)
            => _roles.GetOrAdd(id, x => new Role(x, this));
        internal Role RemoveRole(ulong id)
        {
            Role role;
            _roles.TryRemove(id, out role);
            return role;
        }
        public Role GetRole(ulong id)
        {
            Role result;
            _roles.TryGetValue(id, out result);
            return result;
        }

        //Permissions
        internal ServerPermissions GetPermissions(User user)
		{
			Member member;
			if (_users.TryGetValue(user.Id, out member))
				return member.Permissions;
			else
				return null;
		}
		internal void UpdatePermissions(User user)
		{
            Member member;
			if (_users.TryGetValue(user.Id, out member))
				UpdatePermissions(member.User, member.Permissions);
		}
        private void UpdatePermissions(User user, ServerPermissions permissions)
		{
			uint newPermissions = 0;

			if (user.Id == _ownerId)
				newPermissions = ServerPermissions.All.RawValue;
			else
			{
				foreach (var serverRole in user.Roles)
					newPermissions |= serverRole.Permissions.RawValue;
			}

			if (newPermissions.HasBit((byte)PermissionsBits.ManageRolesOrPermissions))
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
		public override string ToString() => Name ?? Id.ToIdString();
	}
}
