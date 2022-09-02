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
            RestUser user = null;
            if (entry.TargetId.HasValue)
            {
                var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
                user = (userInfo != null) ? RestUser.Create(discord, userInfo) : null;
            }

            return new MessageUnpinAuditLogData(entry.Options.MessageId.Value, entry.Options.ChannelId.Value, user);
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
        ///     Gets the user of the message that was unpinned if available.
        /// </summary>
        /// <remarks>
        ///     Will be <see langword="null"/> if the user is a 'Deleted User#....' because Discord does send user data for deleted users.
        /// </remarks>
        /// <returns>
        ///     A user object representing the user that created the unpinned message or <see langword="null"/>.
        /// </returns>
        public IUser Target { get; }
    }
}
