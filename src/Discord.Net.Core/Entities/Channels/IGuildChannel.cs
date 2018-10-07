using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic guild channel.
    /// </summary>
    /// <seealso cref="ITextChannel"/>
    /// <seealso cref="IVoiceChannel"/>
    /// <seealso cref="ICategoryChannel"/>
    public interface IGuildChannel : IChannel, IDeletable
    {
        /// <summary>
        ///     Gets the position of this channel.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the position of this channel in the guild's channel list relative to
        ///     others of the same type.
        /// </returns>
        int Position { get; }

        /// <summary>
        ///     Gets the guild associated with this channel.
        /// </summary>
        /// <returns>
        ///     A guild object that this channel belongs to.
        /// </returns>
        IGuild Guild { get; }
        /// <summary>
        ///     Gets the guild ID associated with this channel.
        /// </summary>
        /// <returns>
        ///     An <see cref="ulong"/> representing the guild snowflake identifier for the guild that this channel
        ///     belongs to.
        /// </returns>
        ulong GuildId { get; }
        /// <summary>
        ///     Gets a collection of permission overwrites for this channel.
        /// </summary>
        /// <returns>
        ///     A collection of overwrites associated with this channel.
        /// </returns>
        IReadOnlyCollection<Overwrite> PermissionOverwrites { get; }

        /// <summary>
        ///     Modifies this guild channel.
        /// </summary>
        /// <remarks>
        ///     This method modifies the current guild channel with the specified properties. To see an example of this
        ///     method and what properties are available, please refer to <see cref="GuildChannelProperties"/>.
        /// </remarks>
        /// <param name="func">The delegate containing the properties to modify the channel with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifyAsync(Action<GuildChannelProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Gets the permission overwrite for a specific role.
        /// </summary>
        /// <param name="role">The role to get the overwrite from.</param>
        /// <returns>
        ///     An overwrite object for the targeted role; <c>null</c> if none is set.
        /// </returns>
        OverwritePermissions? GetPermissionOverwrite(IRole role);
        /// <summary>
        ///     Gets the permission overwrite for a specific user.
        /// </summary>
        /// <param name="user">The user to get the overwrite from.</param>
        /// <returns>
        ///     An overwrite object for the targeted user; <c>null</c> if none is set.
        /// </returns>
        OverwritePermissions? GetPermissionOverwrite(IUser user);
        /// <summary>
        ///     Removes the permission overwrite for the given role, if one exists.
        /// </summary>
        /// <param name="role">The role to remove the overwrite from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task representing the asynchronous operation for removing the specified permissions from the channel.
        /// </returns>
        Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null);
        /// <summary>
        ///     Removes the permission overwrite for the given user, if one exists.
        /// </summary>
        /// <param name="user">The user to remove the overwrite from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task representing the asynchronous operation for removing the specified permissions from the channel.
        /// </returns>
        Task RemovePermissionOverwriteAsync(IUser user, RequestOptions options = null);

        /// <summary>
        ///     Adds or updates the permission overwrite for the given role.
        /// </summary>
        /// <example>
        ///     The following example fetches a role via <see cref="IGuild.GetRole"/> and a channel via 
        ///     <see cref="IGuild.GetChannelAsync"/>. Next, it checks if an overwrite had already been set via 
        ///     <see cref="GetPermissionOverwrite(Discord.IRole)"/>; if not, it denies the role from sending any
        ///     messages to the channel.
        ///     <code lang="cs">
        ///     // Fetches the role and channels
        ///     var role = guild.GetRole(339805618376540160);
        ///     var channel = await guild.GetChannelAsync(233937283911385098);
        /// 
        ///     // If either the of the object does not exist, bail
        ///     if (role == null || channel == null) return;
        ///
        ///     // Fetches the previous overwrite and bail if one is found
        ///     var previousOverwrite = channel.GetPermissionOverwrite(role);
        ///     if (previousOverwrite.HasValue) return;
        /// 
        ///     // Creates a new OverwritePermissions with send message set to deny and pass it into the method
        ///     await channel.AddPermissionOverwriteAsync(role,
        ///         new OverwritePermissions(sendMessage: PermValue.Deny));
        ///     </code>
        /// </example>
        /// <param name="role">The role to add the overwrite to.</param>
        /// <param name="permissions">The overwrite to add to the role.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task representing the asynchronous permission operation for adding the specified permissions to the
        ///     channel.
        /// </returns>
        Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions options = null);
        /// <summary>
        ///     Adds or updates the permission overwrite for the given user.
        /// </summary>
        /// <example>
        ///     The following example fetches a user via <see cref="IGuild.GetUserAsync"/> and a channel via 
        ///     <see cref="IGuild.GetChannelAsync"/>. Next, it checks if an overwrite had already been set via 
        ///     <see cref="GetPermissionOverwrite(Discord.IUser)"/>; if not, it denies the user from sending any
        ///     messages to the channel.
        ///     <code lang="cs">
        ///     // Fetches the role and channels
        ///     var user = await guild.GetUserAsync(168693960628371456);
        ///     var channel = await guild.GetChannelAsync(233937283911385098);
        /// 
        ///     // If either the of the object does not exist, bail
        ///     if (user == null || channel == null) return;
        ///
        ///     // Fetches the previous overwrite and bail if one is found
        ///     var previousOverwrite = channel.GetPermissionOverwrite(user);
        ///     if (previousOverwrite.HasValue) return;
        /// 
        ///     // Creates a new OverwritePermissions with send message set to deny and pass it into the method
        ///     await channel.AddPermissionOverwriteAsync(role,
        ///         new OverwritePermissions(sendMessage: PermValue.Deny));
        ///     </code>
        /// </example>
        /// <param name="user">The user to add the overwrite to.</param>
        /// <param name="permissions">The overwrite to add to the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task representing the asynchronous permission operation for adding the specified permissions to the channel.
        /// </returns>
        Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of users that are able to view the channel.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A paged collection containing a collection of guild users that can access this channel. Flattening the
        ///     paginated response into a collection of users with 
        ///     <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> is required if you wish to access the users.
        /// </returns>
        new IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a user in this channel.
        /// </summary>
        /// <param name="id">The snowflake identifier of the user.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task representing the asynchronous get operation. The task result contains a guild user object that
        ///     represents the user; <c>null</c> if none is found.
        /// </returns>
        new Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    }
}
