using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class InviteUpdateAuditLogData : IAuditLogData
    {
        private InviteUpdateAuditLogData(InviteInfo before, InviteInfo after)
        {
            Before = before;
            After = after;
        }

        internal static InviteUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var maxAgeModel = changes.FirstOrDefault(x => x.ChangedProperty == "max_age");
            var codeModel = changes.FirstOrDefault(x => x.ChangedProperty == "code");
            var temporaryModel = changes.FirstOrDefault(x => x.ChangedProperty == "temporary");
            var channelIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "channel_id");
            var maxUsesModel = changes.FirstOrDefault(x => x.ChangedProperty == "max_uses");

            int? oldMaxAge = maxAgeModel?.OldValue?.ToObject<int>(),
                newMaxAge = maxAgeModel?.NewValue?.ToObject<int>();
            string oldCode = codeModel?.OldValue?.ToObject<string>(),
                newCode = codeModel?.NewValue?.ToObject<string>();
            bool? oldTemporary = temporaryModel?.OldValue?.ToObject<bool>(),
                newTemporary = temporaryModel?.NewValue?.ToObject<bool>();
            ulong? oldChannelId = channelIdModel?.OldValue?.ToObject<ulong>(),
                newChannelId = channelIdModel?.NewValue?.ToObject<ulong>();
            int? oldMaxUses = maxUsesModel?.OldValue?.ToObject<int>(),
                newMaxUses = maxUsesModel?.NewValue?.ToObject<int>();

            var before = new InviteInfo(oldMaxAge, oldCode, oldTemporary, oldChannelId, oldMaxUses);
            var after = new InviteInfo(newMaxAge, newCode, newTemporary, newChannelId, newMaxUses);

            return new InviteUpdateAuditLogData(before, after);
        }

        public InviteInfo Before { get; }
        public InviteInfo After { get; }
    }
}
