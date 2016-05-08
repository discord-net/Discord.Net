using Discord.API;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public interface IDiscordClient
    {
        ISelfUser CurrentUser { get; }
        DiscordRawClient BaseClient { get; }
        //IMessageQueue MessageQueue { get; }

        Task Login(TokenType tokenType, string token);
        Task Logout();

        Task<IChannel> GetChannel(ulong id);
        Task<IEnumerable<IDMChannel>> GetDMChannels();

        Task<IEnumerable<IConnection>> GetConnections();

        Task<IGuild> GetGuild(ulong id);
        Task<IEnumerable<IUserGuild>> GetGuilds();
        Task<IGuild> CreateGuild(string name, IVoiceRegion region, Stream jpegIcon = null);
        
        Task<IPublicInvite> GetInvite(string inviteIdOrXkcd);

        Task<IUser> GetUser(ulong id);
        Task<IUser> GetUser(string username, ushort discriminator);
        Task<ISelfUser> GetCurrentUser();
        Task<IEnumerable<IUser>> QueryUsers(string query, int limit);

        Task<IEnumerable<IVoiceRegion>> GetVoiceRegions();
        Task<IVoiceRegion> GetVoiceRegion(string id);
        Task<IVoiceRegion> GetOptimalVoiceRegion();
    }
}
