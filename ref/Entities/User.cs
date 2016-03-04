using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
	public class User : IEntity<ulong>
    {        
        public ulong Id { get; }
        public DiscordClient Discord { get; }
        public EntityState State { get; }

        public Server Server { get; }

        public string Name { get; }
        public ushort Discriminator { get; }
        public string AvatarId { get; }
        public string CurrentGame { get; }
        public UserStatus Status { get; }
        public DateTime JoinedAt { get; }
        public DateTime? LastActivityAt { get; }
        
        public string Mention => null;
        public bool IsSelfMuted => false;
        public bool IsSelfDeafened => false;
        public bool IsServerMuted => false;
        public bool IsServerDeafened => false;
        public bool IsServerSuppressed => false;
        public DateTime? LastOnlineAt => null;
        public VoiceChannel VoiceChannel => null;
        public string AvatarUrl => null;
        public IEnumerable<Role> Roles => null;        
        public IEnumerable<IPublicChannel> Channels => null;
        public ServerPermissions ServerPermissions => default(ServerPermissions);

        public ChannelPermissions GetPermissions(IPublicChannel channel) => default(ChannelPermissions);
        public Task<PrivateChannel> GetPrivateChannel() => null;

        public Task<Message> SendMessage(string text) => null;
        public Task<Message> SendFile(string filePath) => null;
        public Task<Message> SendFile(string filename, Stream stream) => null;

        public bool HasRole(Role role) => false;

        public Task AddRoles(params Role[] roles) => null;
        public Task RemoveRoles(params Role[] roles) => null;

        public Task Update() => null;
        public Task Kick() => null;
        public Task Ban(User user, int pruneDays = 0) => null;
        public Task Unban(User user) => null;
        public Task Unban(ulong userId) => null;
    }
}