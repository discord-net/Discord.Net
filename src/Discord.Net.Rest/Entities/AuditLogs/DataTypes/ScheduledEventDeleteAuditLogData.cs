using System;
using System.Linq;
using Discord.API;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a scheduled event deleteion.
    /// </summary>
    public class ScheduledEventDeleteAuditLogData : IAuditLogData
    {
        private ScheduledEventDeleteAuditLogData(ulong id)
        {
            Id = id;
        }

        internal static ScheduledEventDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var id = entry.TargetId.Value;

            return new ScheduledEventDeleteAuditLogData(id);
        }

        // Doc Note: Corresponds to the *current* data

        /// <summary>
        ///     Gets the snowflake id of the event.
        /// </summary>
        public ulong Id { get; }
    }
}
