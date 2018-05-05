using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class RoleUpdateAuditLogData : IAuditLogData
    {
        private RoleUpdateAuditLogData(ulong id, RoleInfo oldProps, RoleInfo newProps)
        {
            RoleId = id;
            Before = oldProps;
            After = newProps;
        }

        internal static RoleUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            Color? oldColor = null,
                newColor = null;
            bool? oldMentionable = null,
                newMentionable = null;
            bool? oldHoist = null,
                newHoist = null;
            string oldName = null,
                newName = null;
            GuildPermissions? oldPermissions = null,
                newPermissions = null;

            foreach (var model in changes)
            {
                switch (model.ChangedProperty)
                {
                    case "color":
                        if (model.NewValue != null)
                            newColor = new Color(model.NewValue.ToObject<uint>());
                        if (model.OldValue != null)
                            oldColor = new Color(model.OldValue.ToObject<uint>());
                        break;
                    case "mentionable":
                        if (model.NewValue != null)
                            newMentionable = model.NewValue.ToObject<bool>();
                        if (model.OldValue != null)
                            oldMentionable = model.OldValue.ToObject<bool>();
                        break;
                    case "hoist":
                        if (model.NewValue != null)
                            newHoist = model.NewValue.ToObject<bool>();
                        if (model.OldValue != null)
                            oldHoist = model.OldValue.ToObject<bool>();
                        break;
                    case "name":
                        if (model.NewValue != null)
                            newName = model.NewValue.ToObject<string>();
                        if (model.OldValue != null)
                            oldName = model.OldValue.ToObject<string>();
                        break;
                    case "permissions":
                        if (model.NewValue != null)
                            newPermissions = new GuildPermissions(model.NewValue.ToObject<ulong>());
                        if (model.OldValue != null)
                            oldPermissions = new GuildPermissions(model.OldValue.ToObject<ulong>());
                        break;
                }
            }

            var oldProps = new RoleInfo(oldColor, oldMentionable, oldHoist, oldName, oldPermissions);
            var newProps = new RoleInfo(newColor, newMentionable, newHoist, newName, newPermissions);

            return new RoleUpdateAuditLogData(entry.TargetId.Value, oldProps, newProps);
        }

        public ulong RoleId { get; }
        public RoleInfo Before { get; }
        public RoleInfo After { get; }
    }
}
