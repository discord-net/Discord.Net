using System.Linq;

using EntryModel = Discord.API.AuditLogEntry;
using InfoModel = Discord.API.AuditLogs.GuildInfoAuditLogModel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Contains a piece of audit log data related to a guild update.
    /// </summary>
    public class GuildUpdateAuditLogData : IAuditLogData
    {
        private GuildUpdateAuditLogData(SocketGuildInfo before, SocketGuildInfo after)
        {
            Before = before;
            After = after;
        }

        internal static GuildUpdateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
        {
            var info = Rest.AuditLogHelper.CreateAuditLogEntityInfo<InfoModel>(entry.Changes, discord);

            var data = new GuildUpdateAuditLogData(SocketGuildInfo.Create(info.Item1), SocketGuildInfo.Create(info.Item2));
            return data;
        }

        /// <summary>
        ///     Gets the guild information before the changes.
        /// </summary>
        /// <returns>
        ///     An information object containing the original guild information before the changes were made.
        /// </returns>
        public SocketGuildInfo Before { get; }
        /// <summary>
        ///     Gets the guild information after the changes.
        /// </summary>
        /// <returns>
        ///     An information object containing the guild information after the changes were made.
        /// </returns>
        public SocketGuildInfo After { get; }
    }
}
