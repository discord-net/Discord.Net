using Discord.API.Client;
using Discord.API.Client.Rest;
using Discord.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> Represents a Discord server (also known as a guild). </summary>
	public sealed class Server
    {
        internal static string GetIconUrl(ulong serverId, string iconId)
            => iconId != null ? $"{DiscordConfig.CDNUrl}icons/{serverId}/{iconId}.jpg" : null;

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
        
        internal DiscordClient Client { get; }

        /// <summary> Gets the unique identifier for this server. </summary>
        public ulong Id { get; }
        /// <summary> Gets the default channel for this server. </summary>
        public Channel DefaultChannel { get; }
        /// <summary> Gets the the role representing all users in a server. </summary>
        public Role EveryoneRole { get; }

        /// <summary> Gets the name of this server. </summary>
        public string Name { get; private set; }
        /// <summary> Gets the voice region for this server. </summary>
        public Region Region { get; private set; }
        /// <summary> Gets the unique identifier for this user's current avatar. </summary>
        public string IconId { get; private set; }
        /// <summary> Gets the amount of time (in seconds) a user must be inactive for until they are automatically moved to the AFK voice channel, if one is set. </summary>
        public int AFKTimeout { get; private set; }
        /// <summary> Gets the date and time you joined this server. </summary>
        public DateTime JoinedAt { get; private set; }

        /// <summary> Gets the user that created this server. </summary>
        public User Owner => GetUser(_ownerId);
        /// <summary> Gets the AFK voice channel for this server. </summary>
        public Channel AFKChannel => _afkChannelId != null ? GetChannel(_afkChannelId.Value) : null;
        /// <summary> Gets the current user in this server. </summary>
        public User CurrentUser => GetUser(Client.CurrentUser.Id);
        /// <summary> Gets the URL to this user's current avatar. </summary>
        public string IconUrl => GetIconUrl(Id, IconId);

        /// <summary> Gets a collection of the ids of all users banned on this server. </summary>
        public IEnumerable<ulong> BannedUserIds => _bans.Select(x => x.Key);
        /// <summary> Gets a collection of all channels in this server. </summary>
        public IEnumerable<Channel> AllChannels => _channels.Select(x => x.Value);
        /// <summary> Gets a collection of text channels in this server. </summary>
        public IEnumerable<Channel> TextChannels => _channels.Select(x => x.Value).Where(x => x.Type == ChannelType.Text);
        /// <summary> Gets a collection of voice channels in this server. </summary>
        public IEnumerable<Channel> VoiceChannels => _channels.Select(x => x.Value).Where(x => x.Type == ChannelType.Voice);
        /// <summary> Gets a collection of all members in this server. </summary>
        public IEnumerable<User> Users => _users.Select(x => x.Value.User);
        /// <summary> Gets a collection of all roles in this server. </summary>
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
            _afkChannelId = model.AFKChannelId; //Can be null
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
                    AddUser(subModel.User.Id).Update(subModel);
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
        
        /// <summary> Edits this server, changing only non-null attributes. </summary>
        public Task Edit(string name = null, string region = null, Stream icon = null, ImageType iconType = ImageType.Png)
        {
            var request = new UpdateGuildRequest(Id)
            {
                Name = name ?? Name,
                Region = region ?? Region.Id,
                IconBase64 = icon.Base64(iconType, IconId),
                AFKChannelId = AFKChannel?.Id,
                AFKTimeout = AFKTimeout
            };
            return Client.ClientAPI.Send(request);
        }

        /// <summary> Leaves this server. This function will fail if you're the owner - use Delete instead. </summary>
        public async Task Leave()
        {
            if (_ownerId == CurrentUser.Id)
                throw new InvalidOperationException("Unable to leave a server you own, use Server.Delete instead");
            try { await Client.ClientAPI.Send(new LeaveGuildRequest(Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
        /// <summary> Deletes this server. This function will fail if you're not the owner - use Leave instead. </summary>
        public async Task Delete()
        {
            if (_ownerId != CurrentUser.Id)
                throw new InvalidOperationException("Unable to delete a server you don't own, use Server.Leave instead");
            try { await Client.ClientAPI.Send(new LeaveGuildRequest(Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        #region Bans
        internal void AddBan(ulong banId)
            => _bans.TryAdd(banId, true);
        internal bool RemoveBan(ulong banId)
        {
            bool ignored;
            return _bans.TryRemove(banId, out ignored);
        }

        public Task Ban(User user, int pruneDays = 0)
        {
            var request = new AddGuildBanRequest(user.Server.Id, user.Id);
            request.PruneDays = pruneDays;
            return Client.ClientAPI.Send(request);
        }
        public Task Unban(User user, int pruneDays = 0)
            => Unban(user.Id);
        public async Task Unban(ulong userId)
        {
            try { await Client.ClientAPI.Send(new RemoveGuildBanRequest(Id, userId)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
        #endregion

        #region Channels
        internal Channel AddChannel(ulong id)
        {
            var channel = _channels.GetOrAdd(id, x => new Channel(Client, x, this));
            Client.AddChannel(channel);
            return channel;
        }
        internal Channel RemoveChannel(ulong id)
        {
            Channel channel;
            _channels.TryRemove(id, out channel);
            return channel;
        }

        /// <summary> Gets the channel with the provided id and owned by this server, or null if not found. </summary>
        public Channel GetChannel(ulong id)
        {
            Channel result;
            _channels.TryGetValue(id, out result);
            return result;
        }

        /// <summary> Returns all channels with the specified server and name. </summary>
        /// <remarks> Name formats supported: Name, #Name and &lt;#Id&gt;. Search is case-insensitive if exactMatch is false.</remarks>
        public IEnumerable<Channel> FindChannels(string name, ChannelType type = null, bool exactMatch = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));

            return _channels.Select(x => x.Value).Find(name, type, exactMatch);
        }

        /// <summary> Creates a new channel. </summary>
        public async Task<Channel> CreateChannel(string name, ChannelType type)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var request = new CreateChannelRequest(Id) { Name = name, Type = type.Value };
            var response = await Client.ClientAPI.Send(request).ConfigureAwait(false);

            var channel = AddChannel(response.Id);
            channel.Update(response);
            return channel;
        }

        /// <summary> Reorders the provided channels and optionally places them after a certain channel. </summary>
        public Task ReorderChannels(IEnumerable<Channel> channels, Channel after = null)
        {
            if (channels == null) throw new ArgumentNullException(nameof(channels));

            var request = new ReorderChannelsRequest(Id)
            {
                ChannelIds = channels.Select(x => x.Id).ToArray(),
                StartPos = after != null ? after.Position + 1 : channels.Min(x => x.Position)
            };
            return Client.ClientAPI.Send(request);
        }
        #endregion

        #region Invites
        /// <summary> Gets all active (non-expired) invites to this server. </summary>
        public async Task<IEnumerable<Invite>> GetInvites()
        {
            var response = await Client.ClientAPI.Send(new GetInvitesRequest(Id)).ConfigureAwait(false);
            return response.Select(x =>
            {
                var invite = new Invite(Client, x.Code, x.XkcdPass);
                invite.Update(x);
                return invite;
            });
        }

        /// <summary> Creates a new invite to the default channel of this server. </summary>
        /// <param name="maxAge"> Time (in seconds) until the invite expires. Set to null to never expire. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to null to have unlimited uses. </param>
        /// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
        /// <param name="withXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to null. </param>
        public Task<Invite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false)
            => DefaultChannel.CreateInvite(maxAge, maxUses, tempMembership, withXkcd);
        #endregion

        #region Roles
        internal Role AddRole(ulong id)
            => _roles.GetOrAdd(id, x => new Role(x, this));
        internal Role RemoveRole(ulong id)
        {
            Role role;
            _roles.TryRemove(id, out role);
            return role;
        }

        /// <summary> Gets the role with the provided id and owned by this server, or null if not found. </summary>
        public Role GetRole(ulong id)
        {
            Role result;
            _roles.TryGetValue(id, out result);
            return result;
        }
        /// <summary> Returns all roles with the specified server and name. </summary>
        /// <remarks> Search is case-insensitive if exactMatch is false.</remarks>
        public IEnumerable<Role> FindRoles(string name, bool exactMatch = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return _roles.Select(x => x.Value).Find(name, exactMatch);
        }
        
        /// <summary> Creates a new role. </summary>
        public async Task<Role> CreateRole(string name, ServerPermissions permissions = null, Color color = null, bool isHoisted = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var createRequest = new CreateRoleRequest(Id);
            var createResponse = await Client.ClientAPI.Send(createRequest).ConfigureAwait(false);
            var role = AddRole(createResponse.Id);
            role.Update(createResponse);

            var editRequest = new UpdateRoleRequest(role.Server.Id, role.Id)
            {
                Name = name,
                Permissions = (permissions ?? role.Permissions).RawValue,
                Color = (color ?? Color.Default).RawValue,
                IsHoisted = isHoisted
            };
            var editResponse = await Client.ClientAPI.Send(editRequest).ConfigureAwait(false);
            role.Update(editResponse);

            return role;
        }

        /// <summary> Reorders the provided roles and optionally places them after a certain role. </summary>
        public Task ReorderRoles(IEnumerable<Role> roles, Role after = null)
        {
            if (roles == null) throw new ArgumentNullException(nameof(roles));

            return Client.ClientAPI.Send(new ReorderRolesRequest(Id)
            {
                RoleIds = roles.Select(x => x.Id).ToArray(),
                StartPos = after != null ? after.Position + 1 : roles.Min(x => x.Position)
            });
        }
        #endregion

        #region Permissions
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
        #endregion

        #region Users
        internal User AddUser(ulong id)
        {
            Member user;
            if (_users.TryGetOrAdd(id, x => new Member(new User(Client, x, this)), out user))
            {
                foreach (var channel in AllChannels)
                    channel.AddUser(user.User);
                if (id == Client.CurrentUser.Id)
                {
                    user.User.CurrentGame = Client.CurrentGame;
                    user.User.Status = Client.Status;
                }
            }
            return user.User;
        }
        internal User RemoveUser(ulong id)
        {
            Member member;
            if (_users.TryRemove(id, out member))
            {
                foreach (var channel in AllChannels)
                    channel.RemoveUser(id);
            }
            return member.User;
        }

        /// <summary> Gets the user with the provided id and is a member of this server, or null if not found. </summary>
        public User GetUser(ulong id)
        {
            Member result;
            _users.TryGetValue(id, out result);
            return result.User;
        }
        /// <summary> Gets the user with the provided username and discriminator, that is a member of this server, or null if not found. </summary>
        public User GetUser(string name, ushort discriminator)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return _users.Select(x => x.Value.User).Find(name, discriminator: discriminator, exactMatch: false).FirstOrDefault();
        }
        /// <summary> Returns all members of this server with the specified name. </summary>
        /// <remarks> Name formats supported: Name, @Name and &lt;@Id&gt;. Search is case-insensitive if exactMatch is false.</remarks>
        public IEnumerable<User> FindUsers(string name, bool exactMatch = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return _users.Select(x => x.Value.User).Find(name, exactMatch: exactMatch);
        }

        /// <summary> Kicks all users with an inactivity greater or equal to the provided number of days. </summary>
        /// <param name="simulate">If true, no pruning will actually be done but instead return the number of users that would be pruned. </param>
        public async Task<int> PruneUsers(int days = 30, bool simulate = false)
        {
            if (days <= 0) throw new ArgumentOutOfRangeException(nameof(days));

            var request = new PruneMembersRequest(Id)
            {
                Days = days,
                IsSimulation = simulate
            };
            var response = await Client.ClientAPI.Send(request).ConfigureAwait(false);
            return response.Pruned;
        }

        /// <summary>When Config.UseLargeThreshold is enabled, running this command will request the Discord server to provide you with all offline users for this server.</summary>
        public void RequestOfflineUsers()
            => Client.GatewaySocket.SendRequestMembers(Id, "", 0);
        #endregion

        public override bool Equals(object obj) => obj is Server && (obj as Server).Id == Id;
		public override int GetHashCode() => unchecked(Id.GetHashCode() + 5175);
		public override string ToString() => Name ?? Id.ToIdString();
	}
}
