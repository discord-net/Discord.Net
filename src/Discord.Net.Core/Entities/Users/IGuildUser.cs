using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> A Guild-User pairing. </summary>
    public interface IGuildUser : IUser, IVoiceState
    {
        /// <summary> Gets when this user joined this guild. </summary>
        DateTimeOffset? JoinedAt { get; }
        /// <summary> Gets the nickname for this user. </summary>
        string Nickname { get; }
        /// <summary> Gets the guild-level permissions for this user. </summary>
        GuildPermissions GuildPermissions { get; }

        /// <summary> Gets the guild for this user. </summary>
        IGuild Guild { get; }
        /// <summary> Gets the id of the guild for this user. </summary>
        ulong GuildId { get; }
        /// <summary> Returns a collection of the ids of the roles this user is a member of in this guild, including the guild's @everyone role. </summary>
        IReadOnlyCollection<ulong> RoleIds { get; }

        /// <summary> Gets the level permissions granted to this user to a given channel. </summary>
        ChannelPermissions GetPermissions(IGuildChannel channel);

        /// <summary> Kicks this user from this guild. </summary>
        Task KickAsync(RequestOptions options = null);
        /// <summary> Modifies this user's properties in this guild. </summary>
        Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions options = null);

        /// <summary> Adds a role to this user in this guild. </summary>
        Task AddRoleAsync(IRole role, RequestOptions options = null);
        /// <summary> Adds roles to this user in this guild. </summary>
        Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null);
        /// <summary> Removes a role from this user in this guild. </summary>
        Task RemoveRoleAsync(IRole role, RequestOptions options = null);
        /// <summary> Removes roles from this user in this guild. </summary>
        Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null);
    }
}
