using Discord.Net.Queue;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord.Rest
{
    public class DiscordRestClient : DiscordClient, IDiscordClient
    {
        public new RestSelfUser CurrentUser => base.CurrentUser as RestSelfUser;

        public DiscordRestClient() : this(new DiscordRestConfig()) { }
        public DiscordRestClient(DiscordRestConfig config) : base(config, CreateApiClient(config)) { }

        private static API.DiscordRestApiClient CreateApiClient(DiscordRestConfig config)
            => new API.DiscordRestApiClient(config.RestClientProvider, DiscordRestConfig.UserAgent, requestQueue: new RequestQueue());

        protected override async Task OnLoginAsync(TokenType tokenType, string token)
        {
            await ApiClient.LoginAsync(tokenType, token).ConfigureAwait(false);
            base.CurrentUser = RestSelfUser.Create(this, ApiClient.CurrentUser);
        }

        /// <inheritdoc />
        public Task<RestApplication> GetApplicationInfoAsync()
            => ClientHelper.GetApplicationInfoAsync(this);
        
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
        public Task<RestUser> GetUserAsync(string username, string discriminator)
            => ClientHelper.GetUserAsync(this, username, discriminator);

        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync()
            => ClientHelper.GetVoiceRegionsAsync(this);
        /// <inheritdoc />
        public Task<RestVoiceRegion> GetVoiceRegionAsync(string id)
            => ClientHelper.GetVoiceRegionAsync(this, id);

        //IDiscordClient
        async Task<IApplication> IDiscordClient.GetApplicationInfoAsync()
            => await GetApplicationInfoAsync().ConfigureAwait(false);

        async Task<IChannel> IDiscordClient.GetChannelAsync(ulong id)
            => await GetChannelAsync(id);
        async Task<IReadOnlyCollection<IPrivateChannel>> IDiscordClient.GetPrivateChannelsAsync()
            => await GetPrivateChannelsAsync();

        async Task<IReadOnlyCollection<IConnection>> IDiscordClient.GetConnectionsAsync()
            => await GetConnectionsAsync().ConfigureAwait(false);

        async Task<IInvite> IDiscordClient.GetInviteAsync(string inviteId)
            => await GetInviteAsync(inviteId).ConfigureAwait(false);

        async Task<IGuild> IDiscordClient.GetGuildAsync(ulong id)
            => await GetGuildAsync(id).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IGuild>> IDiscordClient.GetGuildsAsync()
            => await GetGuildsAsync().ConfigureAwait(false);
        async Task<IGuild> IDiscordClient.CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon)
            => await CreateGuildAsync(name, region, jpegIcon).ConfigureAwait(false);

        async Task<IUser> IDiscordClient.GetUserAsync(ulong id)
            => await GetUserAsync(id).ConfigureAwait(false);
        async Task<IUser> IDiscordClient.GetUserAsync(string username, string discriminator)
            => await GetUserAsync(username, discriminator).ConfigureAwait(false);

        async Task<IReadOnlyCollection<IVoiceRegion>> IDiscordClient.GetVoiceRegionsAsync()
            => await GetVoiceRegionsAsync().ConfigureAwait(false);
        async Task<IVoiceRegion> IDiscordClient.GetVoiceRegionAsync(string id)
            => await GetVoiceRegionAsync(id).ConfigureAwait(false);
    }
}
