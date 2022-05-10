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
        ///     Gets the displayed name for this user.
        /// </summary>
        /// <returns>
        ///     A string representing the display name of the user; If the nickname is null, this will be the username.
        /// </returns>
        string DisplayName { get; }
        /// <summary>
        ///     Gets the nickname for this user.
        /// </summary>
        /// <returns>
        ///     A string representing the nickname of the user; <c>null</c> if none is set.
        /// </returns>
        string Nickname { get; }
        /// <summary>
        ///     Gets the displayed avatar for this user.
        /// </summary>
        /// <returns>
        ///     The users displayed avatar hash. If the user does not have a guild avatar, this will be the regular avatar.
        ///     If the user also does not have a regular avatar, this will be <see langword="null"/>.
        /// </returns>
        string DisplayAvatarId { get; }
        /// <summary>
        ///     Gets the guild specific avatar for this user.
        /// </summary>
        /// <returns>
        ///     The users guild avatar hash if they have one; otherwise <see langword="null"/>.
        /// </returns>
        string GuildAvatarId { get; }
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
        ///     Gets the date and time for when this user's guild boost began.
        /// </summary>
        /// <returns>
        ///     A <see cref="DateTimeOffset"/> for when the user began boosting this guild; <c>null</c> if they are not boosting the guild.
        /// </returns>
        DateTimeOffset? PremiumSince { get; }
        /// <summary>
        ///     Gets a collection of IDs for the roles that this user currently possesses in the guild.
        /// </summary>
        /// <remarks>
        ///     This property returns a read-only collection of the identifiers of the roles that this user possesses.
        ///     For WebSocket users, a Roles property can be found in place of this property. Due to the REST
        ///     implementation, only a collection of identifiers can be retrieved instead of the full role objects.
        /// </remarks>
        /// <returns>
        ///     A read-only collection of <see cref="ulong"/>, each representing a snowflake identifier for a role that
        ///     this user possesses.
        /// </returns>
        IReadOnlyCollection<ulong> RoleIds { get; }

        /// <summary>
        ///     Whether the user has passed the guild's Membership Screening requirements.
        /// </summary>
        bool? IsPending { get; }

        /// <summary>
        ///     Gets the users position within the role hierarchy.
        /// </summary>
        int Hierarchy { get; }

        /// <summary>
        ///     Gets the date and time that indicates if and for how long a user has been timed out.
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> or a timestamp in the past if the user is not timed out.
        /// </remarks>
        /// <returns>
        ///     A <see cref="DateTimeOffset"/> indicating how long the user will be timed out for.
        /// </returns>
        DateTimeOffset? TimedOutUntil { get; }

        /// <summary>
        ///     Gets the level permissions granted to this user to a given channel.
        /// </summary>
        /// <example>
        ///     <para>The following example checks if the current user has the ability to send a message with attachment in
        ///     this channel; if so, uploads a file via <see cref="IMessageChannel.SendFileAsync(string, string, bool, Embed, RequestOptions, bool, AllowedMentions, MessageReference, MessageComponent, ISticker[], Embed[], MessageFlags)"/>.</para>
        /// <code language="cs">
        ///     if (currentUser?.GetPermissions(targetChannel)?.AttachFiles)
        ///         await targetChannel.SendFileAsync("fortnite.png");
        ///     </code>
        /// </example>
        /// <param name="channel">The channel to get the permission from.</param>
        /// <returns>
        ///     A <see cref="Discord.ChannelPermissions"/> structure representing the permissions that a user has in the
        ///     specified channel.
        /// </returns>
        ChannelPermissions GetPermissions(IGuildChannel channel);

        /// <summary>
        ///     Gets the guild avatar URL for this user.
        /// </summary>
        /// <remarks>
        ///     This property retrieves a URL for this guild user's guild specific avatar. In event that the user does not have a valid guild avatar
        ///     (i.e. their avatar identifier is not set), this method will return <see langword="null"/>.
        /// </remarks>
        /// <param name="format">The format to return.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.
        /// </param>
        /// <returns>
        ///     A string representing the user's avatar URL; <see langword="null"/> if the user does not have an avatar in place.
        /// </returns>
        string GetGuildAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128);
        /// <summary>
        ///     Gets the display avatar URL for this user.
        /// </summary>
        /// <remarks>
        ///     This property retrieves an URL for this guild user's displayed avatar.
        ///     If the user does not have a guild avatar, this will be the user's regular avatar.
        /// </remarks>
        /// <param name="format">The format to return.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.</param>
        /// <returns>
        ///     A string representing the URL of the displayed avatar for this user. <see langword="null"/> if the user does not have an avatar in place.
        /// </returns>
        string GetDisplayAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128);
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
        /// <remarks>
        ///     This method modifies the current guild user with the specified properties. To see an example of this
        ///     method and what properties are available, please refer to <see cref="GuildUserProperties"/>.
        /// </remarks>
        /// <param name="func">The delegate containing the properties to modify the user with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions options = null);
        /// <summary>
        ///     Adds the specified role to this user in the guild.
        /// </summary>
        /// <param name="roleId">The role to be added to the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role addition operation.
        /// </returns>
        Task AddRoleAsync(ulong roleId, RequestOptions options = null);
        /// <summary>
        ///     Adds the specified role to this user in the guild.
        /// </summary>
        /// <param name="role">The role to be added to the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role addition operation.
        /// </returns>
        Task AddRoleAsync(IRole role, RequestOptions options = null);
        /// <summary>
        ///     Adds the specified <paramref name="roleIds"/> to this user in the guild.
        /// </summary>
        /// <param name="roleIds">The roles to be added to the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role addition operation.
        /// </returns>
        Task AddRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null);
        /// <summary>
        ///     Adds the specified <paramref name="roles"/> to this user in the guild.
        /// </summary>
        /// <param name="roles">The roles to be added to the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role addition operation.
        /// </returns>
        Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null);
        /// <summary>
        ///     Removes the specified <paramref name="roleId"/> from this user in the guild.
        /// </summary>
        /// <param name="roleId">The role to be removed from the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role removal operation.
        /// </returns>
        Task RemoveRoleAsync(ulong roleId, RequestOptions options = null);
        /// <summary>
        ///     Removes the specified <paramref name="role"/> from this user in the guild.
        /// </summary>
        /// <param name="role">The role to be removed from the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role removal operation.
        /// </returns>
        Task RemoveRoleAsync(IRole role, RequestOptions options = null);
        /// <summary>
        ///     Removes the specified <paramref name="roleIds"/> from this user in the guild.
        /// </summary>
        /// <param name="roleIds">The roles to be removed from the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role removal operation.
        /// </returns>
        Task RemoveRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null);
        /// <summary>
        ///     Removes the specified <paramref name="roles"/> from this user in the guild.
        /// </summary>
        /// <param name="roles">The roles to be removed from the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous role removal operation.
        /// </returns>
        Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null);
        /// <summary>
        ///     Sets a timeout based on provided <see cref="TimeSpan"/> to this user in the guild.
        /// </summary>
        /// <param name="span">The <see cref="TimeSpan"/> indicating how long a user should be timed out for.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous timeout creation operation.
        /// </returns>
        Task SetTimeOutAsync(TimeSpan span, RequestOptions options = null);
        /// <summary>
        ///     Removes the current timeout from the user in this guild if one exists.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous timeout removal operation.
        /// </returns>
        Task RemoveTimeOutAsync(RequestOptions options = null);
    }
}
