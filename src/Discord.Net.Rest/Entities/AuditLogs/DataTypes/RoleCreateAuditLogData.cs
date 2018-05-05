using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class RoleCreateAuditLogData : IAuditLogData
    {
        private RoleCreateAuditLogData(ulong id, RoleInfo props)
        {
            RoleId = id;
            Properties = props;
        }

        internal static RoleCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            Color? color = null;
            bool? mentionable = null,
                hoist = null;
            string name = null;
            GuildPermissions? permissions = null;

            foreach (var model in changes)
            {
                switch (model.ChangedProperty)
                {
                    case "color":
                        if (model.NewValue != null)
                            color = new Color(model.NewValue.ToObject<uint>());
                        break;
                    case "mentionable":
                        if (model.NewValue != null)
                            mentionable = model.NewValue.ToObject<bool>();
                        break;
                    case "hoist":
                        if (model.NewValue != null)
                            hoist = model.NewValue.ToObject<bool>();
                        break;
                    case "name":
                        if (model.NewValue != null)
                            name = model.NewValue.ToObject<string>();
                        break;
                    case "permissions":
                        if (model.NewValue != null)
                            permissions = new GuildPermissions(model.NewValue.ToObject<ulong>());
                        break;
                }
            }

            return new RoleCreateAuditLogData(entry.TargetId.Value,
                new RoleInfo(color, mentionable, hoist, name, permissions));
        }

        public ulong RoleId { get; }
        public RoleInfo Properties { get; }
    }
}
