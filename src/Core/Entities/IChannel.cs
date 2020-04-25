using System;

namespace Discord.Core
{
    /// <summary>
    /// An interface representing the collection of operations which can be
    /// performed on a channel.
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// Gets a value indicating the unique identifier for this channel.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        /// Gets a value indicating the type of the current channel.
        /// </summary>
        ChannelType Type { get; }

        /// <summary>
        /// Gets a value indicating the unique identifier of the guild this
        /// channel belongs to.
        /// </summary>
        ulong? GuildId { get; }

        /// <summary>
        /// Gets a value indicating the sorting position of the channel, or
        /// <code>null</code> if one is not present.
        /// </summary>
        int? Position { get; }

        // TODO: permission overwrites

        /// <summary>
        /// Gets a value indicating the name of this channel, or
        /// <code>null</code> if one is not present.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Gets a value indicating the topic of this channel, or
        /// <code>null</code> if one is not present.
        /// </summary>
        string? Topic { get; }

        /// <summary>
        /// Gets a value indicating the last message sent in this channel, or
        /// <code>null</code> if one is not present.
        /// </summary>
        /// <remarks>
        /// This identifier may point to a non-existent or invalid message.
        /// </remarks>
        ulong? LastMessageId { get; }

        /// <summary>
        /// Gets a value indicating the bitrate, in bits per second, of the
        /// voice channel, or <code>null</code> if one is not present.
        /// </summary>
        int? Bitrate { get; }

        /// <summary>
        /// Gets a value indicating the user limit of the voice channel, or
        /// <code>null</code> if one is not present.
        /// </summary>
        int? UserLimit { get; }

        /// <summary>
        /// Gets a value indicating the rate limit per user of the text
        /// channel, or <code>null</code> if one is not present.
        /// </summary>
        /// <remarks>
        /// If the current user has the Manage Messages permission or
        /// Manage Channel permission, this rate limit takes no effect.
        /// </remarks>
        int? RateLimit { get; }

        // TODO: recipients

        /// <summary>
        /// Gets a value indicating the icon of the text channel, or
        /// <code>null</code> if one is not present.
        /// </summary>
        string? Icon { get; }

        /// <summary>
        /// Gets a value indicating the unique identifier of the owner this
        /// DM channel belongs to, or <code>null</code> if one is not present.
        /// </summary>
        ulong? OwnerId { get; }

        /// <summary>
        /// Gets a value indicating the unique identifier of the owner this
        /// DM channel belongs to, or <code>null</code> if one is not present.
        /// </summary>
        ulong? ApplicationId { get; }

        /// <summary>
        /// Gets a value indicating the unique identifier of the parent
        /// category this channel belongs to, or <code>null</code> if one is
        /// not present.
        /// </summary>
        /// <remarks>
        /// Each category channel can contain 50 child channels.
        /// </remarks>
        ulong? ParentId { get; }

        /// <summary>
        /// Gets a value indicating the time when the last pinned message was
        /// pinned, or <code>null</code> if one is not present.
        /// </summary>
        DateTimeOffset? LastPinTimestamp { get; }
    }
}
