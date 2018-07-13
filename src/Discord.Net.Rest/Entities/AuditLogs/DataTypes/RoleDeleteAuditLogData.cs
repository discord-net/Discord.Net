using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data relating to a role deletion.
    /// </summary>
    public class RoleDeleteAuditLogData : IAuditLogData
    {
        private RoleDeleteAuditLogData(ulong id, RoleEditInfo props)
        {
            RoleId = id;
            Properties = props;
        }

        internal static RoleDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var colorModel = changes.FirstOrDefault(x => x.ChangedProperty == "color");
            var mentionableModel = changes.FirstOrDefault(x => x.ChangedProperty == "mentionable");
            var hoistModel = changes.FirstOrDefault(x => x.ChangedProperty == "hoist");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var permissionsModel = changes.FirstOrDefault(x => x.ChangedProperty == "permissions");

            uint? colorRaw = colorModel?.OldValue?.ToObject<uint>();
            bool? mentionable = mentionableModel?.OldValue?.ToObject<bool>();
            bool? hoist = hoistModel?.OldValue?.ToObject<bool>();
            string name = nameModel?.OldValue?.ToObject<string>();
            ulong? permissionsRaw = permissionsModel?.OldValue?.ToObject<ulong>();

            Color? color = null;
            GuildPermissions? permissions = null;

            if (colorRaw.HasValue)
                color = new Color(colorRaw.Value);
            if (permissionsRaw.HasValue)
                permissions = new GuildPermissions(permissionsRaw.Value);

            return new RoleDeleteAuditLogData(entry.TargetId.Value,
                new RoleEditInfo(color, mentionable, hoist, name, permissions));
        }

        /// <summary>
        ///     Gets the ID of the role that was deleted.
        /// </summary>
        /// <return>
        ///     A <see cref="ulong"/> representing the snowflake identifer to the role that was deleted.
        /// </return>
        public ulong RoleId { get; }
        /// <summary>
        ///     Gets the role information that was deleted.
        /// </summary>
        /// <return>
        ///     An information object representing the properties of the role that was deleted.
        /// </return>
        public RoleEditInfo Properties { get; }
    }
}
