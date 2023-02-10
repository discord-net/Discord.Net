using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to an unban.
    /// </summary>
    public class UnbanAuditLogData : IAuditLogData
    {
        private UnbanAuditLogData(IUser user)
        {
            Target = user;
        }

        internal static UnbanAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            return new UnbanAuditLogData((userInfo != null) ? RestUser.Create(discord, userInfo) : null);
        }

        /// <summary>
        ///     Gets the user that was unbanned.
        /// </summary>
        /// <returns>
        ///     A user object representing the user that was unbanned.
        /// </returns>
        public IUser Target { get; }
    }
}
