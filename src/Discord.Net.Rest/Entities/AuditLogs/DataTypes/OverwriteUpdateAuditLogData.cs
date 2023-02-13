using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to the update of a permission overwrite.
    /// </summary>
    public class OverwriteUpdateAuditLogData : IAuditLogData
    {
        private OverwriteUpdateAuditLogData(ulong channelId, OverwritePermissions before, OverwritePermissions after, ulong targetId, PermissionTarget targetType)
        {
            ChannelId = channelId;
            OldPermissions = before;
            NewPermissions = after;
            OverwriteTargetId = targetId;
            OverwriteType = targetType;
        }

        internal static OverwriteUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var denyModel = changes.FirstOrDefault(x => x.ChangedProperty == "deny");
            var allowModel = changes.FirstOrDefault(x => x.ChangedProperty == "allow");

            var beforeAllow = allowModel?.OldValue?.ToObject<ulong>(discord.ApiClient.Serializer);
            var afterAllow = allowModel?.NewValue?.ToObject<ulong>(discord.ApiClient.Serializer);
            var beforeDeny = denyModel?.OldValue?.ToObject<ulong>(discord.ApiClient.Serializer);
            var afterDeny = denyModel?.NewValue?.ToObject<ulong>(discord.ApiClient.Serializer);

            var beforePermissions = new OverwritePermissions(beforeAllow ?? 0, beforeDeny ?? 0);
            var afterPermissions = new OverwritePermissions(afterAllow ?? 0, afterDeny ?? 0);

            var type = entry.Options.OverwriteType;

            return new OverwriteUpdateAuditLogData(entry.TargetId.Value, beforePermissions, afterPermissions, entry.Options.OverwriteTargetId.Value, type);
        }

        /// <summary>
        ///     Gets the ID of the channel that the overwrite was updated from.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the channel that the overwrite was
        ///     updated from.
        /// </returns>
        public ulong ChannelId { get; }
        /// <summary>
        ///     Gets the overwrite permissions before the changes.
        /// </summary>
        /// <returns>
        ///     An overwrite permissions object representing the overwrite permissions that the overwrite had before
        ///     the changes were made.
        /// </returns>
        public OverwritePermissions OldPermissions { get; }
        /// <summary>
        ///     Gets the overwrite permissions after the changes.
        /// </summary>
        /// <returns>
        ///     An overwrite permissions object representing the overwrite permissions that the overwrite had after the
        ///     changes.
        /// </returns>
        public OverwritePermissions NewPermissions { get; }
        /// <summary>
        ///     Gets the ID of the overwrite that was updated.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the overwrite that was updated.
        /// </returns>
        public ulong OverwriteTargetId { get; }
        /// <summary>
        ///     Gets the target of the updated permission overwrite.
        /// </summary>
        /// <returns>
        ///     The target of the updated permission overwrite.
        /// </returns>
        public PermissionTarget OverwriteType { get; }
    }
}
