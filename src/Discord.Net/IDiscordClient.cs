using Discord.API;
using Discord.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    //TODO: Add docstrings
    //TODO: Docstrings should explain when REST requests are sent and how many
    public interface IDiscordClient : IDisposable
    {
        ConnectionState ConnectionState { get; }

        DiscordRestApiClient ApiClient { get; }
        ILogManager LogManager { get; }

        Task ConnectAsync();
        Task DisconnectAsync();

        Task<IApplication> GetApplicationInfoAsync();

        Task<IChannel> GetChannelAsync(ulong id);
        Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync();

        Task<IReadOnlyCollection<IConnection>> GetConnectionsAsync();

        Task<IGuild> GetGuildAsync(ulong id);
        Task<IReadOnlyCollection<IGuild>> GetGuildsAsync();
        Task<IReadOnlyCollection<IUserGuild>> GetGuildSummariesAsync();
        Task<IGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null);
        
        Task<IInvite> GetInviteAsync(string inviteIdOrXkcd);

        Task<IUser> GetUserAsync(ulong id);
        Task<IUser> GetUserAsync(string username, string discriminator);
        Task<ISelfUser> GetCurrentUserAsync();
        Task<IReadOnlyCollection<IUser>> QueryUsersAsync(string query, int limit);

        Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync();
        Task<IVoiceRegion> GetVoiceRegionAsync(string id);
    }
}
