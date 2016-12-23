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

        Task ConnectAsync();
        Task DisconnectAsync();

        Task<IApplication> GetApplicationInfoAsync();

        Task<IChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload);
        Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync(CacheMode mode = CacheMode.AllowDownload);

        Task<IReadOnlyCollection<IConnection>> GetConnectionsAsync();

        Task<IGuild> GetGuildAsync(ulong id, CacheMode mode = CacheMode.AllowDownload);
        Task<IReadOnlyCollection<IGuild>> GetGuildsAsync(CacheMode mode = CacheMode.AllowDownload);
        Task<IGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null);
        
        Task<IInvite> GetInviteAsync(string inviteId);

        Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload);
        Task<IUser> GetUserAsync(string username, string discriminator);

        Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync();
        Task<IVoiceRegion> GetVoiceRegionAsync(string id);
    }
}
