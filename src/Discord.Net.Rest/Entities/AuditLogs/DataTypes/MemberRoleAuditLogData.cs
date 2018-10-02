using System.Collections.Generic;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a change in a guild member's roles.
    /// </summary>
    public class MemberRoleAuditLogData : IAuditLogData
    {
        private MemberRoleAuditLogData(IReadOnlyCollection<MemberRoleEditInfo> roles, IUser target)
        {
            Roles = roles;
            Target = target;
        }

        internal static MemberRoleAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var roleInfos = changes.SelectMany(x => x.NewValue.ToObject<API.Role[]>(discord.ApiClient.Serializer),
                (model, role) => new { model.ChangedProperty, Role = role })
                .Select(x => new MemberRoleEditInfo(x.Role.Name, x.Role.Id, x.ChangedProperty == "$add"))
                .ToList();

            var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            var user = RestUser.Create(discord, userInfo);

            return new MemberRoleAuditLogData(roleInfos.ToReadOnlyCollection(), user);
        }

        /// <summary>
        ///     Gets a collection of role changes that were performed on the member.
        /// </summary>
        /// <returns>
        ///     A read-only collection of <see cref="MemberRoleEditInfo"/>, containing the roles that were changed on
        ///     the member.
        /// </returns>
        public IReadOnlyCollection<MemberRoleEditInfo> Roles { get; }
        /// <summary>
        ///     Gets the user that the roles changes were performed on.
        /// </summary>
        /// <returns>
        ///     A user object representing the user that the role changes were performed on.
        /// </returns>
        public IUser Target { get; }
    }
}
