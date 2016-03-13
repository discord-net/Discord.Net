using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
	public class ServerUser : IUser
    {
        /// <inheritdoc />
        public EntityState State { get; }
        /// <inheritdoc />
        public ulong Id { get; }
        /// <summary> Returns the private channel for this user. </summary>
        public Server Server { get; }

        /// <inheritdoc />
        bool IUser.IsPrivate => false;

        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public ushort Discriminator { get; }
        /// <inheritdoc />
        public bool IsBot { get; }
        /// <inheritdoc />
        public string AvatarId { get; }
        /// <inheritdoc />
        public string CurrentGame { get; }
        /// <inheritdoc />
        public UserStatus Status { get; }
        /// <inheritdoc />
        public DateTime JoinedAt { get; }
        /// <inheritdoc />
        public IReadOnlyList<Role> Roles { get; }

        /// <summary> Returns true if this user has marked themselves as muted. </summary>
        public bool IsSelfMuted { get; }
        /// <summary> Returns true if this user has marked themselves as deafened. </summary>
        public bool IsSelfDeafened { get; }
        /// <summary> Returns true if the server is blocking audio from this user. </summary>
        public bool IsServerMuted { get; }
        /// <summary> Returns true if the server is blocking audio to this user. </summary>
        public bool IsServerDeafened { get; }
        /// <summary> Returns true if the server is temporarily blocking audio to/from this user. </summary>
        public bool IsServerSuppressed { get; }
        /// <summary> Gets this user's current voice channel. </summary>
        public VoiceChannel VoiceChannel { get; }

        /// <inheritdoc />
        public DiscordClient Discord { get; }
        /// <inheritdoc />
        public string AvatarUrl { get; }
        /// <inheritdoc />
        public string Mention { get; }
        
        public ServerPermissions ServerPermissions { get; }

        public ChannelPermissions GetPermissions(IPublicChannel channel) => default(ChannelPermissions);
        /// <inheritdoc />
        public Task<PrivateChannel> GetPrivateChannel() => null;
        public Task<IEnumerable<IPublicChannel>> GetChannels() => null;

        public bool HasRole(Role role) => false;

        public Task AddRoles(params Role[] roles) => null;
        public Task RemoveRoles(params Role[] roles) => null;

        public Task Update() => null;
        public Task Kick() => null;
        public Task Ban(int pruneDays = 0) => null;
        public Task Unban() => null;
    }
}