using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to message deletion(s).
    /// </summary>
    public class MessageDeleteAuditLogData : IAuditLogData
    {
        private MessageDeleteAuditLogData(ulong channelId, int count)
        {
            ChannelId = channelId;
            MessageCount = count;
        }

        internal static MessageDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            return new MessageDeleteAuditLogData(entry.Options.MessageDeleteChannelId.Value, entry.Options.MessageDeleteCount.Value);
        }

        /// <summary>
        ///     Gets the number of messages that were deleted.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the number of messages that were deleted from the channel.
        /// </returns>
        public int MessageCount { get; }
        /// <summary>
        ///     Gets the ID of the channel that the messages were deleted from.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the channel that the messages were
        ///     deleted from.
        /// </returns>
        public ulong ChannelId { get; }
    }
}
