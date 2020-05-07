using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to an unpinned message.
    /// </summary>
    public class MessageUnpinAuditLogData : IAuditLogData
    {
        private MessageUnpinAuditLogData(ulong messageId, ulong channelId, IUser user)
        {
            MessageId = messageId;
            ChannelId = channelId;
            Target = user;
        }

        internal static MessageUnpinAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            return new MessageUnpinAuditLogData(entry.Options.MessageId.Value, entry.Options.ChannelId.Value, RestUser.Create(discord, userInfo));
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
        ///     Gets the user of the message that was unpinned.
        /// </summary>
        /// <returns>
        ///     A user object representing the user that created the unpinned message.
        /// </returns>
        public IUser Target { get; }
    }
}
