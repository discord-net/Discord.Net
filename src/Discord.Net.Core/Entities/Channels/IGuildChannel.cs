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
        ///     The position of this channel in the guild's channel list, relative to others of the same type.
        /// </returns>
        int Position { get; }

        /// <summary>
        ///     Gets the guild associated with this channel.
        /// </summary>
        /// <returns>
        ///     A guild that this channel belongs to.
        /// </returns>
        IGuild Guild { get; }
        /// <summary>
        ///     Gets the guild ID associated with this channel.
        /// </summary>
        /// <returns>
        ///     A guild snowflake identifier for the guild that this channel belongs to.
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
        ///     Creates a new invite to this channel.
        /// </summary>
        /// <param name="maxAge">
        ///     The time (in seconds) until the invite expires. Set to <c>null</c> to never expire.
        /// </param>
        /// <param name="maxUses">
        ///     The max amount of times this invite may be used. Set to <c>null</c> to have unlimited uses.
        /// </param>
        /// <param name="isTemporary">
        ///     If <c>true</c>, a user accepting this invite will be kicked from the guild after closing their client.
        /// </param>
        /// <param name="isUnique">
        ///     If <c>true</c>, don't try to reuse a similar invite (useful for creating many unique one time use
        ///     invites).
        /// </param>
        /// <param name="options">
        ///     The options to be used when sending the request.
        /// </param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing an invite metadata object containing information for the
        ///     created invite. 
        /// </returns>
        Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null);
        /// <summary>
        ///     Returns a collection of all invites to this channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a read-only collection of invite metadata that are created
        ///     for this channel.
        /// </returns>
        Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null);

        /// <summary>
        ///     Modifies this guild channel.
        /// </summary>
        /// <param name="func">The properties to modify the channel with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task ModifyAsync(Action<GuildChannelProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Gets the permission overwrite for a specific role, or <c>null</c> if one does not exist.
        /// </summary>
        /// <param name="role">The role to get the overwrite from.</param>
        /// <returns>
        ///     An overwrite object for the targeted role; <c>null</c> if none is set.
        /// </returns>
        OverwritePermissions? GetPermissionOverwrite(IRole role);
        /// <summary>
        ///     Gets the permission overwrite for a specific user, or <c>null</c> if one does not exist.
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
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null);
        /// <summary>
        ///     Removes the permission overwrite for the given user, if one exists.
        /// </summary>
        /// <param name="user">The user to remove the overwrite from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task RemovePermissionOverwriteAsync(IUser user, RequestOptions options = null);

        /// <summary>
        ///     Adds or updates the permission overwrite for the given role.
        /// </summary>
        /// <param name="role">The role to add the overwrite to.</param>
        /// <param name="permissions">The overwrite to add to the role.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions options = null);
        /// <summary>
        ///     Adds or updates the permission overwrite for the given user.
        /// </summary>
        /// <param name="user">The user to add the overwrite to.</param>
        /// <param name="permissions">The overwrite to add to the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of all users in this channel.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A paged collection containing a collection of guild users that can access this channel. Flattening the
        ///     paginated response into a collection of users with 
        ///     <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> is required if you wish to access the users.
        /// </returns>
        new IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a user in this channel with the provided ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="mode">
        /// The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a guild user object that represents the user.
        /// </returns>
        new Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    }
}
