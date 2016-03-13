using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public class VoiceChannel : IPublicChannel, IModifiable<VoiceChannel.Properties>
    {
        public sealed class Properties
        {
            public string Name { get; }
            public int Bitrate { get; set; }
            public int Position { get; }
        }

        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public EntityState State { get; }
        /// <inheritdoc />
        public Server Server { get; }

        /// <inheritdoc />
        public DiscordClient Discord { get; }
        /// <inheritdoc />
        ChannelType IChannel.Type => ChannelType.Public | ChannelType.Voice;

        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public int Position { get; }
        /// <inheritdoc />
        public int Bitrate { get; }

        /// <inheritdoc />
        public string Mention { get; }
        /// <inheritdoc />
        public IEnumerable<PermissionOverwriteEntry> PermissionOverwrites { get; }

        /// <inheritdoc />
        public OverwritePermissions? GetPermissionOverwrite(ServerUser user) => null;
        /// <inheritdoc />
        public OverwritePermissions? GetPermissionOverwrite(Role role) => null;
        /// <inheritdoc />
        public Task<ServerUser> GetUser(ulong id) => null;
        /// <inheritdoc />
        Task<IUser> IChannel.GetUser(ulong id) => null;
        /// <inheritdoc />
        public Task<IEnumerable<ServerUser>> GetUsers() => null;
        /// <inheritdoc />
        Task<IEnumerable<IUser>> IChannel.GetUsers() => null;
        /// <inheritdoc />
        public Task<IEnumerable<Invite>> GetInvites() => null;
                
        /// <inheritdoc />
        public Task UpdatePermissionOverwrite(ServerUser user, OverwritePermissions permissions) => null;
        /// <inheritdoc />
        public Task UpdatePermissionOverwrite(Role role, OverwritePermissions permissions) => null;
        /// <inheritdoc />
        public Task RemovePermissionOverwrite(ServerUser user) => null;
        /// <inheritdoc />
        public Task RemovePermissionOverwrite(Role role) => null;

        /// <inheritdoc />
        public Task<Invite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false) => null;

        /// <inheritdoc />
        public Task Update() => null;
        /// <inheritdoc />
        public Task Modify(Action<Properties> func) => null;
        /// <inheritdoc />
        public Task Delete() => null;
    }
}
