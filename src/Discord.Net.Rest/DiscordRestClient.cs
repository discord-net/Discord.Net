using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

namespace Discord.Rest
{
    public class DiscordRestClient : BaseDiscordClient, IDiscordClient
    {
        private RestApplication _applicationInfo;

        public new RestSelfUser CurrentUser => base.CurrentUser as RestSelfUser;

        public DiscordRestClient() : this(new DiscordRestConfig()) { }
        public DiscordRestClient(DiscordRestConfig config) : base(config, CreateApiClient(config)) { }

        private static API.DiscordRestApiClient CreateApiClient(DiscordRestConfig config)
            => new API.DiscordRestApiClient(config.RestClientProvider, DiscordRestConfig.UserAgent);

        protected override async Task OnLoginAsync(TokenType tokenType, string token)
        {
            var user = await ApiClient.GetMyUserAsync(new RequestOptions { RetryMode = RetryMode.AlwaysRetry }).ConfigureAwait(false);
            ApiClient.CurrentUserId = user.Id;
            base.CurrentUser = RestSelfUser.Create(this, user);
        }
        protected override Task OnLogoutAsync()
        {
            _applicationInfo = null;
            return Task.Delay(0);
        }

        /// <inheritdoc />
        public async Task<RestApplication> GetApplicationInfoAsync()
        {
            return _applicationInfo ?? (_applicationInfo = await ClientHelper.GetApplicationInfoAsync(this));
        }
        
        /// <inheritdoc />
        public Task<RestChannel> GetChannelAsync(ulong id)
            => ClientHelper.GetChannelAsync(this, id);
        /// <inheritdoc />
        public Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync()
            => ClientHelper.GetPrivateChannelsAsync(this);

        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestConnection>> GetConnectionsAsync()
            => ClientHelper.GetConnectionsAsync(this);

        /// <inheritdoc />
        public Task<RestInvite> GetInviteAsync(string inviteId)
            => ClientHelper.GetInviteAsync(this, inviteId);

        /// <inheritdoc />
        public Task<RestGuild> GetGuildAsync(ulong id)
            => ClientHelper.GetGuildAsync(this, id);
        /// <inheritdoc />
        public Task<RestGuildEmbed?> GetGuildEmbedAsync(ulong id)
            => ClientHelper.GetGuildEmbedAsync(this, id);
        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestUserGuild>> GetGuildSummariesAsync()
            => ClientHelper.GetGuildSummariesAsync(this);
        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync()
            => ClientHelper.GetGuildsAsync(this);
        /// <inheritdoc />
        public Task<RestGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null)
            => ClientHelper.CreateGuildAsync(this, name, region, jpegIcon);

        /// <inheritdoc />
        public Task<RestUser> GetUserAsync(ulong id)
            => ClientHelper.GetUserAsync(this, id);
        /// <inheritdoc />
        public Task<RestGuildUser> GetGuildUserAsync(ulong guildId, ulong id)
            => ClientHelper.GetGuildUserAsync(this, guildId, id);

        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync()
            => ClientHelper.GetVoiceRegionsAsync(this);
        /// <inheritdoc />
        public Task<RestVoiceRegion> GetVoiceRegionAsync(string id)
            => ClientHelper.GetVoiceRegionAsync(this, id);

        //IDiscordClient
        async Task<IApplication> IDiscordClient.GetApplicationInfoAsync()
            => await GetApplicationInfoAsync().ConfigureAwait(false);

        async Task<IChannel> IDiscordClient.GetChannelAsync(ulong id, CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetChannelAsync(id).ConfigureAwait(false);
            else
                return null;
        }
        async Task<IReadOnlyCollection<IPrivateChannel>> IDiscordClient.GetPrivateChannelsAsync(CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetPrivateChannelsAsync().ConfigureAwait(false);
            else
                return ImmutableArray.Create<IPrivateChannel>();
        }

        async Task<IReadOnlyCollection<IConnection>> IDiscordClient.GetConnectionsAsync()
            => await GetConnectionsAsync().ConfigureAwait(false);

        async Task<IInvite> IDiscordClient.GetInviteAsync(string inviteId)
            => await GetInviteAsync(inviteId).ConfigureAwait(false);

        async Task<IGuild> IDiscordClient.GetGuildAsync(ulong id, CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetGuildAsync(id).ConfigureAwait(false);
            else
                return null;
        }
        async Task<IReadOnlyCollection<IGuild>> IDiscordClient.GetGuildsAsync(CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetGuildsAsync().ConfigureAwait(false);
            else
                return ImmutableArray.Create<IGuild>();
        }
        async Task<IGuild> IDiscordClient.CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon)
            => await CreateGuildAsync(name, region, jpegIcon).ConfigureAwait(false);

        async Task<IUser> IDiscordClient.GetUserAsync(ulong id, CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetUserAsync(id).ConfigureAwait(false);
            else
                return null;
        }

        async Task<IReadOnlyCollection<IVoiceRegion>> IDiscordClient.GetVoiceRegionsAsync()
            => await GetVoiceRegionsAsync().ConfigureAwait(false);
        async Task<IVoiceRegion> IDiscordClient.GetVoiceRegionAsync(string id)
            => await GetVoiceRegionAsync(id).ConfigureAwait(false);
    }
}
