using Discord.API.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Guild;
using EmbedModel = Discord.API.GuildEmbed;
using RoleModel = Discord.API.Role;

namespace Discord.Rest
{
    /// <summary> Represents a Discord guild (called a server in the official client). </summary>
    public class Guild : IGuild
    {
        private ConcurrentDictionary<ulong, Role> _roles;
        private string _iconId, _splashId;

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
        public ulong? AFKChannelId { get; private set; }
        /// <inheritdoc />
        public ulong? EmbedChannelId { get; private set; }
        /// <inheritdoc />
        public ulong OwnerId { get; private set; }
        /// <inheritdoc />
        public string VoiceRegionId { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<Emoji> Emojis { get; private set; }
        /// <inheritdoc />
        public IReadOnlyList<string> Features { get; private set; }

        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeHelper.FromSnowflake(Id);
        /// <inheritdoc />
        public string IconUrl => API.CDN.GetGuildIconUrl(Id, _iconId);
        /// <inheritdoc />
        public string SplashUrl => API.CDN.GetGuildSplashUrl(Id, _splashId);
        /// <inheritdoc />
        public ulong DefaultChannelId => Id;
        /// <inheritdoc />
        public Role EveryoneRole => GetRole(Id);
        /// <summary> Gets a collection of all roles in this guild. </summary>
        public IEnumerable<Role> Roles => _roles?.Select(x => x.Value) ?? Enumerable.Empty<Role>();

        internal Guild(DiscordClient discord, Model model)
        {
            Id = model.Id;
            Discord = discord;

            Update(model);
        }
        private void Update(Model model)
        {
            AFKChannelId = model.AFKChannelId;
            AFKTimeout = model.AFKTimeout;
            EmbedChannelId = model.EmbedChannelId;
            IsEmbeddable = model.EmbedEnabled;
            Features = model.Features;
            _iconId = model.Icon;
            Name = model.Name;
            OwnerId = model.OwnerId;
            VoiceRegionId = model.Region;
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
        private void Update(EmbedModel model)
        {
            IsEmbeddable = model.Enabled;
            EmbedChannelId = model.ChannelId;
        }
        private void Update(IEnumerable<RoleModel> models)
        {
            Role role;
            foreach (var model in models)
            {
                if (_roles.TryGetValue(model.Id, out role))
                    role.Update(model);
            }
        }

        /// <inheritdoc />
        public async Task Update()
        {
            var response = await Discord.BaseClient.GetGuild(Id).ConfigureAwait(false);
            Update(response);
        }
        /// <inheritdoc />
        public async Task Modify(Action<ModifyGuildParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildParams();
            func(args);
            var model = await Discord.BaseClient.ModifyGuild(Id, args).ConfigureAwait(false);
            Update(model);
        }
        /// <inheritdoc />
        public async Task ModifyEmbed(Action<ModifyGuildEmbedParams> func)
        { 
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildEmbedParams();
            func(args);
            var model = await Discord.BaseClient.ModifyGuildEmbed(Id, args).ConfigureAwait(false);
            Update(model);
        }
        /// <inheritdoc />
        public async Task ModifyChannels(IEnumerable<ModifyGuildChannelsParams> args)
        {
            await Discord.BaseClient.ModifyGuildChannels(Id, args).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task ModifyRoles(IEnumerable<ModifyGuildRolesParams> args)
        {            
            var models = await Discord.BaseClient.ModifyGuildRoles(Id, args).ConfigureAwait(false);
            Update(models);
        }
        /// <inheritdoc />
        public async Task Leave()
        {
            await Discord.BaseClient.LeaveGuild(Id).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task Delete()
        {
            await Discord.BaseClient.DeleteGuild(Id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetBans()
        {
            var models = await Discord.BaseClient.GetGuildBans(Id).ConfigureAwait(false);
            return models.Select(x => new PublicUser(Discord, x));
        }
        /// <inheritdoc />
        public Task AddBan(IUser user, int pruneDays = 0) => AddBan(user, pruneDays);
        /// <inheritdoc />
        public async Task AddBan(ulong userId, int pruneDays = 0)
        {
            var args = new CreateGuildBanParams()
            {
                PruneDays = pruneDays
            };
            await Discord.BaseClient.CreateGuildBan(Id, userId, args).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public Task RemoveBan(IUser user) => RemoveBan(user.Id);
        /// <inheritdoc />
        public async Task RemoveBan(ulong userId)
        {
            await Discord.BaseClient.RemoveGuildBan(Id, userId).ConfigureAwait(false);
        }

        /// <summary> Gets the channel in this guild with the provided id, or null if not found. </summary>
        public async Task<GuildChannel> GetChannel(ulong id)
        {
            var model = await Discord.BaseClient.GetChannel(Id, id).ConfigureAwait(false);
            if (model != null)
                return ToChannel(model);
            return null;
        }
        /// <summary> Gets a collection of all channels in this guild. </summary>
        public async Task<IEnumerable<GuildChannel>> GetChannels()
        {
            var models = await Discord.BaseClient.GetGuildChannels(Id).ConfigureAwait(false);
            return models.Select(x => ToChannel(x));
        }
        /// <summary> Creates a new text channel. </summary>
        public async Task<TextChannel> CreateTextChannel(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var args = new CreateGuildChannelParams() { Name = name, Type = ChannelType.Text };
            var model = await Discord.BaseClient.CreateGuildChannel(Id, args).ConfigureAwait(false);
            return new TextChannel(this, model);
        }
        /// <summary> Creates a new voice channel. </summary>
        public async Task<VoiceChannel> CreateVoiceChannel(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var args = new CreateGuildChannelParams { Name = name, Type = ChannelType.Voice };
            var model = await Discord.BaseClient.CreateGuildChannel(Id, args).ConfigureAwait(false);
            return new VoiceChannel(this, model);
        }

        /// <summary> Gets a collection of all integrations attached to this guild. </summary>
        public async Task<IEnumerable<GuildIntegration>> GetIntegrations()
        {
            var models = await Discord.BaseClient.GetGuildIntegrations(Id).ConfigureAwait(false);
            return models.Select(x => new GuildIntegration(this, x));
        }
        /// <summary> Creates a new integration for this guild. </summary>
        public async Task<GuildIntegration> CreateIntegration(ulong id, string type)
        {
            var args = new CreateGuildIntegrationParams { Id = id, Type = type };
            var model = await Discord.BaseClient.CreateGuildIntegration(Id, args).ConfigureAwait(false);
            return new GuildIntegration(this, model);
        }

        /// <summary> Gets a collection of all invites to this guild. </summary>
        public async Task<IEnumerable<InviteMetadata>> GetInvites()
        {
            var models = await Discord.BaseClient.GetGuildInvites(Id).ConfigureAwait(false);
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
            var model = await Discord.BaseClient.CreateChannelInvite(DefaultChannelId, args).ConfigureAwait(false);
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
            
            var model = await Discord.BaseClient.CreateGuildRole(Id).ConfigureAwait(false);
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

        /// <summary> Gets a collection of all users in this guild. </summary>
        public async Task<IEnumerable<GuildUser>> GetUsers()
        {
            var args = new GetGuildMembersParams();
            var models = await Discord.BaseClient.GetGuildMembers(Id, args).ConfigureAwait(false);
            return models.Select(x => new GuildUser(this, x));
        }
        /// <summary> Gets a paged collection of all users in this guild. </summary>
        public async Task<IEnumerable<GuildUser>> GetUsers(int limit, int offset)
        {
            var args = new GetGuildMembersParams { Limit = limit, Offset = offset };
            var models = await Discord.BaseClient.GetGuildMembers(Id, args).ConfigureAwait(false);
            return models.Select(x => new GuildUser(this, x));
        }
        /// <summary> Gets the user in this guild with the provided id, or null if not found. </summary>
        public async Task<GuildUser> GetUser(ulong id)
        {
            var model = await Discord.BaseClient.GetGuildMember(Id, id).ConfigureAwait(false);
            if (model != null)
                return new GuildUser(this, model);
            return null;
        }
        /// <summary> Gets a the current user. </summary>
        public async Task<GuildUser> GetCurrentUser()
        {
            var currentUser = await Discord.GetCurrentUser().ConfigureAwait(false);
            return await GetUser(currentUser.Id).ConfigureAwait(false);
        }
        public async Task<int> PruneUsers(int days = 30, bool simulate = false)
        {
            var args = new GuildPruneParams() { Days = days };
            GetGuildPruneCountResponse model;
            if (simulate)
                model = await Discord.BaseClient.GetGuildPruneCount(Id, args).ConfigureAwait(false);
            else
                model = await Discord.BaseClient.BeginGuildPrune(Id, args).ConfigureAwait(false);
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

        public override string ToString() => Name ?? Id.ToString();

        IEnumerable<Emoji> IGuild.Emojis => Emojis;
        ulong IGuild.EveryoneRoleId => EveryoneRole.Id;
        IEnumerable<string> IGuild.Features => Features;

        async Task<IEnumerable<IUser>> IGuild.GetBans()
            => await GetBans().ConfigureAwait(false);
        async Task<IGuildChannel> IGuild.GetChannel(ulong id)
            => await GetChannel(id).ConfigureAwait(false);
        async Task<IEnumerable<IGuildChannel>> IGuild.GetChannels()
            => await GetChannels().ConfigureAwait(false);
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
        async Task<IGuildUser> IGuild.GetUser(ulong id)
            => await GetUser(id).ConfigureAwait(false);
        async Task<IGuildUser> IGuild.GetCurrentUser()
            => await GetCurrentUser().ConfigureAwait(false);
        async Task<IEnumerable<IGuildUser>> IGuild.GetUsers()
            => await GetUsers().ConfigureAwait(false);
    }
}
