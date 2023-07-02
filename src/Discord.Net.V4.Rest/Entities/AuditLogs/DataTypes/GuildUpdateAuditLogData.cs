using Discord.API.AuditLogs;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a guild update.
    /// </summary>
    public class GuildUpdateAuditLogData : IAuditLogData
    {
        private GuildUpdateAuditLogData(GuildInfo before, GuildInfo after)
        {
            Before = before;
            After = after;
        }

        internal static GuildUpdateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
        {
            var changes = entry.Changes;
            
            var ownerIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "owner_id");

            ulong? oldOwnerId = ownerIdModel?.OldValue?.ToObject<ulong>(discord.ApiClient.Serializer),
                newOwnerId = ownerIdModel?.NewValue?.ToObject<ulong>(discord.ApiClient.Serializer);

            IUser oldOwner = null;
            if (oldOwnerId != null)
            {
                var oldOwnerInfo = log.Users.FirstOrDefault(x => x.Id == oldOwnerId.Value);
                oldOwner = RestUser.Create(discord, oldOwnerInfo);
            }

            IUser newOwner = null;
            if (newOwnerId != null)
            {
                var newOwnerInfo = log.Users.FirstOrDefault(x => x.Id == newOwnerId.Value);
                newOwner = RestUser.Create(discord, newOwnerInfo);
            }

            var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<GuildInfoAuditLogModel>(changes, discord);

            return new GuildUpdateAuditLogData(new(before, oldOwner), new(after, newOwner));
        }

        /// <summary>
        ///     Gets the guild information before the changes.
        /// </summary>
        /// <returns>
        ///     An information object containing the original guild information before the changes were made.
        /// </returns>
        public GuildInfo Before { get; }
        /// <summary>
        ///     Gets the guild information after the changes.
        /// </summary>
        /// <returns>
        ///     An information object containing the guild information after the changes were made.
        /// </returns>
        public GuildInfo After { get; }
    }
}
