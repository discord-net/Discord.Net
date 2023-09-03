using System;

namespace Discord;

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
    ///     Gets the flags related to this channel.
    /// </summary>
    /// <remarks>
    ///     This value is determined by bitwise OR-ing <see cref="ChannelFlags"/> values together.
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
    IEntitySource<ulong, IGuild> Guild { get; }

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
    new IEntityEnumerableSource<ulong, IGuildUser> Users { get; }
}
