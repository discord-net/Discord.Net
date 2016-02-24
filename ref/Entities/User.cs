using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
	public class User : IModel<ulong>
    {
        public DiscordClient Client { get; }
        
        public ulong Id { get; }
        public Server Server { get; }
        
        public string Name { get; }
        public ushort Discriminator { get; }
        public string AvatarId { get; }
        public string CurrentGame { get; }
        public UserStatus Status { get; }
        public DateTime JoinedAt { get; }
        public DateTime? LastActivityAt { get; }
        
        public Channel PrivateChannel => null;
        public string Mention => null;
        public bool IsSelfMuted => false;
        public bool IsSelfDeafened => false;
        public bool IsServerMuted => false;
        public bool IsServerDeafened => false;
        public bool IsServerSuppressed => false;
        public DateTime? LastOnlineAt => null;
        public Channel VoiceChannel => null;
        public string AvatarUrl => null;
        public IEnumerable<Role> Roles => null;
        
        public IEnumerable<Channel> Channels => null;

        public Task Kick() => null;

        public ServerPermissions ServerPermissions => default(ServerPermissions);
        public ChannelPermissions GetPermissions(Channel channel) => default(ChannelPermissions);
        
        public Task<Channel> CreatePMChannel() => null;

        public Task<Message> SendMessage(string text) => null;
        public Task<Message> SendFile(string filePath) => null;
        public Task<Message> SendFile(string filename, Stream stream) => null;

        public bool HasRole(Role role) => false;

        public Task AddRoles(params Role[] roles) => null;
        public Task RemoveRoles(params Role[] roles) => null;

        public Task Save() => null;
    }
}