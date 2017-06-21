using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;
using ChangeModel = Discord.API.AuditLogChange;

namespace Discord.Rest
{
    public class MemberUpdateChanges : IAuditLogChanges
    {
        private MemberUpdateChanges(IUser user, string newNick, string oldNick)
        {
            User = user;
            NewNick = newNick;
            OldNick = oldNick;
        }

        internal static MemberUpdateChanges Create(BaseDiscordClient discord, Model log, EntryModel entry, ChangeModel model)
        {
            var newNick = model.NewValue?.ToObject<string>();
            var oldNick = model.OldValue?.ToObject<string>();

            var targetInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            var user = RestUser.Create(discord, targetInfo);

            return new MemberUpdateChanges(user, newNick, oldNick);
        }

        public IUser User { get; }
        public string NewNick { get; }
        public string OldNick { get; }
    }
}
