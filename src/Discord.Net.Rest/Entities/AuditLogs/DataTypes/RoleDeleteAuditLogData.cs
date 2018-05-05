using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class RoleDeleteAuditLogData : IAuditLogData
    {
        private RoleDeleteAuditLogData(ulong id, RoleInfo props)
        {
            RoleId = id;
            Properties = props;
        }

        internal static RoleDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
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
                        if (model.OldValue != null)
                            color = new Color(model.OldValue.ToObject<uint>());
                        break;
                    case "mentionable":
                        if (model.OldValue != null)
                            mentionable = model.OldValue.ToObject<bool>();
                        break;
                    case "hoist":
                        if (model.OldValue != null)
                            hoist = model.OldValue.ToObject<bool>();
                        break;
                    case "name":
                        if (model.OldValue != null)
                            name = model.OldValue.ToObject<string>();
                        break;
                    case "permissions":
                        if (model.OldValue != null)
                            permissions = new GuildPermissions(model.OldValue.ToObject<ulong>());
                        break;
                }
            }

            return new RoleDeleteAuditLogData(entry.TargetId.Value,
                new RoleInfo(color, mentionable, hoist, name, permissions));
        }

        public ulong RoleId { get; }
        public RoleInfo Properties { get; }
    }
}
