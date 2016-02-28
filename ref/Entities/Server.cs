using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
	public class Server : IEntity<ulong>
    {
        public class Emoji
        {
            public string Id { get; }
            public string Name { get; }
            public bool IsManaged { get; }
            public bool RequireColons { get; }
            public IEnumerable<Role> Roles { get; }
        }
        
        public ulong Id { get; }
        public DiscordClient Discord { get; }
        public EntityState State { get; }

        public User CurrentUser { get; }
        public string IconId { get; }
        public string SplashId { get; }
        public string IconUrl { get; }
        public string SplashUrl { get; }
        public int ChannelCount { get; }
        public int UserCount { get; }
        public int RoleCount { get; }
        public TextChannel DefaultChannel { get; } 
        public Role EveryoneRole { get; }
        public IEnumerable<string> Features { get; }
        public IEnumerable<Emoji> CustomEmojis { get; }
        public IEnumerable<IChannel> Channels { get; }
        public IEnumerable<TextChannel> TextChannels { get; }
        public IEnumerable<VoiceChannel> VoiceChannels { get; }
        public IEnumerable<User> Users { get; }
        public IEnumerable<Role> Roles { get; }

        public string Name { get; set; }
        public Region Region { get; set; }
        public int AFKTimeout { get; set; }
        public DateTime JoinedAt { get; set; }
        public User Owner { get; set; }                
        public VoiceChannel AFKChannel { get; set; }        
                
        public IPublicChannel GetChannel(ulong id) => null;
        public IPublicChannel GetChannel(string mention) => null;
        public Role GetRole(ulong id) => null;
        public User GetUser(ulong id) => null;
        public User GetUser(string name, ushort discriminator) => null;
        public User GetUser(string mention) => null;
        public Task<IEnumerable<User>> DownloadBans() => null;
        public Task<IEnumerable<Invite>> DownloadInvites() => null;

        public Task<TextChannel> CreateTextChannel(string name) => null;
        public Task<VoiceChannel> CreateVoiceChannel(string name) => null;
        public Task<Invite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false) => null;
        public Task<Role> CreateRole(string name, ServerPermissions? permissions = null, Color color = null, bool isHoisted = false) => null;

        public Task Ban(User user, int pruneDays = 0) => null;
        public Task Unban(User user) => null;
        public Task Unban(ulong userId) => null;
        
        public Task<int> PruneUsers(int days = 30, bool simulate = false) => null;

        public Task Update() => null;
        public Task Leave() => null;
        public Task Delete() => null;
    }
}
