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
        
        Task Login(TokenType tokenType, string token, bool validateToken = true);
        Task Logout();

        Task Connect();
        Task Disconnect();

        Task<IChannel> GetChannel(ulong id);
        Task<IReadOnlyCollection<IDMChannel>> GetDMChannels();

        Task<IReadOnlyCollection<IConnection>> GetConnections();

        Task<IGuild> GetGuild(ulong id);
        Task<IReadOnlyCollection<IUserGuild>> GetGuilds();
        Task<IGuild> CreateGuild(string name, IVoiceRegion region, Stream jpegIcon = null);
        
        Task<IInvite> GetInvite(string inviteIdOrXkcd);

        Task<IUser> GetUser(ulong id);
        Task<IUser> GetUser(string username, string discriminator);
        Task<ISelfUser> GetCurrentUser();
        Task<IReadOnlyCollection<IUser>> QueryUsers(string query, int limit);

        Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegions();
        Task<IVoiceRegion> GetVoiceRegion(string id);
    }
}
