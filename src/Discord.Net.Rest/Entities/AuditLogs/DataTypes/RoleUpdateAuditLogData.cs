using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a role update.
    /// </summary>
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

            uint? oldColorRaw = colorModel?.OldValue?.ToObject<uint>(discord.ApiClient.Serializer),
                newColorRaw = colorModel?.NewValue?.ToObject<uint>(discord.ApiClient.Serializer);
            bool? oldMentionable = mentionableModel?.OldValue?.ToObject<bool>(discord.ApiClient.Serializer),
                newMentionable = mentionableModel?.NewValue?.ToObject<bool>(discord.ApiClient.Serializer);
            bool? oldHoist = hoistModel?.OldValue?.ToObject<bool>(discord.ApiClient.Serializer),
                newHoist = hoistModel?.NewValue?.ToObject<bool>(discord.ApiClient.Serializer);
            string oldName = nameModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer),
                newName = nameModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
            ulong? oldPermissionsRaw = permissionsModel?.OldValue?.ToObject<ulong>(discord.ApiClient.Serializer),
                newPermissionsRaw = permissionsModel?.OldValue?.ToObject<ulong>(discord.ApiClient.Serializer);

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

        /// <summary>
        ///     Gets the ID of the role that was changed.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the role that was changed.
        /// </returns>
        public ulong RoleId { get; }
        /// <summary>
        ///     Gets the role information before the changes.
        /// </summary>
        /// <returns>
        ///     A role information object containing the role information before the changes were made.
        /// </returns>
        public RoleEditInfo Before { get; }
        /// <summary>
        ///     Gets the role information after the changes.
        /// </summary>
        /// <returns>
        ///     A role information object containing the role information after the changes were made.
        /// </returns>
        public RoleEditInfo After { get; }
    }
}
