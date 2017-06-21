using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;
using ChangeModel = Discord.API.AuditLogChange;
using OptionModel = Discord.API.AuditLogOptions;

namespace Discord.Rest
{
    public class OverwriteDeleteChanges : IAuditLogChanges
    {
        private OverwriteDeleteChanges(Overwrite deletedOverwrite)
        {
            Overwrite = deletedOverwrite;
        }

        internal static OverwriteDeleteChanges Create(BaseDiscordClient discord, Model log, EntryModel entry, ChangeModel[] models)
        {
            var denyModel = models.FirstOrDefault(x => x.ChangedProperty == "deny");
            var typeModel = models.FirstOrDefault(x => x.ChangedProperty == "type");
            var idModel = models.FirstOrDefault(x => x.ChangedProperty == "id");
            var allowModel = models.FirstOrDefault(x => x.ChangedProperty == "allow");

            var deny = denyModel.OldValue.ToObject<ulong>();
            var type = typeModel.OldValue.ToObject<string>(); //'role' or 'member', can't use PermissionsTarget :(
            var id = idModel.OldValue.ToObject<ulong>();
            var allow = allowModel.OldValue.ToObject<ulong>();

            PermissionTarget target;

            if (type == "member")
                target = PermissionTarget.User;
            else
                target = PermissionTarget.Role;

            return new OverwriteDeleteChanges(new Overwrite(id, target, new OverwritePermissions(allow, deny)));
        }

        public Overwrite Overwrite { get; }
    }
}
