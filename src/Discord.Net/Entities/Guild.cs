using Discord.API.Rest;
using Discord.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Model = Discord.API.Guild;

namespace Discord
{
    /// <summary> Represents a Discord guild (called a server in the official client). </summary>
    public class Guild : IEntity<ulong>
    {
        private struct Member
        {
            public readonly GuildUser User;
            public readonly GuildPermissions Permissions;
            public Member(GuildUser user, GuildPermissions permissions)
            {
                User = user;
                Permissions = permissions;
            }
        }

        private ConcurrentDictionary<ulong, GuildChannel> _channels;
        private ConcurrentDictionary<ulong, Member> _members;
        private ConcurrentDictionary<ulong, GuildPresence> _presences;
        private ConcurrentDictionary<ulong, Role> _roles;
        private ConcurrentDictionary<ulong, VoiceState> _voiceStates;
        private ulong _ownerId;
        private ulong? _afkChannelId, _embedChannelId;
        private int _userCount;
        private string _iconId, _splashId;

        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public DiscordClient Discord { get; }
        public GuildUser CurrentUser { get; }

        /// <summary> Gets the name of this guild. </summary>
        public string Name { get; private set; }
        /// <summary> Gets the amount of time (in seconds) a user must be inactive in a voice channel for until they are automatically moved to the AFK voice channel, if one is set. </summary>
        public int AFKTimeout { get; private set; }
        /// <summary> Returns true if this guild is embeddable (e.g. widget) </summary>
        public bool IsEmbeddable { get; private set; }

        /// <summary> Gets a list of all custom emojis for this guild. </summary>
        public IReadOnlyList<Emoji> Emojis { get; private set; }
        /// <summary> Gets a list of extra features added to this guild. </summary>
        public IReadOnlyList<string> Features { get; private set; }

        /// <summary> Gets the voice region for this guild. </summary>
        public VoiceRegion Region { get; private set; }
        /*/// <summary> Gets the date and time you joined this guild. </summary>
        public DateTime JoinedAt { get; private set; }*/
        /// <summary> Gets the the role representing all users in a guild. </summary>
        public Role EveryoneRole { get; private set; }

        /// <summary> Gets the number of channels in this guild. </summary>
        public int ChannelCount => _channels.Count;
        /// <summary> Gets the number of roles in this guild. </summary>
        public int RoleCount => _roles.Count;
        /// <summary> Gets the number of users in this guild. </summary>
        public int UserCount => _userCount;
        /// <summary> Gets the number of users downloaded for this guild so far. </summary>
        internal int CurrentUserCount => _members.Count;

        /// <summary> Gets the URL to this guild's current icon. </summary>
        public string IconUrl => CDN.GetGuildIconUrl(Id, _iconId);
        /// <summary> Gets the URL to this guild's splash image. </summary>
        public string SplashUrl => CDN.GetGuildSplashUrl(Id, _splashId);

        /// <summary> Gets the user that created this guild. </summary>
        public GuildUser Owner => GetUser(_ownerId);
        /// <summary> Gets the default channel for this guild. </summary>
        public TextChannel DefaultChannel => GetChannel(Id) as TextChannel;
        /// <summary> Gets the AFK voice channel for this guild. </summary>
        public VoiceChannel AFKChannel => GetChannel(_afkChannelId) as VoiceChannel;
        /// <summary> Gets the embed channel for this guild. </summary>
        public IChannel EmbedChannel => GetChannel(_embedChannelId); //TODO: Is this text or voice?
        /// <summary> Gets a collection of all channels in this guild. </summary>
        public IEnumerable<GuildChannel> Channels => _channels.Select(x => x.Value);
        /// <summary> Gets a collection of text channels in this guild. </summary>
        public IEnumerable<TextChannel> TextChannels => _channels.Select(x => x.Value).OfType<TextChannel>();
        /// <summary> Gets a collection of voice channels in this guild. </summary>
        public IEnumerable<VoiceChannel> VoiceChannels => _channels.Select(x => x.Value).OfType<VoiceChannel>();
        /// <summary> Gets a collection of all members in this guild. </summary>
        public IEnumerable<GuildUser> Users => _members.Select(x => x.Value.User);
        /// <summary> Gets a collection of all roles in this guild. </summary>
        public IEnumerable<Role> Roles => _roles.Select(x => x.Value);
        
        internal Guild(ulong id, DiscordClient client)
        {
            Id = id;
            Discord = client;

            _channels = new ConcurrentDictionary<ulong, GuildChannel>();
            _members = new ConcurrentDictionary<ulong, Member>();
            _presences = new ConcurrentDictionary<ulong, GuildPresence>();
            _roles = new ConcurrentDictionary<ulong, Role>();
            _voiceStates = new ConcurrentDictionary<ulong, VoiceState>();
        }

        internal void Update(Model model)
        {
            Name = model.Name;
            AFKTimeout = model.AFKTimeout;
            _ownerId = model.OwnerId;
            _afkChannelId = model.AFKChannelId;
            Region = Discord.GetVoiceRegion(model.Region);
            _iconId = model.Icon;
            _splashId = model.Splash;
            Features = model.Features;
            IsEmbeddable = model.EmbedEnabled;
            _embedChannelId = model.EmbedChannelId;
            _userCount = 0;// model.UserCount;

            _roles = new ConcurrentDictionary<ulong, Role>(2, model.Roles.Length);
            foreach (var x in model.Roles)
                _roles[x.Id] = Discord.CreateRole(this, x);
            EveryoneRole = _roles[Id];

            Emojis = model.Emojis.Select(x => new Emoji(x, this)).ToArray();
        }
        /*internal void Update(ExtendedModel model)
        {
            Update(model as Model);

            //Only channels or members should have AddXXX(cachePerms: true), not both
            if (model.Channels != null)
            {
                _channels = new ConcurrentDictionary<ulong, Channel>(2, (int)(model.Channels.Length * 1.05));
                foreach (var subModel in model.Channels)
                    AddChannel(subModel.Id, false).Update(subModel);
                DefaultChannel = _channels[Id];
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
        }*/

        /// <summary> Gets the channel with the given id, or null if not found. </summary>
        public GuildChannel GetChannel(ulong id)
        {
            GuildChannel result;
            _channels.TryGetValue(id, out result);
            return result;
        }
        /// <summary> Gets the channel refered to by the given mention, or null if not found. </summary>
        public GuildChannel GetChannel(string mention) => GetChannel(MentionHelper.GetChannelId(mention));
        private GuildChannel GetChannel(ulong? id) => id != null ? GetChannel(id.Value) : null;

        /// <summary> Gets the channel with the given id, or null if not found. </summary>
        public Role GetRole(ulong id)
        {
            Role result;
            _roles.TryGetValue(id, out result);
            return result;
        }
        private Role GetRole(ulong? id) => id != null ? GetRole(id.Value) : null;

        public GuildUser GetUser(ulong id)
        {
            Member result;
            if (_members.TryGetValue(id, out result))
                return result.User;
            else
                return null;
        }
        public GuildUser GetUser(string username, ushort discriminator)
        {
            if (username == null) throw new ArgumentNullException(nameof(username));

            foreach (var member in _members)
            {
                var user = member.Value.User;
                if (user.Discriminator == discriminator && user.Username == username)
                    return user;
            }
            return null;
        }
        public GuildUser GetUser(string mention) => GetUser(MentionHelper.GetUserId(mention));
        private GuildUser GetUser(ulong? id) => id != null ? GetUser(id.Value) : null;

        public async Task<IEnumerable<User>> GetBans()
        {
            var discord = Discord;
            var response = await Discord.RestClient.Send(new GetGuildBansRequest(Id)).ConfigureAwait(false);
            return response.Select(x => Discord.CreateBannedUser(this, x));
        }

        public async Task<IEnumerable<PublicInvite>> GetInvites()
        {
            var response = await Discord.RestClient.Send(new GetGuildInvitesRequest(Id)).ConfigureAwait(false);
            return response.Select(x =>
            {
                var invite = Discord.CreatePublicInvite(x);
                invite.Update(x);
                return invite;
            });
        }

        public async Task<TextChannel> CreateTextChannel(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var request = new CreateChannelRequest(Id) { Name = name, Type = ChannelType.Text };
            var response = await Discord.RestClient.Send(request).ConfigureAwait(false);

            return Discord.CreateTextChannel(this, response);
        }
        public async Task<VoiceChannel> CreateVoiceChannel(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var request = new CreateChannelRequest(Id) { Name = name, Type = ChannelType.Voice };
            var response = await Discord.RestClient.Send(request).ConfigureAwait(false);

            return Discord.CreateVoiceChannel(this, response);
        }
        public Task<PublicInvite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false)
        {
            return DefaultChannel.CreateInvite(maxAge, maxUses, tempMembership, withXkcd);
        }
        public async Task<Role> CreateRole(string name, GuildPermissions? permissions = null, Color color = null, bool isHoisted = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var createRequest = new CreateRoleRequest(Id);
            var createResponse = await Discord.RestClient.Send(createRequest).ConfigureAwait(false);
            var role = Discord.CreateRole(this, createResponse);

            var editRequest = new ModifyGuildRoleRequest(role.Guild.Id, role.Id)
            {
                Name = name,
                Permissions = (int)(permissions ?? role.Permissions).RawValue,
                Color = (int)(color ?? Color.Default).RawValue,
                Hoist = isHoisted
            };
            var editResponse = await Discord.RestClient.Send(editRequest).ConfigureAwait(false);
            role.Update(editResponse);

            return role;
        }

        public async Task<int> PruneUsers(int days = 30, bool simulate = false)
        {
            if (simulate)
            {
                var response = await Discord.RestClient.Send(new GetGuildPruneCountRequest(Id) { Days = days }).ConfigureAwait(false);
                return response.Pruned;
            }
            else
            {
                var response = await Discord.RestClient.Send(new BeginGuildPruneRequest(Id) { Days = days }).ConfigureAwait(false);
                return response.Pruned;
            }
        }

        public Task Ban(GuildUser user, int pruneDays = 0)
        {
            return Discord.RestClient.Send(new CreateGuildBanRequest(Id, user.Id)
            {
                PruneDays = pruneDays
            });
        }
        public async Task Unban(GuildUser user)
        {
            try { await Discord.RestClient.Send(new RemoveGuildBanRequest(Id, user.Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        public async Task Update()
        {
            var response = await Discord.RestClient.Send(new GetGuildRequest(Id)).ConfigureAwait(false);
            if (response != null)
                Update(response);
        }

        public async Task Modify(Action<ModifyGuildRequest> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var req = new ModifyGuildRequest(Id);
            func(req);
            await Discord.RestClient.Send(req).ConfigureAwait(false);
        }

        /// <summary> Leaves this guild. </summary>
        public async Task Leave()
        {
            try { await Discord.RestClient.Send(new LeaveGuildRequest(Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
        /// <summary> Deletes this guild. </summary>
        public async Task Delete()
        {
            try { await Discord.RestClient.Send(new DeleteGuildRequest(Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        internal void UpdatePermissions(GuildUser user)
        {
            Member member;
            if (_members.TryGetValue(user.Id, out member))
            {
                var perms = member.Permissions;
                if (UpdatePermissions(member.User, ref perms))
                {
                    _members[user.Id] = new Member(member.User, perms);
                    foreach (var channel in _channels)
                        channel.Value.UpdatePermissions(user);
                }
            }
        }

        private bool UpdatePermissions(GuildUser user, ref GuildPermissions permissions)
        {
            uint newPermissions = 0;

            if (user.Id == _ownerId)
                newPermissions = GuildPermissions.All.RawValue;
            else
            {
                foreach (var role in user.Presence.Roles)
                    newPermissions |= role.Permissions.RawValue;
            }

            if (PermissionsHelper.HasBit(ref newPermissions, (byte)PermissionBit.ManageRolesOrPermissions))
                newPermissions = GuildPermissions.All.RawValue;

            if (newPermissions != permissions.RawValue)
            {
                permissions = new GuildPermissions(newPermissions);
                return true;
            }
            return false;
        }

        /*internal IGuildChannel AddChannel(ulong id, bool cachePerms)
        {
            var channel = new Channel(Discord, id, this);
            if (cachePerms && Discord.UsePermissionsCache)
            {
                foreach (var user in Users)
                    channel.AddUser(user);
            }
            Discord.AddChannel(channel);
            return _channels.GetOrAdd(id, x => channel);
        }
        internal IGuildChannel RemoveChannel(ulong id)
        {
            IGuildChannel channel;
            _channels.TryRemove(id, out channel);
            return channel;
        }*/
    }
}
