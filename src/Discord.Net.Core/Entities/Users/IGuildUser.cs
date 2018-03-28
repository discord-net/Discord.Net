using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> Represents a Discord user that is in a guild. </summary>
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
        /// <param name="channel"> The channel to get the permission from. </param>
        ChannelPermissions GetPermissions(IGuildChannel channel);

        /// <summary> Kicks this user from this guild. </summary>
        /// <param name="reason"> The reason for the kick which will be recorded in the audit log. </param>
        Task KickAsync(string reason = null, RequestOptions options = null);
        /// <summary> Modifies this user's properties in this guild. </summary>
        Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions options = null);

        /// <summary> Adds a role to this user in this guild. </summary>
        /// <param name="role"> The role to be added to the user. </param>
        Task AddRoleAsync(IRole role, RequestOptions options = null);
        /// <summary> Adds roles to this user in this guild. </summary>
        /// <param name="roles"> The roles to be added to the user. </param>
        Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null);
        /// <summary> Removes a role from this user in this guild. </summary>
        /// <param name="role"> The role to be removed from the user. </param>
        Task RemoveRoleAsync(IRole role, RequestOptions options = null);
        /// <summary> Removes roles from this user in this guild. </summary>
        /// <param name="roles"> The roles to be removed from the user. </param>
        Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null);
    }
}
