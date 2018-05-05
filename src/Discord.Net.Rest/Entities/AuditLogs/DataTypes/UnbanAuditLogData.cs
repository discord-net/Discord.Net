using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class UnbanAuditLogData : IAuditLogData
    {
        private UnbanAuditLogData(IUser user)
        {
            Target = user;
        }

        internal static UnbanAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            return new UnbanAuditLogData(RestUser.Create(discord, userInfo));
        }

        public IUser Target { get; }
    }
}
