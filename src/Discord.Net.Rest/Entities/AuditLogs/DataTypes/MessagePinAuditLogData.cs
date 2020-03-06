using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a pinned message.
    /// </summary>
    public class MessagePinAuditLogData : IAuditLogData
    {
        private MessagePinAuditLogData(ulong messageId, ulong channelId, IUser user)
        {
            MessageId = messageId;
            ChannelId = channelId;
            Target = user;
        }

        internal static MessagePinAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            return new MessagePinAuditLogData(entry.Options.MessageId.Value, entry.Options.ChannelId.Value, RestUser.Create(discord, userInfo));
        }

        /// <summary>
        ///     Gets the ID of the messages that was pinned.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the messages that was pinned.
        /// </returns>
        public ulong MessageId { get; }
        /// <summary>
        ///     Gets the ID of the channel that the message was pinned from.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the channel that the message was pinned from.
        /// </returns>
        public ulong ChannelId { get; }
        /// <summary>
        ///     Gets the user of the message that was pinned.
        /// </summary>
        /// <returns>
        ///     A user object representing the user that created the pinned message.
        /// </returns>
        public IUser Target { get; }
    }
}
