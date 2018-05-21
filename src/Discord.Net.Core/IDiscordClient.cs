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

        Task StartAsync();
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
        ///     Gets a generic channel with the provided ID.
        /// </summary>
        /// <param name="id">The ID of the channel.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        Task<IChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a list of private channels.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Returns a collection of direct message channels.
        /// </summary>
        /// <remarks>
        ///     This method returns a collection of currently opened direct message channels.
        ///     <note type="note">
        ///         This method will not return previously opened DM channels outside of the current session! If you
        ///         have just started the client, this may return an empty collection.
        ///     </note>
        /// </remarks>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <returns>
        ///     An awaitable <see cref="Task" /> containing a collection of DM channels.
        /// </returns>
        Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a list of group channels.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        Task<IReadOnlyCollection<IGroupChannel>> GetGroupChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

        Task<IReadOnlyCollection<IConnection>> GetConnectionsAsync(RequestOptions options = null);

        Task<IGuild> GetGuildAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IReadOnlyCollection<IGuild>> GetGuildsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null, RequestOptions options = null);
        
        Task<IInvite> GetInviteAsync(string inviteId, bool withCount = false, RequestOptions options = null);

        Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IUser> GetUserAsync(string username, string discriminator, RequestOptions options = null);

        Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null);
        Task<IVoiceRegion> GetVoiceRegionAsync(string id, RequestOptions options = null);

        Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null);

        Task<int> GetRecommendedShardCountAsync(RequestOptions options = null);
    }
}
