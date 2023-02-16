using System.Linq;

using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Contains a piece of audit log data related to a guild update.
    /// </summary>
    public class GuildUpdateAuditLogData : IAuditLogData
    {
        private GuildUpdateAuditLogData(GuildInfo before, GuildInfo after)
        {
            Before = before;
            After = after;
        }

        internal static GuildUpdateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
        {
            GuildInfo before = new(entry.Changes, true), after = new(entry.Changes, false);

            return new(before, after);
        }

        /// <summary>
        ///     Gets the guild information before the changes.
        /// </summary>
        /// <returns>
        ///     An information object containing the original guild information before the changes were made.
        /// </returns>
        public GuildInfo Before { get; }
        /// <summary>
        ///     Gets the guild information after the changes.
        /// </summary>
        /// <returns>
        ///     An information object containing the guild information after the changes were made.
        /// </returns>
        public GuildInfo After { get; }
    }
}
