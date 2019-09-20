using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to the deletion of a permission overwrite.
    /// </summary>
    public class OverwriteDeleteAuditLogData : IAuditLogData
    {
        private OverwriteDeleteAuditLogData(ulong channelId, Overwrite deletedOverwrite)
        {
            ChannelId = channelId;
            Overwrite = deletedOverwrite;
        }

        internal static OverwriteDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var denyModel = changes.FirstOrDefault(x => x.ChangedProperty == "deny");
            var typeModel = changes.FirstOrDefault(x => x.ChangedProperty == "type");
            var idModel = changes.FirstOrDefault(x => x.ChangedProperty == "id");
            var allowModel = changes.FirstOrDefault(x => x.ChangedProperty == "allow");

            var deny = denyModel.OldValue.ToObject<ulong>(discord.ApiClient.Serializer);
            var type = typeModel.OldValue.ToObject<PermissionTarget>(discord.ApiClient.Serializer);
            var id = idModel.OldValue.ToObject<ulong>(discord.ApiClient.Serializer);
            var allow = allowModel.OldValue.ToObject<ulong>(discord.ApiClient.Serializer);

            return new OverwriteDeleteAuditLogData(entry.TargetId.Value, new Overwrite(id, type, new OverwritePermissions(allow, deny)));
        }

        /// <summary>
        ///     Gets the ID of the channel that the overwrite was deleted from.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the channel that the overwrite was
        ///     deleted from.
        /// </returns>
        public ulong ChannelId { get; }
        /// <summary>
        ///     Gets the permission overwrite object that was deleted.
        /// </summary>
        /// <returns>
        ///     An <see cref="Overwrite"/> object representing the overwrite that was deleted.
        /// </returns>
        public Overwrite Overwrite { get; }
    }
}
