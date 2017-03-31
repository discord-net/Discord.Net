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
        internal override void Dispose(bool disposing)
        {
            if (disposing)
                ApiClient.Dispose();
        }

        internal override async Task OnLoginAsync(TokenType tokenType, string token)
        {
            var user = await ApiClient.GetMyUserAsync(new RequestOptions { RetryMode = RetryMode.AlwaysRetry }).ConfigureAwait(false);
            ApiClient.CurrentUserId = user.Id;
            base.CurrentUser = RestSelfUser.Create(this, user);
        }
        internal override Task OnLogoutAsync()
        {
            _applicationInfo = null;
            return Task.Delay(0);
        }

        /// <inheritdoc />
        public async Task<RestApplication> GetApplicationInfoAsync(RequestOptions options = null)
        {
            return _applicationInfo ?? (_applicationInfo = await ClientHelper.GetApplicationInfoAsync(this, options));
        }
        
        /// <inheritdoc />
        public Task<RestChannel> GetChannelAsync(ulong id, RequestOptions options = null)
            => ClientHelper.GetChannelAsync(this, id, options);
        /// <inheritdoc />
        public Task<IReadOnlyCollection<IRestPrivateChannel>> GetPrivateChannelsAsync(RequestOptions options = null)
            => ClientHelper.GetPrivateChannelsAsync(this, options);
        public Task<IReadOnlyCollection<RestDMChannel>> GetDMChannelsAsync(RequestOptions options = null)
            => ClientHelper.GetDMChannelsAsync(this, options);
        public Task<IReadOnlyCollection<RestGroupChannel>> GetGroupChannelsAsync(RequestOptions options = null)
            => ClientHelper.GetGroupChannelsAsync(this, options);

        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestConnection>> GetConnectionsAsync(RequestOptions options = null)
            => ClientHelper.GetConnectionsAsync(this, options);

        /// <inheritdoc />
        public Task<RestInvite> GetInviteAsync(string inviteId, RequestOptions options = null)
            => ClientHelper.GetInviteAsync(this, inviteId, options);

        /// <inheritdoc />
        public Task<RestGuild> GetGuildAsync(ulong id, RequestOptions options = null)
            => ClientHelper.GetGuildAsync(this, id, options);
        /// <inheritdoc />
        public Task<RestGuildEmbed?> GetGuildEmbedAsync(ulong id, RequestOptions options = null)
            => ClientHelper.GetGuildEmbedAsync(this, id, options);
        /// <inheritdoc />
        public IAsyncEnumerable<IReadOnlyCollection<RestUserGuild>> GetGuildSummariesAsync(RequestOptions options = null)
            => ClientHelper.GetGuildSummariesAsync(this, null, null, options);
        /// <inheritdoc />
        public IAsyncEnumerable<IReadOnlyCollection<RestUserGuild>> GetGuildSummariesAsync(ulong fromGuildId, int limit, RequestOptions options = null)
            => ClientHelper.GetGuildSummariesAsync(this, fromGuildId, limit, options);
        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync(RequestOptions options = null)
            => ClientHelper.GetGuildsAsync(this, options);
        /// <inheritdoc />
        public Task<RestGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null, RequestOptions options = null)
            => ClientHelper.CreateGuildAsync(this, name, region, jpegIcon, options);

        /// <inheritdoc />
        public Task<RestUser> GetUserAsync(ulong id, RequestOptions options = null)
            => ClientHelper.GetUserAsync(this, id, options);
        /// <inheritdoc />
        public Task<RestGuildUser> GetGuildUserAsync(ulong guildId, ulong id, RequestOptions options = null)
            => ClientHelper.GetGuildUserAsync(this, guildId, id, options);

        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null)
            => ClientHelper.GetVoiceRegionsAsync(this, options);
        /// <inheritdoc />
        public Task<RestVoiceRegion> GetVoiceRegionAsync(string id, RequestOptions options = null)
            => ClientHelper.GetVoiceRegionAsync(this, id, options);

        //IDiscordClient
        async Task<IApplication> IDiscordClient.GetApplicationInfoAsync(RequestOptions options)
            => await GetApplicationInfoAsync(options).ConfigureAwait(false);

        async Task<IChannel> IDiscordClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetChannelAsync(id, options).ConfigureAwait(false);
            else
                return null;
        }
        async Task<IReadOnlyCollection<IPrivateChannel>> IDiscordClient.GetPrivateChannelsAsync(CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetPrivateChannelsAsync(options).ConfigureAwait(false);
            else
                return ImmutableArray.Create<IPrivateChannel>();
        }
        async Task<IReadOnlyCollection<IDMChannel>> IDiscordClient.GetDMChannelsAsync(CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetDMChannelsAsync(options).ConfigureAwait(false);
            else
                return ImmutableArray.Create<IDMChannel>();
        }
        async Task<IReadOnlyCollection<IGroupChannel>> IDiscordClient.GetGroupChannelsAsync(CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetGroupChannelsAsync(options).ConfigureAwait(false);
            else
                return ImmutableArray.Create<IGroupChannel>();
        }

        async Task<IReadOnlyCollection<IConnection>> IDiscordClient.GetConnectionsAsync(RequestOptions options)
            => await GetConnectionsAsync(options).ConfigureAwait(false);

        async Task<IInvite> IDiscordClient.GetInviteAsync(string inviteId, RequestOptions options)
            => await GetInviteAsync(inviteId, options).ConfigureAwait(false);

        async Task<IGuild> IDiscordClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetGuildAsync(id, options).ConfigureAwait(false);
            else
                return null;
        }
        async Task<IReadOnlyCollection<IGuild>> IDiscordClient.GetGuildsAsync(CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetGuildsAsync(options).ConfigureAwait(false);
            else
                return ImmutableArray.Create<IGuild>();
        }
        async Task<IGuild> IDiscordClient.CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon, RequestOptions options)
            => await CreateGuildAsync(name, region, jpegIcon, options).ConfigureAwait(false);

        async Task<IUser> IDiscordClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetUserAsync(id, options).ConfigureAwait(false);
            else
                return null;
        }

        async Task<IReadOnlyCollection<IVoiceRegion>> IDiscordClient.GetVoiceRegionsAsync(RequestOptions options)
            => await GetVoiceRegionsAsync(options).ConfigureAwait(false);
        async Task<IVoiceRegion> IDiscordClient.GetVoiceRegionAsync(string id, RequestOptions options)
            => await GetVoiceRegionAsync(id, options).ConfigureAwait(false);
    }
}
