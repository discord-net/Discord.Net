using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a guild prune.
    /// </summary>
    public class PruneAuditLogData : IAuditLogData
    {
        private PruneAuditLogData(int pruneDays, int membersRemoved)
        {
            PruneDays = pruneDays;
            MembersRemoved = membersRemoved;
        }

        internal static PruneAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            return new PruneAuditLogData(entry.Options.PruneDeleteMemberDays.Value, entry.Options.PruneMembersRemoved.Value);
        }

        /// <summary>
        ///     Gets the threshold for a guild member to not be kicked.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the amount of days that a member must have been seen in the server,
        ///     to avoid being kicked. (i.e. If a user has not been seen for more than <paramref cref="PruneDays"/>, they will be
        ///     kicked from the server)
        /// </returns>
        public int PruneDays { get; }
        /// <summary>
        ///     Gets the number of members that were kicked during the purge.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the number of members that were removed from this guild for having
        ///     not been seen within <paramref cref="PruneDays"/>.
        /// </returns>
        public int MembersRemoved { get; }
    }
}
