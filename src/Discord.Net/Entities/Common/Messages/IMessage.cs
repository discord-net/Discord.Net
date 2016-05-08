using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.API.Rest;

namespace Discord
{
    public interface IMessage : IDeletable, ISnowflakeEntity
    {
        /// <summary> Gets the time of this message's last edit, if any. </summary>
        DateTime? EditedTimestamp { get; }
        /// <summary> Returns true if this message originated from the logged-in account. </summary>
        bool IsAuthor { get; }
        /// <summary> Returns true if this message was sent as a text-to-speech message. </summary>
        bool IsTTS { get; }
        /// <summary> Returns the original, unprocessed text for this message. </summary>
        string RawText { get; }
        /// <summary> Returns the text for this message after mention processing. </summary>
        string Text { get; }
        /// <summary> Gets the time this message was sent. </summary>
        DateTime Timestamp { get; } //TODO: Is this different from IHasSnowflake.CreatedAt?

        /// <summary> Gets the channel this message was sent to. </summary>
        IMessageChannel Channel { get; }
        /// <summary> Gets the author of this message. </summary>
        IUser Author { get; }

        /// <summary> Returns a collection of all attachments included in this message. </summary>
        IReadOnlyList<Attachment> Attachments { get; }
        /// <summary> Returns a collection of all embeds included in this message. </summary>
        IReadOnlyList<Embed> Embeds { get; }
        /// <summary> Returns a collection of channel ids mentioned in this message. </summary>
        IReadOnlyList<ulong> MentionedChannelIds { get; }
        /// <summary> Returns a collection of role ids mentioned in this message. </summary>
        IReadOnlyList<ulong> MentionedRoleIds { get; }
        /// <summary> Returns a collection of user ids mentioned in this message. </summary>
        IReadOnlyList<IUser> MentionedUsers { get; }

        /// <summary> Modifies this message. </summary>
        Task Modify(Action<ModifyMessageParams> func);
    }
}