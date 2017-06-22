using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class RoleUpdateAuditLogData : IAuditLogData
    {
        private RoleUpdateAuditLogData(RoleProperties oldProps, RoleProperties newProps)
        {
            Before = oldProps;
            After = newProps;
        }

        internal static RoleUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var newProps = new RoleProperties();
            var oldProps = new RoleProperties();

            foreach (var model in changes)
            {
                switch (model.ChangedProperty)
                {
                    case "color":
                        if (model.NewValue != null)
                            newProps.Color = new Color(model.NewValue.ToObject<uint>());
                        if (model.OldValue != null)
                            oldProps.Color = new Color(model.OldValue.ToObject<uint>());
                        break;
                    case "mentionable":
                        if (model.NewValue != null)
                            newProps.Mentionable = model.NewValue.ToObject<bool>();
                        if (model.OldValue != null)
                            oldProps.Mentionable = model.OldValue.ToObject<bool>();
                        break;
                    case "hoist":
                        if (model.NewValue != null)
                            newProps.Hoist = model.NewValue.ToObject<bool>();
                        if (model.OldValue != null)
                            oldProps.Hoist = model.OldValue.ToObject<bool>();
                        break;
                    case "name":
                        if (model.NewValue != null)
                            newProps.Name = model.NewValue.ToObject<string>();
                        if (model.OldValue != null)
                            oldProps.Name = model.OldValue.ToObject<string>();
                        break;
                    case "permissions":
                        if (model.NewValue != null)
                            newProps.Permissions = new GuildPermissions(model.NewValue.ToObject<ulong>());
                        if (model.OldValue != null)
                            oldProps.Permissions = new GuildPermissions(model.OldValue.ToObject<ulong>());
                        break;
                }
            }

            return new RoleUpdateAuditLogData(oldProps, newProps);
        }

        //TODO: replace these with something read-only
        public RoleProperties Before { get; }
        public RoleProperties After { get; }
    }
}
