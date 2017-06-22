using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class OverwriteUpdateAuditLogData : IAuditLogData
    {
        private OverwriteUpdateAuditLogData(OverwritePermissions before, OverwritePermissions after, ulong targetId, PermissionTarget targetType)
        {
            Before = before;
            After = after;
            OverwriteTargetId = targetId;
            OverwriteType = targetType;
        }

        internal static OverwriteUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var denyModel = changes.FirstOrDefault(x => x.ChangedProperty == "deny");
            var allowModel = changes.FirstOrDefault(x => x.ChangedProperty == "allow");

            var beforeAllow = allowModel?.OldValue?.ToObject<ulong>();
            var afterAllow = allowModel?.NewValue?.ToObject<ulong>();
            var beforeDeny = denyModel?.OldValue?.ToObject<ulong>();
            var afterDeny = denyModel?.OldValue?.ToObject<ulong>();

            var beforePermissions = new OverwritePermissions(beforeAllow ?? 0, beforeDeny ?? 0);
            var afterPermissions = new OverwritePermissions(afterAllow ?? 0, afterDeny ?? 0);

            PermissionTarget target;
            if (entry.Options.OverwriteType == "member")
                target = PermissionTarget.User;
            else
                target = PermissionTarget.Role;

            return new OverwriteUpdateAuditLogData(beforePermissions, afterPermissions, entry.Options.OverwriteTargetId.Value, target);
        }

        //TODO: this is kind of janky. Should I leave it, create a custom type, or what?
        public OverwritePermissions Before { get; }
        public OverwritePermissions After { get; }

        public ulong OverwriteTargetId { get; }
        public PermissionTarget OverwriteType { get; }
        //TODO: should we also include the role name if it is given?
    }
}
