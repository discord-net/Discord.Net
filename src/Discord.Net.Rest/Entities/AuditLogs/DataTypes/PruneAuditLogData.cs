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

        public int PruneDays { get; }
        public int MembersRemoved { get; }

        internal static PruneAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry) =>
            new PruneAuditLogData(entry.Options.PruneDeleteMemberDays.Value, entry.Options.PruneMembersRemoved.Value);
    }
}
