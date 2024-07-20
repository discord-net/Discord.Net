using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic Discord client.
    /// </summary>
    public interface IDiscordClient : IDisposable, IAsyncDisposable
    {
        /// <summary>
        ///     Gets the current state of connection.
        /// </summary>
        ConnectionState ConnectionState { get; }
        /// <summary>
        ///     Gets the currently logged-in user.
        /// </summary>
        ISelfUser CurrentUser { get; }
        /// <summary>
        ///     Gets the token type of the logged-in user.
        /// </summary>
        TokenType TokenType { get; }

        /// <summary>
        ///     Starts the connection between Discord and the client..
        /// </summary>
        /// <remarks>
        ///     This method will initialize the connection between the client and Discord.
        ///     <note type="important">
        ///         This method will immediately return after it is called, as it will initialize the connection on
        ///         another thread.
        ///     </note>
        /// </remarks>
        /// <returns>
        ///     A task that represents the asynchronous start operation.
        /// </returns>
        Task StartAsync();
        /// <summary>
        ///     Stops the connection between Discord and the client.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous stop operation.
        /// </returns>
        Task StopAsync();

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
        Task<IApplication> GetApplicationInfoAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a generic channel.
        /// </summary>
        /// <example>
        ///     <code language="cs" title="Example method">
        ///     var channel = await _client.GetChannelAsync(381889909113225237);
        ///     if (channel != null &amp;&amp; channel is IMessageChannel msgChannel)
        ///     {
        ///         await msgChannel.SendMessageAsync($"{msgChannel} is created at {msgChannel.CreatedAt}");
        ///     }
        ///     </code>
        /// </example>
        /// <param name="id">The snowflake identifier of the channel (e.g. `381889909113225237`).</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the channel associated
        ///     with the snowflake identifier; <see langword="null" /> when the channel cannot be found.
        /// </returns>
        Task<IChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of private channels opened in this session.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <remarks>
        ///     This method will retrieve all private channels (including direct-message, group channel and such) that
        ///     are currently opened in this session.
        ///     <note type="warning">
        ///         This method will not return previously opened private channels outside of the current session! If
        ///         you have just started the client, this may return an empty collection.
        ///     </note>
        /// </remarks>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of private channels that the user currently partakes in.
        /// </returns>
        Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of direct message channels opened in this session.
        /// </summary>
        /// <remarks>
        ///     This method returns a collection of currently opened direct message channels.
        ///     <note type="warning">
        ///         This method will not return previously opened DM channels outside of the current session! If you
        ///         have just started the client, this may return an empty collection.
        ///     </note>
        /// </remarks>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of direct-message channels that the user currently partakes in.
        /// </returns>
        Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of group channels opened in this session.
        /// </summary>
        /// <remarks>
        ///     This method returns a collection of currently opened group channels.
        ///     <note type="warning">
        ///         This method will not return previously opened group channels outside of the current session! If you
        ///         have just started the client, this may return an empty collection.
        ///     </note>
        /// </remarks>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of group channels that the user currently partakes in.
        /// </returns>
        Task<IReadOnlyCollection<IGroupChannel>> GetGroupChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

        /// <summary>
        ///     Gets the connections that the user has set up.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of connections.
        /// </returns>
        Task<IReadOnlyCollection<IConnection>> GetConnectionsAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a global application command.
        /// </summary>
        /// <param name="id">The id of the command.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the application command if found, otherwise
        ///     <see langword="null"/>.
        /// </returns>
        Task<IApplicationCommand> GetGlobalApplicationCommandAsync(ulong id, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all global commands.
        /// </summary>
        /// <param name="withLocalizations">Whether to include full localization dictionaries in the returned objects, instead of the name localized and description localized fields.</param>
        /// <param name="locale">The target locale of the localized name and description fields. Sets <c>X-Discord-Locale</c> header, which takes precedence over <c>Accept-Language</c>.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of global
        ///     application commands.
        /// </returns>
        Task<IReadOnlyCollection<IApplicationCommand>> GetGlobalApplicationCommandsAsync(bool withLocalizations = false, string locale = null, RequestOptions options = null);

        /// <summary>
        ///     Creates a global application command.
        /// </summary>
        /// <param name="properties">The properties to use when creating the command.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///      A task that represents the asynchronous creation operation. The task result contains the created application command.
        /// </returns>
        Task<IApplicationCommand> CreateGlobalApplicationCommand(ApplicationCommandProperties properties, RequestOptions options = null);

        /// <summary>
        ///     Bulk overwrites all global application commands.
        /// </summary>
        /// <param name="properties">A collection of properties to use when creating the commands.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains a collection of application commands that were created.
        /// </returns>
        Task<IReadOnlyCollection<IApplicationCommand>> BulkOverwriteGlobalApplicationCommand(ApplicationCommandProperties[] properties, RequestOptions options = null);

        /// <summary>
        ///     Gets a guild.
        /// </summary>
        /// <param name="id">The guild snowflake identifier.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the guild associated
        ///     with the snowflake identifier; <see langword="null" /> when the guild cannot be found.
        /// </returns>
        Task<IGuild> GetGuildAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of guilds that the user is currently in.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of guilds that the current user is in.
        /// </returns>
        Task<IReadOnlyCollection<IGuild>> GetGuildsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
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
        Task<IGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null, RequestOptions options = null);

        /// <summary>
        ///     Gets an invite.
        /// </summary>
        /// <param name="inviteId">The invitation identifier.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the invite information.
        /// </returns>
        Task<IInvite> GetInviteAsync(string inviteId, RequestOptions options = null);

        /// <summary>
        ///     Gets a user.
        /// </summary>
        /// <example>
        ///     <code language="cs" title="Example method">
        ///     var user = await _client.GetUserAsync(168693960628371456);
        ///     if (user != null)
        ///         Console.WriteLine($"{user} is created at {user.CreatedAt}.";
        ///     </code>
        /// </example>
        /// <param name="id">The snowflake identifier of the user (e.g. `168693960628371456`).</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the user associated with
        ///     the snowflake identifier; <see langword="null" /> if the user is not found.
        /// </returns>
        Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a user.
        /// </summary>
        /// <example>
        ///     <code language="cs" title="Example method">
        ///    var user = await _client.GetUserAsync("Still", "2876");
        ///    if (user != null)
        ///        Console.WriteLine($"{user} is created at {user.CreatedAt}.";
        ///    </code>
        /// </example>
        /// <param name="username">The name of the user (e.g. `Still`).</param>
        /// <param name="discriminator">The discriminator value of the user (e.g. `2876`).</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the user associated with
        ///     the name and the discriminator; <see langword="null" /> if the user is not found.
        /// </returns>
        Task<IUser> GetUserAsync(string username, string discriminator, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of the available voice regions.
        /// </summary>
        /// <example>
        ///     The following example gets the most optimal voice region from the collection.
        ///     <code language="cs">
        ///         var regions = await client.GetVoiceRegionsAsync();
        ///         var optimalRegion = regions.FirstOrDefault(x =&gt; x.IsOptimal);
        ///     </code>
        /// </example>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     with all of the available voice regions in this session.
        /// </returns>
        Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null);
        /// <summary>
        ///     Gets a voice region.
        /// </summary>
        /// <param name="id">The identifier of the voice region (e.g. <c>eu-central</c> ).</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the voice region
        ///     associated with the identifier; <see langword="null" /> if the voice region is not found.
        /// </returns>
        Task<IVoiceRegion> GetVoiceRegionAsync(string id, RequestOptions options = null);

        /// <summary>
        ///     Gets a webhook available.
        /// </summary>
        /// <param name="id">The identifier of the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a webhook associated
        ///     with the identifier; <see langword="null" /> if the webhook is not found.
        /// </returns>
        Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null);

        /// <summary>
        ///     Gets the recommended shard count as suggested by Discord.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains an <see cref="int"/>
        ///     that represents the number of shards that should be used with this account.
        /// </returns>
        Task<int> GetRecommendedShardCountAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets the gateway information related to the bot.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a <see cref="BotGateway"/>
        ///     that represents the gateway information related to the bot.
        /// </returns>
        Task<BotGateway> GetBotGatewayAsync(RequestOptions options = null);

        /// <summary>
        ///     Creates a test entitlement to a given SKU for a given guild or user. 
        /// </summary>
        Task<IEntitlement> CreateTestEntitlementAsync(ulong skuId, ulong ownerId, SubscriptionOwnerType ownerType, RequestOptions options = null);

        /// <summary>
        ///     Deletes a currently-active test entitlement.
        /// </summary>
        Task DeleteTestEntitlementAsync(ulong entitlementId, RequestOptions options = null);

        /// <summary>
        ///     Returns all entitlements for a given app, active and expired.
        /// </summary>
        IAsyncEnumerable<IReadOnlyCollection<IEntitlement>> GetEntitlementsAsync(int? limit = 100,
            ulong? afterId = null, ulong? beforeId = null, bool excludeEnded = false, ulong? guildId = null, ulong? userId = null,
            ulong[] skuIds = null, RequestOptions options = null);

        /// <summary>
        ///     Returns all SKUs for a given application. 
        /// </summary>
        Task<IReadOnlyCollection<SKU>> GetSKUsAsync(RequestOptions options = null);

        /// <summary>
        ///     Marks a given one-time purchase entitlement for the user as consumed.
        /// </summary>
        /// <param name="entitlementId">The id of the entitlement.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        Task ConsumeEntitlementAsync(ulong entitlementId, RequestOptions options = null);
    }
}
