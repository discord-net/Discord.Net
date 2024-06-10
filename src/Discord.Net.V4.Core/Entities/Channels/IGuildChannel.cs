using Discord.Models.Json;

namespace Discord;

public interface IGuildChannel : IChannel, IDeletable, IModifiable<ModifyGuildChannelProperties, ModifyGuildChannelParams>
{
    /// <summary>
    ///     Gets the position of this channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int" /> representing the position of this channel in the guild's channel list relative to
    ///     others of the same type.
    /// </returns>
    int Position { get; }

    /// <summary>
    ///     Gets the flags related to this channel.
    /// </summary>
    /// <remarks>
    ///     This value is determined by bitwise OR-ing <see cref="ChannelFlags" /> values together.
    /// </remarks>
    /// <returns>
    ///     A channel's flags, if any is associated.
    /// </returns>
    ChannelFlags Flags { get; }

    /// <summary>
    ///     Gets the guild associated with this channel.
    /// </summary>
    /// <returns>
    ///     A guild object that this channel belongs to.
    /// </returns>
    ILoadableEntity<ulong, IGuild> Guild { get; }

    /// <summary>
    ///     Gets a collection of permission overwrites for this channel.
    /// </summary>
    /// <returns>
    ///     A collection of overwrites associated with this channel.
    /// </returns>
    IReadOnlyCollection<Overwrite> PermissionOverwrites { get; }

    /// <summary>
    ///     Gets a collection of users that are able to view the channel or are currently in this channel.
    /// </summary>
    new ILoadableEntityEnumerable<ulong, IGuildUser> Users { get; }

    /// <summary>
    ///     Adds or updates the permission overwrite.
    /// </summary>
    /// <param name="overwrite">The overwrite to add.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task representing the asynchronous permission operation for adding the specified permissions to the
    ///     channel.
    /// </returns>
    Task AddPermissionOverwriteAsync(Overwrite overwrite, RequestOptions? options = null,
        CancellationToken token = default);

    /// <summary>
    ///     Removes the permission overwrite for the given entity, if one exists.
    /// </summary>
    /// <param name="targetId">The target ID of the overwrite to remove.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task representing the asynchronous operation for removing the specified permissions from the channel.
    /// </returns>
    Task RemovePermissionOverwriteAsync(ulong targetId, RequestOptions? options = null,
        CancellationToken token = default);
}
