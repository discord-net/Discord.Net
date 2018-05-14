using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class KickAuditLogData : IAuditLogData
    {
        private KickAuditLogData(RestUser user)
        {
            Target = user;
        }

        internal static KickAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            return new KickAuditLogData(RestUser.Create(discord, userInfo));
        }

        public IUser Target { get; }
    }
}
