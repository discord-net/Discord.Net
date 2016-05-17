using Discord.API.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Guild;
using System.Diagnostics;

namespace Discord.WebSocket
{
    /// <summary> Represents a Discord guild (called a server in the official client). </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Guild : IGuild, IUserGuild
    {
        private ConcurrentDictionary<ulong, GuildChannel> _channels;
        private ConcurrentDictionary<ulong, GuildUser> _members;
        private ConcurrentDictionary<ulong, Role> _roles;
        private ulong _ownerId;
        private ulong? _afkChannelId, _embedChannelId;
        private string _iconId, _splashId;
        private int _userCount;

        /// <inheritdoc />
        public ulong Id { get; }
        internal DiscordClient Discord { get; }

        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public int AFKTimeout { get; private set; }
        /// <inheritdoc />
        public bool IsEmbeddable { get; private set; }
        /// <inheritdoc />
        public int VerificationLevel { get; private set; }
        
        /// <inheritdoc />
        public VoiceRegion VoiceRegion { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<Emoji> Emojis { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<string> Features { get; private set; }
        
        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeUtils.FromSnowflake(Id);

        /// <inheritdoc />
        public string IconUrl => API.CDN.GetGuildIconUrl(Id, _iconId);
        /// <inheritdoc />
        public string SplashUrl => API.CDN.GetGuildSplashUrl(Id, _splashId);

        /// <summary> Gets the number of channels in this guild. </summary>
        public int ChannelCount => _channels.Count;
        /// <summary> Gets the number of roles in this guild. </summary>
        public int RoleCount => _roles.Count;
        /// <summary> Gets the number of users in this guild. </summary>
        public int UserCount => _userCount;
        /// <summary> Gets the number of users downloaded for this guild so far. </summary>
        internal int CurrentUserCount => _members.Count;

        /// <summary> Gets the the role representing all users in a guild. </summary>
        public Role EveryoneRole => GetRole(Id);
        public GuildUser CurrentUser => GetUser(Discord.CurrentUser.Id);
        /// <summary> Gets the user that created this guild. </summary>
        public GuildUser Owner => GetUser(_ownerId);
        /// <summary> Gets the default channel for this guild. </summary>
        public TextChannel DefaultChannel => GetChannel(Id) as TextChannel;
        /// <summary> Gets the AFK voice channel for this guild. </summary>
        public VoiceChannel AFKChannel => GetChannel(_afkChannelId.GetValueOrDefault(0)) as VoiceChannel;
        /// <summary> Gets the embed channel for this guild. </summary>
        public IChannel EmbedChannel => GetChannel(_embedChannelId.GetValueOrDefault(0)); //TODO: Is this text or voice?
        /// <summary> Gets a collection of all channels in this guild. </summary>
        public IEnumerable<GuildChannel> Channels => _channels.Select(x => x.Value);
        /// <summary> Gets a collection of text channels in this guild. </summary>
        public IEnumerable<TextChannel> TextChannels => _channels.Select(x => x.Value).OfType<TextChannel>();
        /// <summary> Gets a collection of voice channels in this guild. </summary>
        public IEnumerable<VoiceChannel> VoiceChannels => _channels.Select(x => x.Value).OfType<VoiceChannel>();
        /// <summary> Gets a collection of all roles in this guild. </summary>
        public IEnumerable<Role> Roles => _roles?.Select(x => x.Value) ?? Enumerable.Empty<Role>();
        /// <summary> Gets a collection of all users in this guild. </summary>
        public IEnumerable<GuildUser> Users => _members.Select(x => x.Value);

        internal Guild(DiscordClient discord, Model model)
        {
            Id = model.Id;
            Discord = discord;

            Update(model);
        }
        private void Update(Model model)
        {
            _afkChannelId = model.AFKChannelId;
            AFKTimeout = model.AFKTimeout;
            _embedChannelId = model.EmbedChannelId;
            IsEmbeddable = model.EmbedEnabled;
            Features = model.Features;
            _iconId = model.Icon;
            Name = model.Name;
            _ownerId = model.OwnerId;
            VoiceRegion = Discord.GetVoiceRegion(model.Region);
            _splashId = model.Splash;
            VerificationLevel = model.VerificationLevel;

            if (model.Emojis != null)
            {
                var emojis = ImmutableArray.CreateBuilder<Emoji>(model.Emojis.Length);
                for (int i = 0; i < model.Emojis.Length; i++)
                    emojis.Add(new Emoji(model.Emojis[i]));
                Emojis = emojis.ToArray();
            }
            else
                Emojis = Array.Empty<Emoji>();

            var roles = new ConcurrentDictionary<ulong, Role>(1, model.Roles?.Length ?? 0);
            if (model.Roles != null)
            {
                for (int i = 0; i < model.Roles.Length; i++)
                    roles[model.Roles[i].Id] = new Role(this, model.Roles[i]);
            }
            _roles = roles;
        }
        
        /// <inheritdoc />
        public async Task Modify(Action<ModifyGuildParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildParams();
            func(args);
            await Discord.ApiClient.ModifyGuild(Id, args).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task ModifyEmbed(Action<ModifyGuildEmbedParams> func)
        { 
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildEmbedParams();
            func(args);
            await Discord.ApiClient.ModifyGuildEmbed(Id, args).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task ModifyChannels(IEnumerable<ModifyGuildChannelsParams> args)
        {
            await Discord.ApiClient.ModifyGuildChannels(Id, args).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task ModifyRoles(IEnumerable<ModifyGuildRolesParams> args)
        {            
            await Discord.ApiClient.ModifyGuildRoles(Id, args).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task Leave()
        {
            await Discord.ApiClient.LeaveGuild(Id).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task Delete()
        {
            await Discord.ApiClient.DeleteGuild(Id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetBans()
        {
            var models = await Discord.ApiClient.GetGuildBans(Id).ConfigureAwait(false);
            return models.Select(x => new User(Discord, x));
        }
        /// <inheritdoc />
        public Task AddBan(IUser user, int pruneDays = 0) => AddBan(user, pruneDays);
        /// <inheritdoc />
        public async Task AddBan(ulong userId, int pruneDays = 0)
        {
            var args = new CreateGuildBanParams() { PruneDays = pruneDays };
            await Discord.ApiClient.CreateGuildBan(Id, userId, args).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public Task RemoveBan(IUser user) => RemoveBan(user.Id);
        /// <inheritdoc />
        public async Task RemoveBan(ulong userId)
        {
            await Discord.ApiClient.RemoveGuildBan(Id, userId).ConfigureAwait(false);
        }

        /// <summary> Gets the channel in this guild with the provided id, or null if not found. </summary>
        public GuildChannel GetChannel(ulong id)
        {
            GuildChannel channel;
            if (_channels.TryGetValue(id, out channel))
                return channel;
            return null;
        }
        /// <summary> Creates a new text channel. </summary>
        public async Task<TextChannel> CreateTextChannel(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var args = new CreateGuildChannelParams() { Name = name, Type = ChannelType.Text };
            var model = await Discord.ApiClient.CreateGuildChannel(Id, args).ConfigureAwait(false);
            return new TextChannel(this, model);
        }
        /// <summary> Creates a new voice channel. </summary>
        public async Task<VoiceChannel> CreateVoiceChannel(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var args = new CreateGuildChannelParams { Name = name, Type = ChannelType.Voice };
            var model = await Discord.ApiClient.CreateGuildChannel(Id, args).ConfigureAwait(false);
            return new VoiceChannel(this, model);
        }
        
        /// <summary> Creates a new integration for this guild. </summary>
        public async Task<GuildIntegration> CreateIntegration(ulong id, string type)
        {
            var args = new CreateGuildIntegrationParams { Id = id, Type = type };
            var model = await Discord.ApiClient.CreateGuildIntegration(Id, args).ConfigureAwait(false);
            return new GuildIntegration(this, model);
        }

        /// <summary> Gets a collection of all invites to this guild. </summary>
        public async Task<IEnumerable<InviteMetadata>> GetInvites()
        {
            var models = await Discord.ApiClient.GetGuildInvites(Id).ConfigureAwait(false);
            return models.Select(x => new InviteMetadata(Discord, x));
        }
        /// <summary> Creates a new invite to this guild. </summary>
        public async Task<InviteMetadata> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool isTemporary = false, bool withXkcd = false)
        {
            if (maxAge <= 0) throw new ArgumentOutOfRangeException(nameof(maxAge));
            if (maxUses <= 0) throw new ArgumentOutOfRangeException(nameof(maxUses));

            var args = new CreateChannelInviteParams()
            {
                MaxAge = maxAge ?? 0,
                MaxUses = maxUses ?? 0,
                Temporary = isTemporary,
                XkcdPass = withXkcd
            };
            var model = await Discord.ApiClient.CreateChannelInvite(Id, args).ConfigureAwait(false);
            return new InviteMetadata(Discord, model);
        }

        /// <summary> Gets the role in this guild with the provided id, or null if not found. </summary>
        public Role GetRole(ulong id)
        {
            Role result = null;
            if (_roles?.TryGetValue(id, out result) == true)
                return result;
            return null;
        }

        /// <summary> Creates a new role. </summary>
        public async Task<Role> CreateRole(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            
            var model = await Discord.ApiClient.CreateGuildRole(Id).ConfigureAwait(false);
            var role = new Role(this, model);

            await role.Modify(x =>
            {
                x.Name = name;
                x.Permissions = (permissions ?? role.Permissions).RawValue;
                x.Color = (color ?? Color.Default).RawValue;
                x.Hoist = isHoisted;
            }).ConfigureAwait(false);

            return role;
        }
        
        /// <summary> Gets the user in this guild with the provided id, or null if not found. </summary>
        public GuildUser GetUser(ulong id)
        {
            GuildUser user;
            if (_members.TryGetValue(id, out user))
                return user;
            return null;
        }
        public async Task<int> PruneUsers(int days = 30, bool simulate = false)
        {
            var args = new GuildPruneParams() { Days = days };
            GetGuildPruneCountResponse model;
            if (simulate)
                model = await Discord.ApiClient.GetGuildPruneCount(Id, args).ConfigureAwait(false);
            else
                model = await Discord.ApiClient.BeginGuildPrune(Id, args).ConfigureAwait(false);
            return model.Pruned;
        }

        internal GuildChannel ToChannel(API.Channel model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                default:
                    return new TextChannel(this, model);
                case ChannelType.Voice:
                    return new VoiceChannel(this, model);
            }
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";

        IEnumerable<Emoji> IGuild.Emojis => Emojis;
        IEnumerable<string> IGuild.Features => Features;
        ulong? IGuild.AFKChannelId => _afkChannelId;
        ulong IGuild.DefaultChannelId => Id;
        ulong? IGuild.EmbedChannelId => _embedChannelId;
        ulong IGuild.EveryoneRoleId => EveryoneRole.Id;
        ulong IGuild.OwnerId => _ownerId;
        string IGuild.VoiceRegionId => VoiceRegion.Id;
        bool IUserGuild.IsOwner => CurrentUser.Id == _ownerId;
        GuildPermissions IUserGuild.Permissions => CurrentUser.GuildPermissions;

        async Task<IEnumerable<IUser>> IGuild.GetBans()
            => await GetBans().ConfigureAwait(false);
        Task<IGuildChannel> IGuild.GetChannel(ulong id)
            => Task.FromResult<IGuildChannel>(GetChannel(id));
        Task<IEnumerable<IGuildChannel>> IGuild.GetChannels()
            => Task.FromResult<IEnumerable<IGuildChannel>>(Channels);
        async Task<IInviteMetadata> IGuild.CreateInvite(int? maxAge, int? maxUses, bool isTemporary, bool withXkcd)
            => await CreateInvite(maxAge, maxUses, isTemporary, withXkcd).ConfigureAwait(false);
        async Task<IRole> IGuild.CreateRole(string name, GuildPermissions? permissions, Color? color, bool isHoisted)
            => await CreateRole(name, permissions, color, isHoisted).ConfigureAwait(false);
        async Task<ITextChannel> IGuild.CreateTextChannel(string name)
            => await CreateTextChannel(name).ConfigureAwait(false);
        async Task<IVoiceChannel> IGuild.CreateVoiceChannel(string name)
            => await CreateVoiceChannel(name).ConfigureAwait(false);
        async Task<IEnumerable<IInviteMetadata>> IGuild.GetInvites()
            => await GetInvites().ConfigureAwait(false);
        Task<IRole> IGuild.GetRole(ulong id)
            => Task.FromResult<IRole>(GetRole(id));
        Task<IEnumerable<IRole>> IGuild.GetRoles()
            => Task.FromResult<IEnumerable<IRole>>(Roles);
        Task<IGuildUser> IGuild.GetUser(ulong id)
            => Task.FromResult<IGuildUser>(GetUser(id));
        Task<IGuildUser> IGuild.GetCurrentUser()
            => Task.FromResult<IGuildUser>(CurrentUser);
        Task<IEnumerable<IGuildUser>> IGuild.GetUsers()
            => Task.FromResult<IEnumerable<IGuildUser>>(Users);
        Task IUpdateable.Update() 
            => Task.CompletedTask;
    }
}
