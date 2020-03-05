using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to an unpinned message.
    /// </summary>
    public class MessageUnpinAuditLogData : IAuditLogData
    {
        private MessageUnpinAuditLogData(ulong messageId, ulong channelId, ulong authorId)
        {
            MessageId = messageId;
            ChannelId = channelId;
            AuthorId = authorId;
        }

        internal static MessageUnpinAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            return new MessageUnpinAuditLogData(entry.Options.MessageId.Value, entry.Options.ChannelId.Value, entry.TargetId.Value);
        }

        /// <summary>
        ///     Gets the ID of the messages that was unpinned.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the messages that was unpinned.
        /// </returns>
        public ulong MessageId { get; }
        /// <summary>
        ///     Gets the ID of the channel that the message was unpinned from.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the channel that the message was unpinned from.
        /// </returns>
        public ulong ChannelId { get; }
        /// <summary>
        ///     Gets the author of the message that was unpinned.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the user whose message was unpinned.
        /// </returns>
        public ulong AuthorId { get; }
    }
}
