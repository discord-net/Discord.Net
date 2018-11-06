using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data for a permissions overwrite creation.
    /// </summary>
    public class OverwriteCreateAuditLogData : IAuditLogData
    {
        private OverwriteCreateAuditLogData(Overwrite overwrite)
        {
            Overwrite = overwrite;
        }

        internal static OverwriteCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var denyModel = changes.FirstOrDefault(x => x.ChangedProperty == "deny");
            var allowModel = changes.FirstOrDefault(x => x.ChangedProperty == "allow");

            var deny = denyModel.NewValue.ToObject<ulong>(discord.ApiClient.Serializer);
            var allow = allowModel.NewValue.ToObject<ulong>(discord.ApiClient.Serializer);

            var permissions = new OverwritePermissions(allow, deny);

            var id = entry.Options.OverwriteTargetId.Value;
            var type = entry.Options.OverwriteType;

            return new OverwriteCreateAuditLogData(new Overwrite(id, type, permissions));
        }

        /// <summary>
        ///     Gets the permission overwrite object that was created.
        /// </summary>
        /// <returns>
        ///     An <see cref="Overwrite"/> object representing the overwrite that was created.
        /// </returns>
        public Overwrite Overwrite { get; }
    }
}
