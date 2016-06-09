using Discord.API;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    //TODO: Add docstrings
    public interface IDiscordClient
    {
        LoginState LoginState { get; }
        ConnectionState ConnectionState { get; }

        DiscordApiClient ApiClient { get; }
        
        Task LoginAsync(TokenType tokenType, string token, bool validateToken = true);
        Task LogoutAsync();

        Task ConnectAsync();
        Task DisconnectAsync();

        Task<IChannel> GetChannelAsync(ulong id);
        Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync();

        Task<IReadOnlyCollection<IConnection>> GetConnectionsAsync();

        Task<IGuild> GetGuildAsync(ulong id);
        Task<IReadOnlyCollection<IUserGuild>> GetGuildsAsync();
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
