using Discord.API.Rest;
using Discord.Audio;
using Discord.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChannelModel = Discord.API.Channel;
using EmojiUpdateModel = Discord.API.Gateway.GuildEmojiUpdateEvent;
using ExtendedModel = Discord.API.Gateway.ExtendedGuild;
using GuildSyncModel = Discord.API.Gateway.GuildSyncEvent;
using MemberModel = Discord.API.GuildMember;
using Model = Discord.API.Guild;
using PresenceModel = Discord.API.Presence;
using RoleModel = Discord.API.Role;
using VoiceStateModel = Discord.API.VoiceState;

namespace Discord.WebSocket
{
    public class SocketGuild : SocketEntity<ulong>, IGuild
    {
        private ImmutableDictionary<ulong, RestRole> _roles;
        private ImmutableArray<Emoji> _emojis;
        private ImmutableArray<string> _features;
        internal bool _available;

        public string Name { get; private set; }
        public int AFKTimeout { get; private set; }
        public bool IsEmbeddable { get; private set; }
        public VerificationLevel VerificationLevel { get; private set; }
        public MfaLevel MfaLevel { get; private set; }
        public DefaultMessageNotifications DefaultMessageNotifications { get; private set; }

        public ulong? AFKChannelId { get; private set; }
        public ulong? EmbedChannelId { get; private set; }
        public ulong OwnerId { get; private set; }
        public string VoiceRegionId { get; private set; }
        public string IconId { get; private set; }
        public string SplashId { get; private set; }

        public ulong DefaultChannelId => Id;
        public string IconUrl => API.CDN.GetGuildIconUrl(Id, IconId);
        public string SplashUrl => API.CDN.GetGuildSplashUrl(Id, SplashId);
        public bool IsSynced => false;

        public RestRole EveryoneRole => GetRole(Id);
        public IReadOnlyCollection<RestRole> Roles => _roles.ToReadOnlyCollection();
        public IReadOnlyCollection<Emoji> Emojis => _emojis;
        public IReadOnlyCollection<string> Features => _features;

        internal SocketGuild(DiscordSocketClient client, ulong id)
            : base(client, id)
        {
        }
        internal static SocketGuild Create(DiscordSocketClient discord, Model model)
        {
            var entity = new SocketGuild(discord, model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            AFKChannelId = model.AFKChannelId;
            EmbedChannelId = model.EmbedChannelId;
            AFKTimeout = model.AFKTimeout;
            IsEmbeddable = model.EmbedEnabled;
            IconId = model.Icon;
            Name = model.Name;
            OwnerId = model.OwnerId;
            VoiceRegionId = model.Region;
            SplashId = model.Splash;
            VerificationLevel = model.VerificationLevel;
            MfaLevel = model.MfaLevel;
            DefaultMessageNotifications = model.DefaultMessageNotifications;

            if (model.Emojis != null)
            {
                var emojis = ImmutableArray.CreateBuilder<Emoji>(model.Emojis.Length);
                for (int i = 0; i < model.Emojis.Length; i++)
                    emojis.Add(Emoji.Create(model.Emojis[i]));
                _emojis = emojis.ToImmutableArray();
            }
            else
                _emojis = ImmutableArray.Create<Emoji>();

            if (model.Features != null)
                _features = model.Features.ToImmutableArray();
            else
                _features = ImmutableArray.Create<string>();

            var roles = ImmutableDictionary.CreateBuilder<ulong, RestRole>();
            if (model.Roles != null)
            {
                throw new NotImplementedException();
            }
            _roles = roles.ToImmutable();
        }

        //General
        public async Task UpdateAsync()
            => Update(await Discord.ApiClient.GetGuildAsync(Id));
        public Task DeleteAsync()
            => GuildHelper.DeleteAsync(this, Discord);

        public Task ModifyAsync(Action<ModifyGuildParams> func)
            => GuildHelper.ModifyAsync(this, Discord, func);
        public Task ModifyEmbedAsync(Action<ModifyGuildEmbedParams> func)
            => GuildHelper.ModifyEmbedAsync(this, Discord, func);
        public Task ModifyChannelsAsync(IEnumerable<ModifyGuildChannelsParams> args)
            => GuildHelper.ModifyChannelsAsync(this, Discord, args);
        public Task ModifyRolesAsync(IEnumerable<ModifyGuildRolesParams> args)
            => GuildHelper.ModifyRolesAsync(this, Discord, args);

        public Task LeaveAsync()
            => GuildHelper.LeaveAsync(this, Discord);

        //Bans
        public Task<IReadOnlyCollection<RestBan>> GetBansAsync()
            => GuildHelper.GetBansAsync(this, Discord);

        public Task AddBanAsync(IUser user, int pruneDays = 0)
            => GuildHelper.AddBanAsync(this, Discord, user.Id, pruneDays);
        public Task AddBanAsync(ulong userId, int pruneDays = 0)
            => GuildHelper.AddBanAsync(this, Discord, userId, pruneDays);

        public Task RemoveBanAsync(IUser user)
            => GuildHelper.RemoveBanAsync(this, Discord, user.Id);
        public Task RemoveBanAsync(ulong userId)
            => GuildHelper.RemoveBanAsync(this, Discord, userId);

        //Channels
        public Task<IReadOnlyCollection<RestGuildChannel>> GetChannelsAsync()
            => GuildHelper.GetChannelsAsync(this, Discord);
        public Task<RestGuildChannel> GetChannelAsync(ulong id)
            => GuildHelper.GetChannelAsync(this, Discord, id);
        public Task<RestTextChannel> CreateTextChannelAsync(string name)
            => GuildHelper.CreateTextChannelAsync(this, Discord, name);
        public Task<RestVoiceChannel> CreateVoiceChannelAsync(string name)
            => GuildHelper.CreateVoiceChannelAsync(this, Discord, name);

        //Integrations
        public Task<IReadOnlyCollection<RestGuildIntegration>> GetIntegrationsAsync()
            => GuildHelper.GetIntegrationsAsync(this, Discord);
        public Task<RestGuildIntegration> CreateIntegrationAsync(ulong id, string type)
            => GuildHelper.CreateIntegrationAsync(this, Discord, id, type);

        //Invites
        public Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync()
            => GuildHelper.GetInvitesAsync(this, Discord);

        //Roles
        public RestRole GetRole(ulong id)
        {
            RestRole value;
            if (_roles.TryGetValue(id, out value))
                return value;
            return null;
        }

        public async Task<IRole> CreateRoleAsync(string name, GuildPermissions? permissions = default(GuildPermissions?), Color? color = default(Color?), bool isHoisted = false)
        {
            var role = await GuildHelper.CreateRoleAsync(this, Discord, name, permissions, color, isHoisted);
            _roles = _roles.Add(role.Id, role);
            return role;
        }

        //Users
        public Task<IReadOnlyCollection<RestGuildUser>> GetUsersAsync()
            => GuildHelper.GetUsersAsync(this, Discord);
        public Task<RestGuildUser> GetUserAsync(ulong id)
            => GuildHelper.GetUserAsync(this, Discord, id);
        public Task<RestGuildUser> GetCurrentUserAsync()
            => GuildHelper.GetUserAsync(this, Discord, Discord.CurrentUser.Id);

        public Task<int> PruneUsersAsync(int days = 30, bool simulate = false)
            => GuildHelper.PruneUsersAsync(this, Discord, days, simulate);

        //IGuild
        bool IGuild.Available => true;
        IAudioClient IGuild.AudioClient => null;
        IReadOnlyCollection<IGuildUser> IGuild.CachedUsers => ImmutableArray.Create<IGuildUser>();
        IRole IGuild.EveryoneRole => EveryoneRole;
        IReadOnlyCollection<IRole> IGuild.Roles => Roles;

        async Task<IReadOnlyCollection<IBan>> IGuild.GetBansAsync()
            => await GetBansAsync();

        async Task<IReadOnlyCollection<IGuildChannel>> IGuild.GetChannelsAsync()
            => await GetChannelsAsync();
        async Task<IGuildChannel> IGuild.GetChannelAsync(ulong id)
            => await GetChannelAsync(id);
        IGuildChannel IGuild.GetCachedChannel(ulong id)
            => null;
        async Task<ITextChannel> IGuild.CreateTextChannelAsync(string name)
            => await CreateTextChannelAsync(name);
        async Task<IVoiceChannel> IGuild.CreateVoiceChannelAsync(string name)
            => await CreateVoiceChannelAsync(name);

        async Task<IReadOnlyCollection<IGuildIntegration>> IGuild.GetIntegrationsAsync()
            => await GetIntegrationsAsync();
        async Task<IGuildIntegration> IGuild.CreateIntegrationAsync(ulong id, string type)
            => await CreateIntegrationAsync(id, type);

        async Task<IReadOnlyCollection<IInviteMetadata>> IGuild.GetInvitesAsync()
            => await GetInvitesAsync();

        IRole IGuild.GetRole(ulong id)
            => GetRole(id);

        async Task<IReadOnlyCollection<IGuildUser>> IGuild.GetUsersAsync()
            => await GetUsersAsync();
        async Task<IGuildUser> IGuild.GetUserAsync(ulong id)
            => await GetUserAsync(id);
        IGuildUser IGuild.GetCachedUser(ulong id)
            => null;
        async Task<IGuildUser> IGuild.GetCurrentUserAsync()
            => await GetCurrentUserAsync();
        Task IGuild.DownloadUsersAsync() { throw new NotSupportedException(); }
    }
}
