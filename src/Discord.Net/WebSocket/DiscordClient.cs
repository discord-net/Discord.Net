using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord.API;
using Discord.Net.Rest;

namespace Discord.WebSocket
{
    public class DiscordClient : IDiscordClient
    {
        internal int MessageCacheSize { get; } = 100;

        public SelfUser CurrentUser
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TokenType AuthTokenType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DiscordRawClient BaseClient
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRequestQueue RequestQueue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRestClient RestClient
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task<IGuild> CreateGuild(string name, IVoiceRegion region, Stream jpegIcon = null)
        {
            throw new NotImplementedException();
        }

        public Task<IChannel> GetChannel(ulong id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IConnection>> GetConnections()
        {
            throw new NotImplementedException();
        }

        public Task<ISelfUser> GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IDMChannel>> GetDMChannels()
        {
            throw new NotImplementedException();
        }

        public Task<IGuild> GetGuild(ulong id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IUserGuild>> GetGuilds()
        {
            throw new NotImplementedException();
        }

        public Task<IInvite> GetInvite(string inviteIdOrXkcd)
        {
            throw new NotImplementedException();
        }

        public Task<IVoiceRegion> GetOptimalVoiceRegion()
        {
            throw new NotImplementedException();
        }

        public Task<IUser> GetUser(ulong id)
        {
            throw new NotImplementedException();
        }

        public Task<IUser> GetUser(string username, ushort discriminator)
        {
            throw new NotImplementedException();
        }

        public Task<IVoiceRegion> GetVoiceRegion(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IVoiceRegion>> GetVoiceRegions()
        {
            throw new NotImplementedException();
        }

        public Task Login(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task Login(TokenType tokenType, string token, bool validateToken = true)
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IUser>> QueryUsers(string query, int limit)
        {
            throw new NotImplementedException();
        }
    }
}
