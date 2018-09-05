using System;
using System.Collections.Generic;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
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

        public IReadOnlyCollection<MemberRoleEditInfo> Roles { get; }
        public IUser Target { get; }
    }
}
