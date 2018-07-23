using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.API;
using Discord.Rest;

namespace Discord.WebSocket
{
    public abstract partial class BaseSocketClient : BaseDiscordClient, IDiscordClient
    {
        protected readonly DiscordSocketConfig _baseconfig;

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
            : base(config, client) => _baseconfig = config;
        private static DiscordSocketApiClient CreateApiClient(DiscordSocketConfig config)
            => new DiscordSocketApiClient(config.RestClientProvider, config.WebSocketProvider, DiscordRestConfig.UserAgent);

        /// <inheritdoc />
        public abstract Task<RestApplication> GetApplicationInfoAsync(RequestOptions options = null);
        /// <inheritdoc />
        public abstract SocketUser GetUser(ulong id);
        /// <inheritdoc />
        public abstract SocketUser GetUser(string username, string discriminator);
        /// <inheritdoc />
        public abstract SocketChannel GetChannel(ulong id);
        /// <inheritdoc />
        public abstract SocketGuild GetGuild(ulong id);
        /// <inheritdoc />
        public abstract RestVoiceRegion GetVoiceRegion(string id);
        /// <inheritdoc />
        public abstract Task StartAsync();
        /// <inheritdoc />
        public abstract Task StopAsync();
        public abstract Task SetStatusAsync(UserStatus status);
        public abstract Task SetGameAsync(string name, string streamUrl = null, ActivityType type = ActivityType.Playing);
        public abstract Task SetActivityAsync(IActivity activity);
        public abstract Task DownloadUsersAsync(IEnumerable<IGuild> guilds);

        public Task<IChannel> GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            Func<Task<IChannel>> restAction = async () =>
                await ClientHelper.GetChannelAsync(this, id, options).ConfigureAwait(false) as IChannel;
            Func<IChannel> cacheAction = () => GetChannel(id);

            return CacheHelper(mode, restAction, cacheAction);
        }
        public Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync(CacheMode mode, RequestOptions options)
        {
            Func<Task<IReadOnlyCollection<IPrivateChannel>>> restAction = async () =>
            {
                var col = (await ClientHelper.GetPrivateChannelsAsync(this, options).ConfigureAwait(false)).OfType<IPrivateChannel>();
                return col.ToReadOnlyCollection(col.Count);
            };
            Func<IReadOnlyCollection<IPrivateChannel>> cacheAction = () => PrivateChannels;

            return CacheHelper(mode, restAction, cacheAction);
        }

        public Task<IGuild> GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            Func<Task<IGuild>> restAction = async () =>
                await ClientHelper.GetGuildAsync(this, id, options).ConfigureAwait(false) as IGuild;
            Func<IGuild> cacheAction = () => GetGuild(id);

            return CacheHelper(mode, restAction, cacheAction);
        }
        public Task<IReadOnlyCollection<IGuild>> GetGuildsAsync(CacheMode mode, RequestOptions options)
        {
            Func<Task<IReadOnlyCollection<IGuild>>> restAction = async () =>
            {
                var col = (await ClientHelper.GetGuildsAsync(this, options)).OfType<IGuild>();
                return col.ToReadOnlyCollection(col.Count);
            };
            Func<IReadOnlyCollection<IGuild>> cacheAction = () => Guilds;

            return CacheHelper(mode, restAction, cacheAction);
        }

        public Task<IUser> GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            Func<Task<IUser>> restAction = async () =>
                await ClientHelper.GetUserAsync(this, id, options).ConfigureAwait(false) as IUser;
            Func<IUser> cacheAction = () => GetUser(id);

            return CacheHelper(mode, restAction, cacheAction);
        }

        /// <inheritdoc />
        public Task<RestGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null, RequestOptions options = null)
            => ClientHelper.CreateGuildAsync(this, name, region, jpegIcon, options ?? RequestOptions.Default);
        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestConnection>> GetConnectionsAsync(RequestOptions options = null)
            => ClientHelper.GetConnectionsAsync(this, options ?? RequestOptions.Default);
        /// <inheritdoc />
        public Task<RestInviteMetadata> GetInviteAsync(string inviteId, RequestOptions options = null)
            => ClientHelper.GetInviteAsync(this, inviteId, options ?? RequestOptions.Default);
        
        // IDiscordClient
        async Task<IApplication> IDiscordClient.GetApplicationInfoAsync(RequestOptions options)
            => await GetApplicationInfoAsync(options).ConfigureAwait(false);

        Task<IChannel> IDiscordClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IChannel>(GetChannel(id));
        Task<IReadOnlyCollection<IPrivateChannel>> IDiscordClient.GetPrivateChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IPrivateChannel>>(PrivateChannels);

        async Task<IReadOnlyCollection<IConnection>> IDiscordClient.GetConnectionsAsync(RequestOptions options)
            => await GetConnectionsAsync(options).ConfigureAwait(false);

        async Task<IInvite> IDiscordClient.GetInviteAsync(string inviteId, RequestOptions options)
            => await GetInviteAsync(inviteId, options).ConfigureAwait(false);

        Task<IGuild> IDiscordClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuild>(GetGuild(id));
        Task<IReadOnlyCollection<IGuild>> IDiscordClient.GetGuildsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);

        async Task<IGuild> IDiscordClient.CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon, RequestOptions options)
            => await CreateGuildAsync(name, region, jpegIcon, options).ConfigureAwait(false);

        Task<IUser> IDiscordClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(id));
        Task<IUser> IDiscordClient.GetUserAsync(string username, string discriminator, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(username, discriminator));

        Task<IVoiceRegion> IDiscordClient.GetVoiceRegionAsync(string id, RequestOptions options)
            => Task.FromResult<IVoiceRegion>(GetVoiceRegion(id));
        Task<IReadOnlyCollection<IVoiceRegion>> IDiscordClient.GetVoiceRegionsAsync(RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IVoiceRegion>>(VoiceRegions);

        public async Task<T> CacheHelper<T>(CacheMode mode, Func<Task<T>> restAction, Func<T> cacheAction)
            where T : class
        {
            switch (mode)
            {
                case CacheMode.CacheOnly:
                    return cacheAction();
                case CacheMode.AllowDownload:
                    return cacheAction() ?? await restAction().ConfigureAwait(false);
                case CacheMode.ForceDownload:
                    return await restAction().ConfigureAwait(false);
                default:
                    throw new InvalidOperationException("Unhandled CacheMode");
            }
        }
    }
}
