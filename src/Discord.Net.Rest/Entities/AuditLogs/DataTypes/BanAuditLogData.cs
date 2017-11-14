using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class BanAuditLogData : IAuditLogData
    {
        private BanAuditLogData(IUser user)
        {
            Target = user;
        }

        internal static BanAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            return new BanAuditLogData(RestUser.Create(discord, userInfo));
        }

        public IUser Target { get; }
    }
}
