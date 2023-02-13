using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

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
            var allowModel = changes.FirstOrDefault(x => x.ChangedProperty == "allow");

            var deny = denyModel.OldValue.ToObject<ulong>(discord.ApiClient.Serializer);
            var allow = allowModel.OldValue.ToObject<ulong>(discord.ApiClient.Serializer);

            var permissions = new OverwritePermissions(allow, deny);

            var id = entry.Options.OverwriteTargetId.Value;
            var type = entry.Options.OverwriteType;

            return new OverwriteDeleteAuditLogData(entry.TargetId.Value, new Overwrite(id, type, permissions));
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
