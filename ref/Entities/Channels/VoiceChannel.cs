using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public class VoiceChannel : IPublicChannel, IVoiceChannel, IModifiable<VoiceChannel.Properties>
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
        public DiscordClient Discord { get; }
        /// <inheritdoc />
        public ModelState State { get; }
        /// <inheritdoc />
        public ChannelType Type { get; }
        /// <inheritdoc />
        public bool IsPrivate => false;
        /// <inheritdoc />
        public bool IsPublic => true;
        /// <inheritdoc />
        public bool IsText => false;
        /// <inheritdoc />
        public bool IsVoice => true;

        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public int Position { get; }
        /// <inheritdoc />
        public int Bitrate { get; }
        /// <inheritdoc />
        public Server Server { get; }
        /// <inheritdoc />
        public IEnumerable<PermissionOverwrite> PermissionOverwrites { get; }

        /// <inheritdoc />
        public PermissionOverwrite? GetPermissionOverwrite(User user) => null;
        /// <inheritdoc />
        public PermissionOverwrite? GetPermissionOverwrite(Role role) => null;
        /// <inheritdoc />
        public Task<IEnumerable<User>> GetUsers() => null;
        /// <inheritdoc />
        public Task<IEnumerable<Invite>> GetInvites() => null;
                
        /// <inheritdoc />
        public Task UpdatePermissionOverwrite(User user, ChannelPermissions allow, ChannelPermissions deny) => null;
        /// <inheritdoc />
        public Task UpdatePermissionOverwrite(User user, TriStateChannelPermissions permissions) => null;
        /// <inheritdoc />
        public Task UpdatePermissionOverwrite(Role role, ChannelPermissions allow, ChannelPermissions deny) => null;
        /// <inheritdoc />
        public Task UpdatePermissionOverwrite(Role role, TriStateChannelPermissions permissions) => null;
        /// <inheritdoc />
        public Task RemovePermissionOverwrite(User user) => null;
        /// <inheritdoc />
        public Task RemovePermissionOverwrite(Role role) => null;

        /// <inheritdoc />
        public Task<Invite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false) => null;

        /// <inheritdoc />
        public Task Delete() => null;
        /// <inheritdoc />
        public Task Update() => null;
        /// <inheritdoc />
        public Task Modify(Action<Properties> func) => null;
    }
}
