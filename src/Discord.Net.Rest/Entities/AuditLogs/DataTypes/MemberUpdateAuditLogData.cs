using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;
using ChangeModel = Discord.API.AuditLogChange;

namespace Discord.Rest
{
    public class MemberUpdateAuditLogData : IAuditLogData
    {
        private MemberUpdateAuditLogData(IUser user, string newNick, string oldNick)
        {
            User = user;
            NewNick = newNick;
            OldNick = oldNick;
        }

        internal static MemberUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "nick"); //TODO: only change?

            var newNick = changes.NewValue?.ToObject<string>();
            var oldNick = changes.OldValue?.ToObject<string>();

            var targetInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            var user = RestUser.Create(discord, targetInfo);

            return new MemberUpdateAuditLogData(user, newNick, oldNick);
        }

        public IUser User { get; }
        public string NewNick { get; }
        public string OldNick { get; }
    }
}
