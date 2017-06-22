using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class RoleCreateAuditLogData : IAuditLogData
    {
        private RoleCreateAuditLogData(RoleProperties newProps)
        {
            Properties = newProps;
        }

        internal static RoleCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var newProps = new RoleProperties();

            foreach (var model in changes)
            {
                switch (model.ChangedProperty)
                {
                    case "color":
                        if (model.NewValue != null)
                            newProps.Color = new Color(model.NewValue.ToObject<uint>());
                        break;
                    case "mentionable":
                        if (model.NewValue != null)
                            newProps.Mentionable = model.NewValue.ToObject<bool>();
                        break;
                    case "hoist":
                        if (model.NewValue != null)
                            newProps.Hoist = model.NewValue.ToObject<bool>();
                        break;
                    case "name":
                        if (model.NewValue != null)
                            newProps.Name = model.NewValue.ToObject<string>();
                        break;
                    case "permissions":
                        if (model.NewValue != null)
                            newProps.Permissions = new GuildPermissions(model.NewValue.ToObject<ulong>());
                        break;
                }
            }

            return new RoleCreateAuditLogData(newProps);
        }

        //TODO: replace this with something read-only
        public RoleProperties Properties { get; }
    }
}
