using Discord.API.Rest;
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
        public int VerificationLevel { get; private set; }

        public ulong? AFKChannelId { get; private set; }
        public ulong? EmbedChannelId { get; private set; }
        public ulong OwnerId { get; private set; }
        public string VoiceRegionId { get; private set; }
        public override DiscordClient Discord { get; }
        public ImmutableArray<Emoji> Emojis { get; protected set; }
        public ImmutableArray<string> Features { get; protected set; }

        public ulong DefaultChannelId => Id; 
        public string IconUrl => API.CDN.GetGuildIconUrl(Id, _iconId);
        public string SplashUrl => API.CDN.GetGuildSplashUrl(Id, _splashId);

        public Role EveryoneRole => GetRole(Id);
        public IReadOnlyCollection<IRole> Roles => _roles.ToReadOnlyCollection();

        public Guild(DiscordClient discord, Model model)
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
        
        public async Task Update()
        {
            if (IsAttached) throw new NotSupportedException();

            var response = await Discord.ApiClient.GetGuild(Id).ConfigureAwait(false);
            Update(response, UpdateSource.Rest);
        }
        public async Task Modify(Action<ModifyGuildParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildParams();
            func(args);
            var model = await Discord.ApiClient.ModifyGuild(Id, args).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task ModifyEmbed(Action<ModifyGuildEmbedParams> func)
        { 
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildEmbedParams();
            func(args);
            var model = await Discord.ApiClient.ModifyGuildEmbed(Id, args).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task ModifyChannels(IEnumerable<ModifyGuildChannelsParams> args)
        {
            //TODO: Update channels
            await Discord.ApiClient.ModifyGuildChannels(Id, args).ConfigureAwait(false);
        }
        public async Task ModifyRoles(IEnumerable<ModifyGuildRolesParams> args)
        {            
            var models = await Discord.ApiClient.ModifyGuildRoles(Id, args).ConfigureAwait(false);
            Update(models, UpdateSource.Rest);
        }
        public async Task Leave()
        {
            await Discord.ApiClient.LeaveGuild(Id).ConfigureAwait(false);
        }
        public async Task Delete()
        {
            await Discord.ApiClient.DeleteGuild(Id).ConfigureAwait(false);
        }
        
        public async Task<IReadOnlyCollection<IUser>> GetBans()
        {
            var models = await Discord.ApiClient.GetGuildBans(Id).ConfigureAwait(false);
            return models.Select(x => new User(Discord, x)).ToImmutableArray();
        }
        public Task AddBan(IUser user, int pruneDays = 0) => AddBan(user, pruneDays);
        public async Task AddBan(ulong userId, int pruneDays = 0)
        {
            var args = new CreateGuildBanParams() { PruneDays = pruneDays };
            await Discord.ApiClient.CreateGuildBan(Id, userId, args).ConfigureAwait(false);
        }
        public Task RemoveBan(IUser user) => RemoveBan(user.Id);
        public async Task RemoveBan(ulong userId)
        {
            await Discord.ApiClient.RemoveGuildBan(Id, userId).ConfigureAwait(false);
        }
        
        public virtual async Task<IGuildChannel> GetChannel(ulong id)
        {
            var model = await Discord.ApiClient.GetChannel(Id, id).ConfigureAwait(false);
            if (model != null)
                return ToChannel(model);
            return null;
        }
        public virtual async Task<IReadOnlyCollection<IGuildChannel>> GetChannels()
        {
            var models = await Discord.ApiClient.GetGuildChannels(Id).ConfigureAwait(false);
            return models.Select(x => ToChannel(x)).ToImmutableArray();
        }
        public async Task<ITextChannel> CreateTextChannel(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var args = new CreateGuildChannelParams() { Name = name, Type = ChannelType.Text };
            var model = await Discord.ApiClient.CreateGuildChannel(Id, args).ConfigureAwait(false);
            return new TextChannel(this, model);
        }
        public async Task<IVoiceChannel> CreateVoiceChannel(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var args = new CreateGuildChannelParams { Name = name, Type = ChannelType.Voice };
            var model = await Discord.ApiClient.CreateGuildChannel(Id, args).ConfigureAwait(false);
            return new VoiceChannel(this, model);
        }
        
        public async Task<IReadOnlyCollection<IGuildIntegration>> GetIntegrations()
        {
            var models = await Discord.ApiClient.GetGuildIntegrations(Id).ConfigureAwait(false);
            return models.Select(x => new GuildIntegration(this, x)).ToImmutableArray();
        }
        public async Task<IGuildIntegration> CreateIntegration(ulong id, string type)
        {
            var args = new CreateGuildIntegrationParams { Id = id, Type = type };
            var model = await Discord.ApiClient.CreateGuildIntegration(Id, args).ConfigureAwait(false);
            return new GuildIntegration(this, model);
        }
        
        public async Task<IReadOnlyCollection<IInviteMetadata>> GetInvites()
        {
            var models = await Discord.ApiClient.GetGuildInvites(Id).ConfigureAwait(false);
            return models.Select(x => new InviteMetadata(Discord, x)).ToImmutableArray();
        }
        public async Task<IInviteMetadata> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool isTemporary = false, bool withXkcd = false)
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
            var model = await Discord.ApiClient.CreateChannelInvite(DefaultChannelId, args).ConfigureAwait(false);
            return new InviteMetadata(Discord, model);
        }
        
        public Role GetRole(ulong id)
        {
            Role result = null;
            if (_roles?.TryGetValue(id, out result) == true)
                return result;
            return null;
        }        
        public async Task<IRole> CreateRole(string name, GuildPermissions? permissions = null, Color? color = null, bool isHoisted = false)
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

        public virtual async Task<IGuildUser> GetUser(ulong id)
        {
            var model = await Discord.ApiClient.GetGuildMember(Id, id).ConfigureAwait(false);
            if (model != null)
                return new GuildUser(this, new User(Discord, model.User), model);
            return null;
        }
        public virtual async Task<IGuildUser> GetCurrentUser()
        {
            var currentUser = await Discord.GetCurrentUser().ConfigureAwait(false);
            return await GetUser(currentUser.Id).ConfigureAwait(false);
        }
        public virtual async Task<IReadOnlyCollection<IGuildUser>> GetUsers()
        {
            var args = new GetGuildMembersParams();
            var models = await Discord.ApiClient.GetGuildMembers(Id, args).ConfigureAwait(false);
            return models.Select(x => new GuildUser(this, new User(Discord, x.User), x)).ToImmutableArray();
        }
        public virtual async Task<IReadOnlyCollection<IGuildUser>> GetUsers(int limit, int offset)
        {
            var args = new GetGuildMembersParams { Limit = limit, Offset = offset };
            var models = await Discord.ApiClient.GetGuildMembers(Id, args).ConfigureAwait(false);
            return models.Select(x => new GuildUser(this, new User(Discord, x.User), x)).ToImmutableArray();
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
                    return new TextChannel(this, model);
                case ChannelType.Voice:
                    return new VoiceChannel(this, model);
                default:
                    throw new InvalidOperationException($"Unknown channel type: {model.Type}");
            }
        }

        public override string ToString() => Name;

        private string DebuggerDisplay => $"{Name} ({Id})";

        IRole IGuild.EveryoneRole => EveryoneRole;
        IReadOnlyCollection<Emoji> IGuild.Emojis => Emojis;
        IReadOnlyCollection<string> IGuild.Features => Features;

        IRole IGuild.GetRole(ulong id) => GetRole(id);
    }
}
