using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic Discord client.
    /// </summary>
    public interface IDiscordClient : IDisposable
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
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task StartAsync();
        /// <summary>
        ///     Stops the connection between Discord and the client.
        /// </summary>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
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
        ///     An awaitable <see cref="Task"/> containing the application information.
        /// </returns>
        Task<IApplication> GetApplicationInfoAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a generic channel via the snowflake identifier.
        /// </summary>
        /// <example>
        ///     <code lang="cs" title="Example method">
        ///         var channel = await _client.GetChannelAsync(381889909113225237);
        ///         if (channel != null &amp;&amp; channel is IMessageChannel msgChannel)
        ///         {
        ///             await msgChannel.SendMessageAsync($"{msgChannel} is created at {msgChannel.CreatedAt}");
        ///         }
        ///    </code>
        /// </example>
        /// <param name="id">The snowflake identifier of the channel (e.g. `381889909113225237`).</param>
        /// <param name="mode">
        /// The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the channel associated with the snowflake identifier.
        /// </returns>
        Task<IChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of private channels opened in this session.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
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
        ///     An awaitable <see cref="Task" /> containing a collection of private channels that have been opened in
        ///     this session.
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
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task" /> containing a collection of DM channels that have been opened in this
        ///     session.
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
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task" /> containing a collection of group channels that have been opened in this
        ///     session.
        /// </returns>
        Task<IReadOnlyCollection<IGroupChannel>> GetGroupChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

        Task<IReadOnlyCollection<IConnection>> GetConnectionsAsync(RequestOptions options = null);

        Task<IGuild> GetGuildAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IReadOnlyCollection<IGuild>> GetGuildsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null, RequestOptions options = null);
        
        Task<IInvite> GetInviteAsync(string inviteId, bool withCount = false, RequestOptions options = null);

        /// <summary>
        ///     Gets a user via the snowflake identifier.
        /// </summary>
        /// <example>
        ///     <code lang="cs" title="Example method">
        ///         var user = await _client.GetUserAsync(168693960628371456);
        ///         if (user != null)
        ///             Console.WriteLine($"{user} is created at {user.CreatedAt}.";
        ///     </code>
        /// </example>
        /// <param name="id">The snowflake identifier of the user (e.g. `168693960628371456`).</param>
        /// <param name="mode">
        /// The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the fetched user; <c>null</c> if none is found.
        /// </returns>
        Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a user via the username and discriminator combo.
        /// </summary>
        /// <example>
        ///     <code language="cs" title="Example method">
        ///         var user = await _client.GetUserAsync("Still", "2876");
        ///         if (user != null)
        ///             Console.WriteLine($"{user} is created at {user.CreatedAt}.";
        ///     </code>
        /// </example>
        /// <param name="username">The name of the user (e.g. `Still`).</param>
        /// <param name="discriminator">The discriminator value of the user (e.g. `2876`).</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task" /> containing the fetched user; <c>null</c> if none is found.
        /// </returns>
        Task<IUser> GetUserAsync(string username, string discriminator, RequestOptions options = null);

        Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null);
        Task<IVoiceRegion> GetVoiceRegionAsync(string id, RequestOptions options = null);

        Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null);

        /// <summary>
        ///     Gets the recommended shard count as suggested by Discord.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing an <see cref="int"/> that represents the number of shards
        ///     that should be used with this account.
        /// </returns>
        Task<int> GetRecommendedShardCountAsync(RequestOptions options = null);
    }
}
