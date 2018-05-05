using System;
using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a message object.
    /// </summary>
    public interface IMessage : ISnowflakeEntity, IDeletable
    {
        /// <summary>
        ///     Gets the type of this system message.
        /// </summary>
        MessageType Type { get; }
        /// <summary>
        ///     Gets the source type of this message.
        /// </summary>
        MessageSource Source { get; }
        /// <summary>
        ///     Returns <see langword="true"/> if this message was sent as a text-to-speech message.
        /// </summary>
        bool IsTTS { get; }
        /// <summary>
        ///     Returns <see langword="true"/> if this message was added to its channel's pinned messages.
        /// </summary>
        bool IsPinned { get; }
        /// <summary>
        ///     Returns the content for this message.
        /// </summary>
        string Content { get; }
        /// <summary>
        ///     Gets the time this message was sent.
        /// </summary>
        /// <returns>
        ///     Time of when the message was sent.
        /// </returns>
        DateTimeOffset Timestamp { get; }
        /// <summary>
        ///     Gets the time of this message's last edit.
        /// </summary>
        /// <returns>
        ///     Time of when the message was last edited; <see langword="null"/> when the message is never edited.
        /// </returns>
        DateTimeOffset? EditedTimestamp { get; }
        
        /// <summary>
        ///     Gets the source channel of the message.
        /// </summary>
        IMessageChannel Channel { get; }
        /// <summary>
        ///     Gets the author of this message.
        /// </summary>
        IUser Author { get; }

        /// <summary>
        ///     Returns all attachments included in this message.
        /// </summary>
        /// <returns>
        ///     Collection of attachments.
        /// </returns>
        IReadOnlyCollection<IAttachment> Attachments { get; }
        /// <summary>
        ///     Returns all embeds included in this message.
        /// </summary>
        /// <returns>
        ///     Collection of embed objects.
        /// </returns>
        IReadOnlyCollection<IEmbed> Embeds { get; }
        /// <summary>
        ///     Returns all tags included in this message's content.
        /// </summary>
        IReadOnlyCollection<ITag> Tags { get; }
        /// <summary>
        ///     Returns the IDs of channels mentioned in this message.
        /// </summary>
        /// <returns>
        ///     Collection of channel IDs.
        /// </returns>
        IReadOnlyCollection<ulong> MentionedChannelIds { get; }
        /// <summary>
        ///     Returns the IDs of roles mentioned in this message.
        /// </summary>
        /// <returns>
        ///     Collection of role IDs.
        /// </returns>
        IReadOnlyCollection<ulong> MentionedRoleIds { get; }
        /// <summary>
        ///     Returns the IDs of users mentioned in this message.
        /// </summary>
        /// <returns>
        ///     Collection of user IDs.
        /// </returns>
        IReadOnlyCollection<ulong> MentionedUserIds { get; }
    }
}
