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
        ConnectionState ConnectionState { get; }
        ISelfUser CurrentUser { get; }
        TokenType TokenType { get; }

        Task StartAsync();
        Task StopAsync();

        /// <summary>
        ///     Gets the application information associated with this account.
        /// </summary>
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
        ///     Gets a list of direct message channels.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
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
        
        Task<IInvite> GetInviteAsync(string inviteId, RequestOptions options = null);

        Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IUser> GetUserAsync(string username, string discriminator, RequestOptions options = null);

        Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null);
        Task<IVoiceRegion> GetVoiceRegionAsync(string id, RequestOptions options = null);

        Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null);

        Task<int> GetRecommendedShardCountAsync(RequestOptions options = null);
    }
}
