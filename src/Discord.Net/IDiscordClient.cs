using Discord.API;
using Discord.Net.Queue;
using Discord.WebSocket.Data;
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
        IRequestQueue RequestQueue { get; }
        IDataStore DataStore { get; }

        Task Login(string email, string password);
        Task Login(TokenType tokenType, string token, bool validateToken = true);
        Task Logout();

        Task Connect();
        Task Disconnect();

        Task<IChannel> GetChannel(ulong id);
        Task<IEnumerable<IDMChannel>> GetDMChannels();

        Task<IEnumerable<IConnection>> GetConnections();

        Task<IGuild> GetGuild(ulong id);
        Task<IEnumerable<IUserGuild>> GetGuilds();
        Task<IGuild> CreateGuild(string name, IVoiceRegion region, Stream jpegIcon = null);
        
        Task<IInvite> GetInvite(string inviteIdOrXkcd);

        Task<IUser> GetUser(ulong id);
        Task<IUser> GetUser(string username, ushort discriminator);
        Task<ISelfUser> GetCurrentUser();
        Task<IEnumerable<IUser>> QueryUsers(string query, int limit);

        Task<IEnumerable<IVoiceRegion>> GetVoiceRegions();
        Task<IVoiceRegion> GetVoiceRegion(string id);
    }
}
