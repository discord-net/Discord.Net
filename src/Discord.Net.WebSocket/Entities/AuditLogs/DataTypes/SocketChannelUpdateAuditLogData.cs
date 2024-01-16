using Discord.Rest;
using Discord.API.AuditLogs;

using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Contains a piece of audit log data related to a channel update.
    /// </summary>
    public class SocketChannelUpdateAuditLogData : ISocketAuditLogData
    {
        private SocketChannelUpdateAuditLogData(ulong id, SocketChannelInfo before, SocketChannelInfo after)
        {
            ChannelId = id;
            Before = before;
            After = after;
        }

        internal static SocketChannelUpdateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
        {
            var changes = entry.Changes;

            var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<ChannelInfoAuditLogModel>(changes, discord);

            return new SocketChannelUpdateAuditLogData(entry.TargetId!.Value, new(before), new(after));
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
        public SocketChannelInfo Before { get; }

        /// <summary>
        ///     Gets the channel information after the changes.
        /// </summary>
        /// <returns>
        ///     An information object containing the channel information after the changes were made.
        /// </returns>
        public SocketChannelInfo After { get; }
    }
}
