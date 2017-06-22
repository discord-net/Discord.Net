using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class RoleDeleteAuditLogData : IAuditLogData
    {
        private RoleDeleteAuditLogData(RoleProperties properties)
        {
            Properties = properties;
        }

        internal static RoleDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var oldProps = new RoleProperties();

            foreach (var model in changes)
            {
                switch (model.ChangedProperty)
                {
                    case "color":
                        if (model.OldValue != null)
                            oldProps.Color = new Color(model.OldValue.ToObject<uint>());
                        break;
                    case "mentionable":
                        if (model.OldValue != null)
                            oldProps.Mentionable = model.OldValue.ToObject<bool>();
                        break;
                    case "hoist":
                        if (model.OldValue != null)
                            oldProps.Hoist = model.OldValue.ToObject<bool>();
                        break;
                    case "name":
                        if (model.OldValue != null)
                            oldProps.Name = model.OldValue.ToObject<string>();
                        break;
                    case "permissions":
                        if (model.OldValue != null)
                            oldProps.Permissions = new GuildPermissions(model.OldValue.ToObject<ulong>());
                        break;
                }
            }

            return new RoleDeleteAuditLogData(oldProps);
        }

        //TODO: replace this with something read-only
        public RoleProperties Properties { get; }
    }
}
