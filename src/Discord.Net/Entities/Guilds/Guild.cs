using Discord.API.Rest;
using Discord.Audio;
using Discord.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EmbedModel = Discord.API.GuildEmbed;
using Model = Discord.API.Guild;
using RoleModel = Discord.API.Role;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class Guild : SnowflakeEntity, IGuild
    {
        protected ConcurrentDictionary<ulong, Role> _roles;
        protected string _iconId, _splashId;
    
        public string Name { get; private set; }
        public int AFKTimeout { get; private set; }
        public bool IsEmbeddable { get; private set; }
        public VerificationLevel VerificationLevel { get; private set; }
        public MfaLevel MfaLevel { get; private set; }
        public DefaultMessageNotifications DefaultMessageNotifications { get; private set; }

        public override DiscordRestClient Discord { get; }
        public ulong? AFKChannelId { get; private set; }
        public ulong? EmbedChannelId { get; private set; }
        public ulong OwnerId { get; private set; }
        public string VoiceRegionId { get; private set; }
        public ImmutableArray<Emoji> Emojis { get; protected set; }
        public ImmutableArray<string> Features { get; protected set; }

        public ulong DefaultChannelId => Id; 
        public string IconUrl => API.CDN.GetGuildIconUrl(Id, _iconId);
        public string SplashUrl => API.CDN.GetGuildSplashUrl(Id, _splashId);

        public Role EveryoneRole => GetRole(Id);
        public IReadOnlyCollection<IRole> Roles => _roles.ToReadOnlyCollection();

        public Guild(DiscordRestClient discord, Model model)
            : base(model.Id)
        {
            Discord = discord;

            Update(model, UpdateSource.Creation);
        }
        public void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            AFKChannelId = model.AFKChannelId;
            EmbedChannelId = model.EmbedChannelId;
            AFKTimeout = model.AFKTimeout;
            IsEmbeddable = model.EmbedEnabled;
            _iconId = model.Icon;
            Name = model.Name;
            OwnerId = model.OwnerId;
            VoiceRegionId = model.Region;
            _splashId = model.Splash;
            VerificationLevel = model.VerificationLevel;
            MfaLevel = model.MfaLevel;
            DefaultMessageNotifications = model.DefaultMessageNotifications;

            if (model.Emojis != null)
            {
                var emojis = ImmutableArray.CreateBuilder<Emoji>(model.Emojis.Length);
                for (int i = 0; i < model.Emojis.Length; i++)
                    emojis.Add(new Emoji(model.Emojis[i]));
                Emojis = emojis.ToImmutableArray();
            }
            else
                Emojis = ImmutableArray.Create<Emoji>();

            if (model.Features != null)
                Features = model.Features.ToImmutableArray();
            else
                Features = ImmutableArray.Create<string>();

            var roles = new ConcurrentDictionary<ulong, Role>(1, model.Roles?.Length ?? 0);
            if (model.Roles != null)
            {
                for (int i = 0; i < model.Roles.Length; i++)
                    roles[model.Roles[i].Id] = new Role(this, model.Roles[i]);
            }
            _roles = roles;
        }
        public void Update(EmbedModel model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            IsEmbeddable = model.Enabled;
            EmbedChannelId = model.ChannelId;
        }
        public void Update(IEnumerable<RoleModel> models, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            Role role;
            foreach (var model in models)
            {
                if (_roles.TryGetValue(model.Id, out role))
                    role.Update(model, UpdateSource.Rest);
            }
        }
        
        public async Task UpdateAsync()
        {
            if (IsAttached) throw new NotSupportedException();

            var response = await Discord.ApiClient.GetGuildAsync(Id).ConfigureAwait(false);
            Update(response, UpdateSource.Rest);
        }
        public async Task ModifyAsync(Action<ModifyGuildParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildParams();
            func(args);

            if (args._splash.IsSpecified && _splashId != null)
                args._splash = new API.Image(_splashId);
            if (args._icon.IsSpecified && _iconId != null)
                args._icon = new API.Image(_iconId);

            var model = await Discord.ApiClient.ModifyGuildAsync(Id, args).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task ModifyEmbedAsync(Action<ModifyGuildEmbedParams> func)
        { 
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildEmbedParams();
            func(args);
            var model = await Discord.ApiClient.ModifyGuildEmbedAsync(Id, args).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task ModifyChannelsAsync(IEnumerable<ModifyGuildChannelsParams> args)
        {
            await Discord.ApiClient.ModifyGuildChannelsAsync(Id, args).ConfigureAwait(false);
        }
        public async Task ModifyRolesAsync(IEnumerable<ModifyGuildRolesParams> args)
        {            
            var models = await Discord.ApiClient.ModifyGuildRolesAsync(Id, args).ConfigureAwait(false);
            Update(models, UpdateSource.Rest);
        }
        public async Task LeaveAsync()
        {
            await Discord.ApiClient.LeaveGuildAsync(Id).ConfigureAwait(false);
        }
        public async Task DeleteAsync()
        {
            await Discord.ApiClient.DeleteGuildAsync(Id).ConfigureAwait(false);
        }
        
        public async Task<IReadOnlyCollection<IUser>> GetBansAsync()
        {
            var models = await Discord.ApiClient.GetGuildBansAsync(Id).ConfigureAwait(false);
            return models.Select(x => new User(x)).ToImmutableArray();
        }
        public Task AddBanAsync(IUser user, int pruneDays = 0) => AddBanAsync(user, pruneDays);
        public async Task AddBanAsync(ulong userId, int pruneDays = 0)
        {
            var args = new CreateGuildBanParams() { DeleteMessageDays = pruneDays };
            await Discord.ApiClient.CreateGuildBanAsync(Id, userId, args).ConfigureAwait(false);
        }
        public Task RemoveBanAsync(IUser user) => RemoveBanAsync(user.Id);
        public async Task RemoveBanAsync(ulong userId)
        {
            await Discord.ApiClient.RemoveGuildBanAsync(Id, userId).ConfigureAwait(false);
        }
        
        public virtual async Task<IGuildChannel> GetChannelAsync(ulong id)
        {
            var model = await Discord.ApiClient.GetChannelAsync(Id, id).ConfigureAwait(false);
            if (model != null)
                return ToChannel(model);
            return null;
        }
        public virtual async Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync()
        {
            var models = await Discord.ApiClient.GetGuildChannelsAsync(Id).ConfigureAwait(false);
            return models.Select(x => ToChannel(x)).ToImmutableArray();
        }
        public async Task<ITextChannel> CreateTextChannelAsync(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var args = new CreateGuildChannelParams() { Name = name, Type = ChannelType.Text };
            var model = await Discord.ApiClient.CreateGuildChannelAsync(Id, args).ConfigureAwait(false);
            return new TextChannel(this, model);
        }
        public async Task<IVoiceChannel> CreateVoiceChannelAsync(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var args = new CreateGuildChannelParams { Name = name, Type = ChannelType.Voice };
            var model = await Discord.ApiClient.CreateGuildChannelAsync(Id, args).ConfigureAwait(false);
            return new VoiceChannel(this, model);
        }
        
        public async Task<IReadOnlyCollection<IGuildIntegration>> GetIntegrationsAsync()
        {
            var models = await Discord.ApiClient.GetGuildIntegrationsAsync(Id).ConfigureAwait(false);
            return models.Select(x => new GuildIntegration(this, x)).ToImmutableArray();
        }
        public async Task<IGuildIntegration> CreateIntegrationAsync(ulong id, string type)
        {
            var args = new CreateGuildIntegrationParams { Id = id, Type = type };
            var model = await Discord.ApiClient.CreateGuildIntegrationAsync(Id, args).ConfigureAwait(false);
            return new GuildIntegration(this, model);
        }
        
        public async Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync()
        {
            var models = await Discord.ApiClient.GetGuildInvitesAsync(Id).ConfigureAwait(false);
            return models.Select(x => new InviteMetadata(Discord, x)).ToImmutableArray();
        }
        public async Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 1800, int? maxUses = null, bool isTemporary = false, bool withXkcd = false)
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
            var model = await Discord.ApiClient.CreateChannelInviteAsync(DefaultChannelId, args).ConfigureAwait(false);
            return new InviteMetadata(Discord, model);
        }
        
        public Role GetRole(ulong id)
        {
            Role result = null;
            if (_roles?.TryGetValue(id, out result) == true)
                return result;
            return null;
        }        
        public async Task<IRole> CreateRoleAsync(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            
            var model = await Discord.ApiClient.CreateGuildRoleAsync(Id).ConfigureAwait(false);
            var role = new Role(this, model);

            await role.ModifyAsync(x =>
            {
                x.Name = name;
                x.Permissions = (permissions ?? role.Permissions).RawValue;
                x.Color = (color ?? Color.Default).RawValue;
                x.Hoist = isHoisted;
            }).ConfigureAwait(false);

            return role;
        }

        public virtual async Task<IGuildUser> GetUserAsync(ulong id)
        {
            var model = await Discord.ApiClient.GetGuildMemberAsync(Id, id).ConfigureAwait(false);
            if (model != null)
                return new GuildUser(this, new User(model.User), model);
            return null;
        }
        public virtual async Task<IGuildUser> GetCurrentUserAsync()
        {
            var currentUser = await Discord.GetCurrentUserAsync().ConfigureAwait(false);
            return await GetUserAsync(currentUser.Id).ConfigureAwait(false);
        }
        public virtual async Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync()
        {
            var args = new GetGuildMembersParams();
            var models = await Discord.ApiClient.GetGuildMembersAsync(Id, args).ConfigureAwait(false);
            return models.Select(x => new GuildUser(this, new User(x.User), x)).ToImmutableArray();
        }
        public async Task<int> PruneUsersAsync(int days = 30, bool simulate = false)
        {
            var args = new GuildPruneParams() { Days = days };
            GetGuildPruneCountResponse model;
            if (simulate)
                model = await Discord.ApiClient.GetGuildPruneCountAsync(Id, args).ConfigureAwait(false);
            else
                model = await Discord.ApiClient.BeginGuildPruneAsync(Id, args).ConfigureAwait(false);
            return model.Pruned;
        }
        public virtual Task DownloadUsersAsync()
        {
            throw new NotSupportedException();
        }

        internal GuildChannel ToChannel(API.Channel model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                    return new TextChannel(this, model);
                case ChannelType.Voice:
                    return new VoiceChannel(this, model);
                default:
                    throw new InvalidOperationException($"Unexpected channel type: {model.Type}");
            }
        }

        public override string ToString() => Name;

        private string DebuggerDisplay => $"{Name} ({Id})";

        bool IGuild.Available => false;
        IRole IGuild.EveryoneRole => EveryoneRole;
        IReadOnlyCollection<Emoji> IGuild.Emojis => Emojis;
        IReadOnlyCollection<string> IGuild.Features => Features;
        IAudioClient IGuild.AudioClient => null;

        IRole IGuild.GetRole(ulong id) => GetRole(id);
    }
}
