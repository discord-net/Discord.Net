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
	public class Server
    {
        private readonly static Action<Server, Server> _cloner = DynamicIL.CreateCopyMethod<Server>();

        internal static string GetIconUrl(ulong serverId, string iconId)
            => iconId != null ? $"{DiscordConfig.ClientAPIUrl}guilds/{serverId}/icons/{iconId}.jpg" : null;
        internal static string GetSplashUrl(ulong serverId, string splashId)
            => splashId != null ? $"{DiscordConfig.ClientAPIUrl}guilds/{serverId}/splashes/{splashId}.jpg" : null;

        public class Emoji
        {
            public string Id { get; }

            public string Name { get; internal set; }
            public bool IsManaged { get; internal set; }
            public bool RequireColons { get; internal set; }
            public IEnumerable<Role> Roles { get; internal set; }

            internal Emoji(string id)
            {
                Id = id;
            }
        }
        private struct Member
        {
            public readonly User User;
            public readonly ServerPermissions Permissions;
            public Member(User user, ServerPermissions permissions)
            {
                User = user;
                Permissions = permissions;
            }
        }

        private ConcurrentDictionary<ulong, Role> _roles;
        private ConcurrentDictionary<ulong, Member> _users;
        private ConcurrentDictionary<ulong, Channel> _channels;
        private ulong _ownerId;
        private ulong? _afkChannelId;
        private int _userCount;

        public DiscordClient Client { get; }

        /// <summary> Gets the unique identifier for this server. </summary>
        public ulong Id { get; }

        /// <summary> Gets the name of this server. </summary>
        public string Name { get; private set; }
        /// <summary> Gets the voice region for this server. </summary>
        public Region Region { get; private set; }
        /// <summary> Gets the unique identifier for this user's current avatar. </summary>
        public string IconId { get; private set; }
        /// <summary> Gets the unique identifier for this server's custom splash image. </summary>
        public string SplashId { get; private set; }
        /// <summary> Gets the amount of time (in seconds) a user must be inactive for until they are automatically moved to the AFK voice channel, if one is set. </summary>
        public int AFKTimeout { get; private set; }
        /// <summary> Gets the date and time you joined this server. </summary>
        public DateTime JoinedAt { get; private set; }
        /// <summary> Gets all extra features added to this server. </summary>
        public IEnumerable<string> Features { get; private set; }
        /// <summary> Gets all custom emojis on this server. </summary>
        public IEnumerable<Emoji> CustomEmojis { get; private set; }

        /// <summary> Gets the path to this object. </summary>
        internal string Path => Name;
        /// <summary> Gets the user that created this server. </summary>
        public User Owner => GetUser(_ownerId);
        /// <summary> Returns true if the current user owns this server. </summary>
        public bool IsOwner => _ownerId == Client.CurrentUser.Id;
        /// <summary> Gets the AFK voice channel for this server. </summary>
        public Channel AFKChannel => _afkChannelId != null ? GetChannel(_afkChannelId.Value) : null;
        /// <summary> Gets the current user in this server. </summary>
        public User CurrentUser => GetUser(Client.CurrentUser.Id);
        /// <summary> Gets the URL to this server's current icon. </summary>
        public string IconUrl => GetIconUrl(Id, IconId);
        /// <summary> Gets the URL to this servers's splash image. </summary>
        public string SplashUrl => GetSplashUrl(Id, SplashId);
        /// <summary> Gets the default channel for this server. </summary>
        public Channel DefaultChannel => GetChannel(Id);
        /// <summary> Gets the the role representing all users in a server. </summary>
        public Role EveryoneRole => GetRole(Id);

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

        /// <summary> Gets the number of channels in this server. </summary>
        public int ChannelCount => _channels.Count;
        /// <summary> Gets the number of users downloaded for this server so far. </summary>
        internal int CurrentUserCount => _users.Count;
        /// <summary> Gets the number of users in this server. </summary>
        public int UserCount => _userCount;
        /// <summary> Gets the number of roles in this server. </summary>
        public int RoleCount => _roles.Count;

        internal Server(DiscordClient client, ulong id)
        {
            Client = client;
            Id = id;
        }

        internal void Update(Guild model)
        {
            if (model.Name != null)
                Name = model.Name;
            if (model.AFKTimeout != null)
                AFKTimeout = model.AFKTimeout.Value;
            if (model.JoinedAt != null)
                JoinedAt = model.JoinedAt.Value;
            if (model.OwnerId != null)
                _ownerId = model.OwnerId.Value;
            if (model.Region != null)
                Region = Client.GetRegion(model.Region);
            if (model.Icon != null)
                IconId = model.Icon;
            if (model.Features != null)
                Features = model.Features;
            if (model.Roles != null)
            {
                _roles = new ConcurrentDictionary<ulong, Role>(2, model.Roles.Length);
                foreach (var x in model.Roles)
                {
                    var role = AddRole(x.Id);
                    role.Update(x, false);
                }
            }
            if (model.Emojis != null) //Needs Roles
            {
                CustomEmojis = model.Emojis.Select(x => new Emoji(x.Id)
                {
                    Name = x.Name,
                    IsManaged = x.IsManaged,
                    RequireColons = x.RequireColons,
                    Roles = x.RoleIds.Select(y => GetRole(y)).Where(y => y != null).ToArray()
                }).ToArray();
            }

            //Can be null
            _afkChannelId = model.AFKChannelId;
            SplashId = model.Splash;
        }
        internal void Update(ExtendedGuild model)
        {
            Update(model as Guild);

            //Only channels or members should have AddXXX(cachePerms: true), not both
            if (model.Channels != null)
            {
                _channels = new ConcurrentDictionary<ulong, Channel>(2, (int)(model.Channels.Length * 1.05));
                foreach (var subModel in model.Channels)
                    AddChannel(subModel.Id, false).Update(subModel);
            }
            if (model.MemberCount != null)
            {
                if (_users == null)
                    _users = new ConcurrentDictionary<ulong, Member>(2, (int)(model.MemberCount * 1.05));
                _userCount = model.MemberCount.Value;
            }
            if (!model.IsLarge)
            {
                if (model.Members != null)
                {
                    foreach (var subModel in model.Members)
                        AddUser(subModel.User.Id, true, false).Update(subModel);
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
                AFKTimeout = AFKTimeout,
                Splash = SplashId
            };
            return Client.ClientAPI.Send(request);
        }

        /// <summary> Leaves this server. This function will fail if you're the owner - use Delete instead. </summary>
        public async Task Leave()
        {
            try { await Client.ClientAPI.Send(new LeaveGuildRequest(Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
        /// <summary> Deletes this server. This function will fail if you're not the owner - use Leave instead. </summary>
        public async Task Delete()
        {
            try { await Client.ClientAPI.Send(new DeleteGuildRequest(Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        #region Bans
        public async Task<IEnumerable<User>> GetBans()
        {
            var response = await Client.ClientAPI.Send(new GetBansRequest(Id)).ConfigureAwait(false);
            return response.Select(x =>
            {
                var user = new User(Client, x.Id, this);
                user.Update(x);
                return user;
            });
        }

        public Task Ban(User user, int pruneDays = 0)
        {
            var request = new AddGuildBanRequest(user.Server.Id, user.Id)
            {
                PruneDays = pruneDays
            };
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
        internal Channel AddChannel(ulong id, bool cachePerms)
        {
            var channel = new Channel(Client, id, this);
            if (cachePerms && Client.Config.UsePermissionsCache)
            {
                foreach (var user in Users)
                    channel.AddUser(user);
            }
            Client.AddChannel(channel);
            return _channels.GetOrAdd(id, x => channel);
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

            return _channels.Select(x => x.Value).Find(name, type, exactMatch);
        }

        /// <summary> Creates a new channel. </summary>
        public async Task<Channel> CreateChannel(string name, ChannelType type)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var request = new CreateChannelRequest(Id) { Name = name, Type = type.Value };
            var response = await Client.ClientAPI.Send(request).ConfigureAwait(false);

            var channel = AddChannel(response.Id, true);
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
        public async Task<Role> CreateRole(string name, ServerPermissions? permissions = null, Color color = null, bool isHoisted = false, bool isMentionable = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var createRequest = new CreateRoleRequest(Id);
            var createResponse = await Client.ClientAPI.Send(createRequest).ConfigureAwait(false);
            var role = AddRole(createResponse.Id);
            role.Update(createResponse, false);

            var editRequest = new UpdateRoleRequest(role.Server.Id, role.Id)
            {
                Name = name,
                Permissions = (permissions ?? role.Permissions).RawValue,
                Color = (color ?? Color.Default).RawValue,
                IsHoisted = isHoisted,
                IsMentionable = isMentionable
            };
            var editResponse = await Client.ClientAPI.Send(editRequest).ConfigureAwait(false);
            role.Update(editResponse, true);

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
                return ServerPermissions.None;
        }

        internal void UpdatePermissions(User user)
        {
            Member member;
            if (_users.TryGetValue(user.Id, out member))
            {
                var perms = member.Permissions;
                if (UpdatePermissions(member.User, ref perms))
                {
                    _users[user.Id] = new Member(member.User, perms);
                    foreach (var channel in _channels)
                        channel.Value.UpdatePermissions(user);
                }
            }
        }

        private bool UpdatePermissions(User user, ref ServerPermissions permissions)
        {
            uint newPermissions = 0;

            if (user.Id == _ownerId)
                newPermissions = ServerPermissions.All.RawValue;
            else
            {
                foreach (var serverRole in user.Roles)
                    newPermissions |= serverRole.Permissions.RawValue;
            }

            if (newPermissions.HasBit((byte)PermissionBits.ManageRolesOrPermissions))
                newPermissions = ServerPermissions.All.RawValue;

            if (newPermissions != permissions.RawValue)
            {
                permissions = new ServerPermissions(newPermissions);
                return true;
            }
            return false;
        }
        #endregion

        #region Users
        internal User AddUser(ulong id, bool cachePerms, bool incrementCount)
        {
            if (incrementCount)
                _userCount++;

            Member member;
            if (!_users.TryGetValue(id, out member)) //Users can only be added from websocket thread, ignore threadsafety
            {
                member = new Member(new User(Client, id, this), ServerPermissions.None);
                if (id == Client.CurrentUser.Id)
                {
                    member.User.CurrentGame = Client.CurrentGame;
                    member.User.Status = Client.Status;
                }

                _users[id] = member;
                if (cachePerms && Client.Config.UsePermissionsCache)
                {
                    foreach (var channel in _channels)
                        channel.Value.AddUser(member.User);
                }
            }
            return member.User;
        }
        internal User RemoveUser(ulong id)
        {
            _userCount--;
            Member member;
            if (_users.TryRemove(id, out member))
            {
                foreach (var channel in _channels)
                    channel.Value.RemoveUser(id);
                return member.User;
            }
            return null;
        }

        /// <summary> Gets the user with the provided id and is a member of this server, or null if not found. </summary>
        public User GetUser(ulong id)
        {
            Member result;
            if (_users.TryGetValue(id, out result))
                return result.User;
            else
                return null;
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
        #endregion

        internal Server Clone()
        {
            var result = new Server();
            _cloner(this, result);
            return result;
        }
        private Server() { } //Used for cloning

        public override string ToString() => Name ?? Id.ToIdString();
    }
}
