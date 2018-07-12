using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic guild user.
    /// </summary>
    public interface IGuildUser : IUser, IVoiceState
    {
        /// <summary>
        ///     Gets when this user joined the guild.
        /// </summary>
        /// <returns>
        ///     A <see cref="DateTimeOffset"/> representing the time of which the user has joined the guild; 
        ///     <c>null</c> when it cannot be obtained.
        /// </returns>
        DateTimeOffset? JoinedAt { get; }
        /// <summary>
        ///     Gets the nickname for this user.
        /// </summary>
        /// <returns>
        ///     A string representing the nickname of the user; <c>null</c> if none is set.
        /// </returns>
        string Nickname { get; }
        /// <summary>
        ///     Gets the guild-level permissions for this user.
        /// </summary>
        /// <returns>
        ///     A <see cref="Discord.GuildPermissions"/> structure for this user, representing what
        ///     permissions this user has in the guild.
        /// </returns>
        GuildPermissions GuildPermissions { get; }

        /// <summary>
        ///     Gets the guild for this user.
        /// </summary>
        /// <returns>
        ///     A guild object that this user belongs to.
        /// </returns>
        IGuild Guild { get; }
        /// <summary>
        ///     Gets the ID of the guild for this user.
        /// </summary>
        /// <returns>
        ///     An <see cref="ulong"/> representing the snowflake identifier of the guild that this user belongs to.
        /// </returns>
        ulong GuildId { get; }
        /// <summary>
        ///     Gets a collection of IDs for the roles that this user currently possesses in the guild.
        /// </summary>
        /// <returns>
        ///     A read-only collection of <see cref="ulong"/>, each representing a snowflake identifier for a role that
        ///     this user posesses.
        /// </returns>
        IReadOnlyCollection<ulong> RoleIds { get; }

        /// <summary>
        ///     Gets the level permissions granted to this user to a given channel.
        /// </summary>
        /// <param name="channel">The channel to get the permission from.</param>
        /// <returns>
        ///     A <see cref="Discord.ChannelPermissions"/> structure representing the permissions that a user has in the
        ///     specified channel.
        /// </returns>
        ChannelPermissions GetPermissions(IGuildChannel channel);

        /// <summary>
        ///     Kicks this user from this guild.
        /// </summary>
        /// <param name="reason">The reason for the kick which will be recorded in the audit log.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous kick operation.
        /// </returns>
        Task KickAsync(string reason = null, RequestOptions options = null);
        /// <summary>
        ///     Modifies this user's properties in this guild.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the user with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        /// <seealso cref="Discord.GuildUserProperties"/>
        Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Adds the specified <paramref name="role"/> to this user in this guild.
        /// </summary>
        /// <param name="role">The role to be added to the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role addition operation.
        /// </returns>
        Task AddRoleAsync(IRole role, RequestOptions options = null);
        /// <summary>
        ///     Adds the specified <paramref name="roles"/> to this user in this guild.
        /// </summary>
        /// <param name="roles">The roles to be added to the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role addition operation.
        /// </returns>
        Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null);
        /// <summary>
        ///     Removes the specified <paramref name="role"/> from this user in this guild.
        /// </summary>
        /// <param name="role">The role to be removed from the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role removal operation.
        /// </returns>
        Task RemoveRoleAsync(IRole role, RequestOptions options = null);
        /// <summary>
        ///     Removes the specified <paramref name="roles"/> from this user in this guild.
        /// </summary>
        /// <param name="roles">The roles to be removed from the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role removal operation.
        /// </returns>
        Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null);
    }
}
