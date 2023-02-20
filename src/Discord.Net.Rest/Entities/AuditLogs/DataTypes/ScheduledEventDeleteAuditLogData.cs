using Discord.API;
using System;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a scheduled event deletion.
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
