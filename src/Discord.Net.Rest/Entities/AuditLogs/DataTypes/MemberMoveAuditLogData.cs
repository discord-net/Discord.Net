using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to moving members between voice channels.
    /// </summary>
    public class MemberMoveAuditLogData : IAuditLogData
    {
        private MemberMoveAuditLogData(ulong channelId, int count)
        {
            ChannelId = channelId;
            MemberCount = count;
        }

        internal static MemberMoveAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            return new MemberMoveAuditLogData(entry.Options.ChannelId.Value, entry.Options.Count.Value);
        }

        /// <summary>
        ///     Gets the ID of the channel that the members were moved to.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the channel that the members were moved to.
        /// </returns>
        public ulong ChannelId { get; }
        /// <summary>
        ///     Gets the number of members that were moved.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the number of members that were moved to another voice channel.
        /// </returns>
        public int MemberCount { get; }
    }
}
