using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains audit log data related to a change in a guild member.
    /// </summary>
    public class MemberUpdateAuditLogData : IAuditLogData
    {
        private MemberUpdateAuditLogData(IUser target, string newNick, string oldNick)
        {
            Target = target;
            NewNick = newNick;
            OldNick = oldNick;
        }

        internal static MemberUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "nick");

            var newNick = changes.NewValue?.ToObject<string>();
            var oldNick = changes.OldValue?.ToObject<string>();

            var targetInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            var user = RestUser.Create(discord, targetInfo);

            return new MemberUpdateAuditLogData(user, newNick, oldNick);
        }

        /// <summary>
        ///     Gets the user that the changes were performed on.
        /// </summary>
        /// <returns>
        ///     A user object representing the user who the changes were performed on.
        /// </returns>
        public IUser Target { get; }
        /// <summary>
        ///     Gets the new nickname of the user.
        /// </summary>
        /// <returns>
        ///     A string containing the new nickname of the user; <c>null</c> if the user no longer has a nickname.
        /// </returns>
        public string NewNick { get; }
        /// <summary>
        ///     Gets the old nickname of the user.
        /// </summary>
        /// <returns>
        ///     A string containing the old nickname of the user; <c>null</c> if the user did not have a nickname.
        /// </returns>
        public string OldNick { get; }
    }
}
