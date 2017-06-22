using System;
using System.Collections.Generic;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class MemberRoleAuditLogData : IAuditLogData
    {
        private MemberRoleAuditLogData(IReadOnlyCollection<RoleInfo> roles, IUser target)
        {
            Roles = roles;
            TargetUser = target;
        }

        internal static MemberRoleAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var roleInfos = changes.SelectMany(x => x.NewValue.ToObject<API.Role[]>(),
                (model, role) => new { model.ChangedProperty, Role = role })
                .Select(x => new RoleInfo(x.Role.Name, x.Role.Id, x.ChangedProperty == "$add"))
                .ToList();

            var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            var user = RestUser.Create(discord, userInfo);

            return new MemberRoleAuditLogData(roleInfos.ToReadOnlyCollection(), user);
        }

        public IReadOnlyCollection<RoleInfo> Roles { get; }
        public IUser TargetUser { get; }

        public struct RoleInfo
        {
            internal RoleInfo(string name, ulong roleId, bool added)
            {
                Name = name;
                RoleId = roleId;
                Added = added;
            }

            string Name { get; }
            ulong RoleId { get; }
            bool Added { get; }
        }
    }
}
