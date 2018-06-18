using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class RoleUpdateAuditLogData : IAuditLogData
    {
        private RoleUpdateAuditLogData(ulong id, RoleEditInfo oldProps, RoleEditInfo newProps)
        {
            RoleId = id;
            Before = oldProps;
            After = newProps;
        }

        internal static RoleUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var colorModel = changes.FirstOrDefault(x => x.ChangedProperty == "color");
            var mentionableModel = changes.FirstOrDefault(x => x.ChangedProperty == "mentionable");
            var hoistModel = changes.FirstOrDefault(x => x.ChangedProperty == "hoist");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var permissionsModel = changes.FirstOrDefault(x => x.ChangedProperty == "permissions");

            uint? oldColorRaw = colorModel?.OldValue?.ToObject<uint>(),
                newColorRaw = colorModel?.NewValue?.ToObject<uint>();
            bool? oldMentionable = mentionableModel?.OldValue?.ToObject<bool>(),
                newMentionable = mentionableModel?.NewValue?.ToObject<bool>();
            bool? oldHoist = hoistModel?.OldValue?.ToObject<bool>(),
                newHoist = hoistModel?.NewValue?.ToObject<bool>();
            string oldName = nameModel?.OldValue?.ToObject<string>(),
                newName = nameModel?.NewValue?.ToObject<string>();
            ulong? oldPermissionsRaw = permissionsModel?.OldValue?.ToObject<ulong>(),
                newPermissionsRaw = permissionsModel?.OldValue?.ToObject<ulong>();

            Color? oldColor = null,
                newColor = null;
            GuildPermissions? oldPermissions = null,
                newPermissions = null;

            if (oldColorRaw.HasValue)
                oldColor = new Color(oldColorRaw.Value);
            if (newColorRaw.HasValue)
                newColor = new Color(newColorRaw.Value);
            if (oldPermissionsRaw.HasValue)
                oldPermissions = new GuildPermissions(oldPermissionsRaw.Value);
            if (newPermissionsRaw.HasValue)
                newPermissions = new GuildPermissions(newPermissionsRaw.Value);

            var oldProps = new RoleEditInfo(oldColor, oldMentionable, oldHoist, oldName, oldPermissions);
            var newProps = new RoleEditInfo(newColor, newMentionable, newHoist, newName, newPermissions);

            return new RoleUpdateAuditLogData(entry.TargetId.Value, oldProps, newProps);
        }

        public ulong RoleId { get; }
        public RoleEditInfo Before { get; }
        public RoleEditInfo After { get; }
    }
}
