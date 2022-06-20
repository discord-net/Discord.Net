using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a ban.
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
            return new BanAuditLogData((userInfo != null) ? RestUser.Create(discord, userInfo) : null);
        }

        /// <summary>
        ///     Gets the user that was banned.
        /// </summary>
        /// <remarks>
        ///     Will be <see langword="null"/> if the user is a 'Deleted User#....' because Discord does send user data for deleted users.
        /// </remarks>
        /// <returns>
        ///     A user object representing the banned user.
        /// </returns>
        public IUser Target { get; }
    }
}
