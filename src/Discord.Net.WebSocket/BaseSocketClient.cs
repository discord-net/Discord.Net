using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord.API;
using Discord.Rest;

namespace Discord.WebSocket
{
    public abstract partial class BaseSocketClient : BaseDiscordClient, IDiscordClient
    {
        protected readonly DiscordSocketConfig BaseConfig;

        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the gateway server. </summary>
        public abstract int Latency { get; protected set; }
        public abstract UserStatus Status { get; protected set; } 
        public abstract IActivity Activity { get; protected set; }

        internal new DiscordSocketApiClient ApiClient => base.ApiClient as DiscordSocketApiClient;

        public new SocketSelfUser CurrentUser { get => base.CurrentUser as SocketSelfUser; protected set => base.CurrentUser = value; }
        public abstract IReadOnlyCollection<SocketGuild> Guilds { get; }
        public abstract IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels { get; }
        public abstract IReadOnlyCollection<RestVoiceRegion> VoiceRegions { get; }

        internal BaseSocketClient(DiscordSocketConfig config, DiscordRestApiClient client)
            : base(config, client) => BaseConfig = config;
        private static DiscordSocketApiClient CreateApiClient(DiscordSocketConfig config)
            => new DiscordSocketApiClient(config.RestClientProvider, config.WebSocketProvider, DiscordRestConfig.UserAgent);
        
        public abstract Task<RestApplication> GetApplicationInfoAsync(RequestOptions options = null);
        public abstract SocketUser GetUser(ulong id);
        public abstract SocketUser GetUser(string username, string discriminator);
        public abstract SocketChannel GetChannel(ulong id);
        public abstract SocketGuild GetGuild(ulong id);
        public abstract RestVoiceRegion GetVoiceRegion(string id);
        /// <inheritdoc />
        public abstract Task StartAsync();
        /// <inheritdoc />
        public abstract Task StopAsync();
        public abstract Task SetStatusAsync(UserStatus status);
        public abstract Task SetGameAsync(string name, string streamUrl = null, ActivityType type = ActivityType.Playing);
        public abstract Task SetActivityAsync(IActivity activity);
        public abstract Task DownloadUsersAsync(IEnumerable<IGuild> guilds);  
        
        public Task<RestGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null, RequestOptions options = null)
            => ClientHelper.CreateGuildAsync(this, name, region, jpegIcon, options ?? RequestOptions.Default);
        public Task<IReadOnlyCollection<RestConnection>> GetConnectionsAsync(RequestOptions options = null)
            => ClientHelper.GetConnectionsAsync(this, options ?? RequestOptions.Default);
        public Task<RestInvite> GetInviteAsync(string inviteId, RequestOptions options = null)
            => ClientHelper.GetInviteAsync(this, inviteId, options ?? RequestOptions.Default);
        
        // IDiscordClient
        /// <inheritdoc />
        async Task<IApplication> IDiscordClient.GetApplicationInfoAsync(RequestOptions options)
            => await GetApplicationInfoAsync(options).ConfigureAwait(false);

        /// <inheritdoc />
        Task<IChannel> IDiscordClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IChannel>(GetChannel(id));
        /// <inheritdoc />
        Task<IReadOnlyCollection<IPrivateChannel>> IDiscordClient.GetPrivateChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IPrivateChannel>>(PrivateChannels);

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IConnection>> IDiscordClient.GetConnectionsAsync(RequestOptions options)
            => await GetConnectionsAsync(options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IInvite> IDiscordClient.GetInviteAsync(string inviteId, RequestOptions options)
            => await GetInviteAsync(inviteId, options).ConfigureAwait(false);

        /// <inheritdoc />
        Task<IGuild> IDiscordClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuild>(GetGuild(id));
        /// <inheritdoc />
        Task<IReadOnlyCollection<IGuild>> IDiscordClient.GetGuildsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);

        /// <inheritdoc />
        async Task<IGuild> IDiscordClient.CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon, RequestOptions options)
            => await CreateGuildAsync(name, region, jpegIcon, options).ConfigureAwait(false);

        /// <inheritdoc />
        Task<IUser> IDiscordClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(id));
        /// <inheritdoc />
        Task<IUser> IDiscordClient.GetUserAsync(string username, string discriminator, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(username, discriminator));

        /// <inheritdoc />
        Task<IVoiceRegion> IDiscordClient.GetVoiceRegionAsync(string id, RequestOptions options)
            => Task.FromResult<IVoiceRegion>(GetVoiceRegion(id));
        /// <inheritdoc />
        Task<IReadOnlyCollection<IVoiceRegion>> IDiscordClient.GetVoiceRegionsAsync(RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IVoiceRegion>>(VoiceRegions);
    }
}
