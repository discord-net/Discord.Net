using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public interface IDiscordClient : IDisposable
    {
        ConnectionState ConnectionState { get; }
        ISelfUser CurrentUser { get; }
        TokenType TokenType { get; }

        Task StartAsync();
        Task StopAsync();

        Task<IApplication> GetApplicationInfoAsync(RequestOptions options = null);

        Task<IChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
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
    }
}
