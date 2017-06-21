using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;
using ChangeModel = Discord.API.AuditLogChange;

namespace Discord.Rest
{
    public class MemberRoleChanges : IAuditLogChanges
    {
        private MemberRoleChanges(IReadOnlyCollection<RoleInfo> roles, IUser target)
        {
            Roles = roles;
            TargetUser = target;
        }

        internal static MemberRoleChanges Create(BaseDiscordClient discord, Model log, EntryModel entry, ChangeModel[] models)
        {
            var roleInfos = models.SelectMany(x => x.NewValue.ToObject<API.Role[]>(),
                (model, role) => new { model.ChangedProperty, Role = role })
                .Select(x => new RoleInfo(x.Role.Name, x.Role.Id, x.ChangedProperty == "$add"))
                .ToList();

            var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            var user = RestUser.Create(discord, userInfo);

            return new MemberRoleChanges(roleInfos.ToReadOnlyCollection(), user);
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
