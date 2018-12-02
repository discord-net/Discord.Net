using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord.API;
using Discord.Rest;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the base of a WebSocket-based Discord client.
    /// </summary>
    public abstract partial class BaseSocketClient : BaseDiscordClient, IDiscordClient
    {
        protected readonly DiscordSocketConfig BaseConfig;

        /// <summary>
        ///     Gets the estimated round-trip latency, in milliseconds, to the gateway server.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> that represents the round-trip latency to the WebSocket server. Please
        ///     note that this value does not represent a "true" latency for operations such as sending a message.
        /// </returns>
        public abstract int Latency { get; protected set; }
        /// <summary>
        ///     Gets the status for the logged-in user.
        /// </summary>
        /// <returns>
        ///     A status object that represents the user's online presence status.
        /// </returns>
        public abstract UserStatus Status { get; protected set; }
        /// <summary>
        ///     Gets the activity for the logged-in user.
        /// </summary>
        /// <returns>
        ///     An activity object that represents the user's current activity.
        /// </returns>
        public abstract IActivity Activity { get; protected set; }

        internal new DiscordSocketApiClient ApiClient => base.ApiClient as DiscordSocketApiClient;

        /// <summary>
        ///     Gets the current logged-in user.
        /// </summary>
        public new SocketSelfUser CurrentUser { get => base.CurrentUser as SocketSelfUser; protected set => base.CurrentUser = value; }
        /// <summary>
        ///     Gets a collection of guilds that the user is currently in.
        /// </summary>
        /// <returns>
        ///     A read-only collection of guilds that the current user is in.
        /// </returns>
        public abstract IReadOnlyCollection<SocketGuild> Guilds { get; }
        /// <summary>
        ///     Gets a collection of private channels opened in this session.
        /// </summary>
        /// <remarks>
        ///     This method will retrieve all private channels (including direct-message, group channel and such) that
        ///     are currently opened in this session.
        ///     <note type="warning">
        ///         This method will not return previously opened private channels outside of the current session! If
        ///         you have just started the client, this may return an empty collection.
        ///     </note>
        /// </remarks>
        /// <returns>
        ///     A read-only collection of private channels that the user currently partakes in.
        /// </returns>
        public abstract IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels { get; }
        /// <summary>
        ///     Gets a collection of available voice regions.
        /// </summary>
        /// <returns>
        ///     A read-only collection of voice regions that the user has access to.
        /// </returns>
        public abstract IReadOnlyCollection<RestVoiceRegion> VoiceRegions { get; }

        internal BaseSocketClient(DiscordSocketConfig config, DiscordRestApiClient client)
            : base(config, client) => BaseConfig = config;
        private static DiscordSocketApiClient CreateApiClient(DiscordSocketConfig config)
            => new DiscordSocketApiClient(config.RestClientProvider, config.WebSocketProvider, DiscordRestConfig.UserAgent);

        /// <summary>
        ///     Gets a Discord application information for the logged-in user.
        /// </summary>
        /// <remarks>
        ///     This method reflects your application information you submitted when creating a Discord application via
        ///     the Developer Portal.
        /// </remarks>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the application
        ///     information.
        /// </returns>
        public abstract Task<RestApplication> GetApplicationInfoAsync(RequestOptions options = null);
        /// <summary>
        ///     Gets a generic user.
        /// </summary>
        /// <param name="id">The user snowflake ID.</param>
        /// <remarks>
        ///     This method gets the user present in the WebSocket cache with the given condition.
        ///     <note type="warning">
        ///         Sometimes a user may return <c>null</c> due to Discord not sending offline users in large
        ///         guilds (i.e. guild with 100+ members) actively. To download users on startup, consider enabling 
        ///         <see cref="DiscordSocketConfig.AlwaysDownloadUsers"/>.
        ///     </note>
        ///     <note>
        ///         This method does not attempt to fetch users that the logged-in user does not have access to (i.e.
        ///         users who don't share mutual guild(s) with the current user).
        ///     </note>
        /// </remarks>
        /// <returns>
        ///     A generic WebSocket-based user; <c>null</c> when the user cannot be found.
        /// </returns>
        public abstract SocketUser GetUser(ulong id);

        /// <summary>
        ///     Gets a user.
        /// </summary>
        /// <param name="username">The name of the user.</param>
        /// <param name="discriminator">The discriminator value of the user.</param>
        /// <remarks>
        ///     This method gets the user present in the WebSocket cache with the given condition.
        ///     <note type="warning">
        ///         Sometimes a user may return <c>null</c> due to Discord not sending offline users in large
        ///         guilds (i.e. guild with 100+ members) actively. To download users on startup, consider enabling 
        ///         <see cref="DiscordSocketConfig.AlwaysDownloadUsers"/>.
        ///     </note>
        ///     <note>
        ///         This method does not attempt to fetch users that the logged-in user does not have access to (i.e.
        ///         users who don't share mutual guild(s) with the current user).
        ///     </note>
        /// </remarks>
        /// <returns>
        ///     A generic WebSocket-based user; <c>null</c> when the user cannot be found.
        /// </returns>
        public abstract SocketUser GetUser(string username, string discriminator);
        /// <summary>
        ///     Gets a channel.
        /// </summary>
        /// <param name="id">The snowflake identifier of the channel (e.g. `381889909113225237`).</param>
        /// <returns>
        ///     A generic WebSocket-based channel object (voice, text, category, etc.) associated with the identifier;
        ///     <c>null</c> when the channel cannot be found.
        /// </returns>
        public abstract SocketChannel GetChannel(ulong id);
        /// <summary>
        ///     Gets a guild.
        /// </summary>
        /// <param name="id">The guild snowflake identifier.</param>
        /// <returns>
        ///     A WebSocket-based guild associated with the snowflake identifier; <c>null</c> when the guild cannot be
        ///     found.
        /// </returns>
        public abstract SocketGuild GetGuild(ulong id);
        /// <summary>
        ///     Gets a voice region.
        /// </summary>
        /// <param name="id">The identifier of the voice region (e.g. <c>eu-central</c> ).</param>
        /// <returns>
        ///     A REST-based voice region associated with the identifier; <c>null</c> if the voice region is not
        ///     found.
        /// </returns>
        public abstract RestVoiceRegion GetVoiceRegion(string id);
        /// <inheritdoc />
        public abstract Task StartAsync();
        /// <inheritdoc />
        public abstract Task StopAsync();
        /// <summary>
        ///     Sets the current status of the user (e.g. Online, Do not Disturb).
        /// </summary>
        /// <param name="status">The new status to be set.</param>
        /// <returns>
        ///     A task that represents the asynchronous set operation.
        /// </returns>
        public abstract Task SetStatusAsync(UserStatus status);
        /// <summary>
        ///     Sets the game of the user.
        /// </summary>
        /// <param name="name">The name of the game.</param>
        /// <param name="streamUrl">If streaming, the URL of the stream. Must be a valid Twitch URL.</param>
        /// <param name="type">The type of the game.</param>
        /// <returns>
        ///     A task that represents the asynchronous set operation.
        /// </returns>
        public abstract Task SetGameAsync(string name, string streamUrl = null, ActivityType type = ActivityType.Playing);
        /// <summary>
        ///     Sets the <paramref name="activity"/> of the logged-in user.
        /// </summary>
        /// <remarks>
        ///     This method sets the <paramref name="activity"/> of the user. 
        ///     <note type="note">
        ///         Discord will only accept setting of name and the type of activity.
        ///     </note>
        ///     <note type="warning">
        ///         Rich Presence cannot be set via this method or client. Rich Presence is strictly limited to RPC
        ///         clients only. 
        ///     </note>
        /// </remarks>
        /// <param name="activity">The activity to be set.</param>
        /// <returns>
        ///     A task that represents the asynchronous set operation.
        /// </returns>
        public abstract Task SetActivityAsync(IActivity activity);
        /// <summary>
        ///     Attempts to download users into the user cache for the selected guilds.
        /// </summary>
        /// <param name="guilds">The guilds to download the members from.</param>
        /// <returns>
        ///     A task that represents the asynchronous download operation.
        /// </returns>
        public abstract Task DownloadUsersAsync(IEnumerable<IGuild> guilds);

        /// <summary>
        ///     Creates a guild for the logged-in user who is in less than 10 active guilds.
        /// </summary>
        /// <remarks>
        ///     This method creates a new guild on behalf of the logged-in user. 
        ///     <note type="warning">
        ///         Due to Discord's limitation, this method will only work for users that are in less than 10 guilds.
        ///     </note>
        /// </remarks>
        /// <param name="name">The name of the new guild.</param>
        /// <param name="region">The voice region to create the guild with.</param>
        /// <param name="jpegIcon">The icon of the guild.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the created guild.
        /// </returns>
        public Task<RestGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null, RequestOptions options = null)
            => ClientHelper.CreateGuildAsync(this, name, region, jpegIcon, options ?? RequestOptions.Default);
        /// <summary>
        ///     Gets the connections that the user has set up.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of connections.
        /// </returns>
        public Task<IReadOnlyCollection<RestConnection>> GetConnectionsAsync(RequestOptions options = null)
            => ClientHelper.GetConnectionsAsync(this, options ?? RequestOptions.Default);
        /// <summary>
        ///     Gets an invite.
        /// </summary>
        /// <param name="inviteId">The invitation identifier.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the invite information.
        /// </returns>
        public Task<RestInviteMetadata> GetInviteAsync(string inviteId, RequestOptions options = null)
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
