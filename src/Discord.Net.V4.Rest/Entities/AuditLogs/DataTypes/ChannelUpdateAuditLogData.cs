using Discord.API.AuditLogs;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a channel update.
    /// </summary>
    public class ChannelUpdateAuditLogData : IAuditLogData
    {
        private ChannelUpdateAuditLogData(ulong id, ChannelInfo before, ChannelInfo after)
        {
            ChannelId = id;
            Before = before;
            After = after;
        }

        internal static ChannelUpdateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
        {
            var changes = entry.Changes;

            var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<ChannelInfoAuditLogModel>(changes, discord);

            return new ChannelUpdateAuditLogData(entry.TargetId!.Value, new ChannelInfo(before), new ChannelInfo(after));
        }

        /// <summary>
        ///     Gets the snowflake ID of the updated channel.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the updated channel.
        /// </returns>
        public ulong ChannelId { get; }
        /// <summary>
        ///     Gets the channel information before the changes.
        /// </summary>
        /// <returns>
        ///     An information object containing the original channel information before the changes were made.
        /// </returns>
        public ChannelInfo Before { get; }
        /// <summary>
        ///     Gets the channel information after the changes.
        /// </summary>
        /// <returns>
        ///     An information object containing the channel information after the changes were made.
        /// </returns>
        public ChannelInfo After { get; }
    }
}
