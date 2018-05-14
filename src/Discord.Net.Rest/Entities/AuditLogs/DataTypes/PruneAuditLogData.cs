using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
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

        public int PruneDays { get; }
        public int MembersRemoved { get; }
    }
}
