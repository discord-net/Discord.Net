using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    /// Represents an audit log data for a ban action.
    /// </summary>
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

        /// <summary>
        ///     Gets the user that was banned.
        /// </summary>
        public IUser Target { get; }
    }
}
