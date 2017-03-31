using System;
using System.Collections.Generic;

namespace Discord
{
    public interface IMessage : ISnowflakeEntity, IDeletable
    {
        /// <summary> Gets the type of this system message. </summary>
        MessageType Type { get; }
        /// <summary> Gets the source of this message. </summary>
        MessageSource Source { get; }
        /// <summary> Returns true if this message was sent as a text-to-speech message. </summary>
        bool IsTTS { get; }
        /// <summary> Returns true if this message was added to its channel's pinned messages. </summary>
        bool IsPinned { get; }
        /// <summary> Returns the content for this message. </summary>
        string Content { get; }
        /// <summary> Gets the time this message was sent. </summary>
        DateTimeOffset Timestamp { get; }
        /// <summary> Gets the time of this message's last edit, if any. </summary>
        DateTimeOffset? EditedTimestamp { get; }
        
        /// <summary> Gets the channel this message was sent to. </summary>
        IMessageChannel Channel { get; }
        /// <summary> Gets the author of this message. </summary>
        IUser Author { get; }

        /// <summary> Returns all attachments included in this message. </summary>
        IReadOnlyCollection<IAttachment> Attachments { get; }
        /// <summary> Returns all embeds included in this message. </summary>
        IReadOnlyCollection<IEmbed> Embeds { get; }
        /// <summary> Returns all tags included in this message's content. </summary>
        IReadOnlyCollection<ITag> Tags { get; }
        /// <summary> Returns the ids of channels mentioned in this message. </summary>
        IReadOnlyCollection<ulong> MentionedChannelIds { get; }
        /// <summary> Returns the ids of roles mentioned in this message. </summary>
        IReadOnlyCollection<ulong> MentionedRoleIds { get; }
        /// <summary> Returns the ids of users mentioned in this message. </summary>
        IReadOnlyCollection<ulong> MentionedUserIds { get; }
    }
}
