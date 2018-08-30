using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

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

        internal static GuildUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var afkTimeoutModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");
            var defaultMessageNotificationsModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");
            var afkChannelModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");
            var regionIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");
            var iconHashModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");
            var verificationLevelModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");
            var ownerIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");
            var mfaLevelModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");
            var contentFilterModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");

            int? oldAfkTimeout = afkTimeoutModel?.OldValue?.ToObject<int>(discord.ApiClient.Serializer),
                newAfkTimeout = afkTimeoutModel?.NewValue?.ToObject<int>(discord.ApiClient.Serializer);
            DefaultMessageNotifications? oldDefaultMessageNotifications = defaultMessageNotificationsModel?.OldValue?.ToObject<DefaultMessageNotifications>(discord.ApiClient.Serializer),
                newDefaultMessageNotifications = defaultMessageNotificationsModel?.NewValue?.ToObject<DefaultMessageNotifications>(discord.ApiClient.Serializer);
            ulong? oldAfkChannelId = afkChannelModel?.OldValue?.ToObject<ulong>(discord.ApiClient.Serializer),
                newAfkChannelId = afkChannelModel?.NewValue?.ToObject<ulong>(discord.ApiClient.Serializer);
            string oldName = nameModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer),
                newName = nameModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
            string oldRegionId = regionIdModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer),
                newRegionId = regionIdModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
            string oldIconHash = iconHashModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer),
                newIconHash = iconHashModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
            VerificationLevel? oldVerificationLevel = verificationLevelModel?.OldValue?.ToObject<VerificationLevel>(discord.ApiClient.Serializer),
                newVerificationLevel = verificationLevelModel?.NewValue?.ToObject<VerificationLevel>(discord.ApiClient.Serializer);
            ulong? oldOwnerId = ownerIdModel?.OldValue?.ToObject<ulong>(discord.ApiClient.Serializer),
                newOwnerId = ownerIdModel?.NewValue?.ToObject<ulong>(discord.ApiClient.Serializer);
            MfaLevel? oldMfaLevel = mfaLevelModel?.OldValue?.ToObject<MfaLevel>(discord.ApiClient.Serializer),
                newMfaLevel = mfaLevelModel?.NewValue?.ToObject<MfaLevel>(discord.ApiClient.Serializer);
            int? oldContentFilter = contentFilterModel?.OldValue?.ToObject<int>(discord.ApiClient.Serializer),
                newContentFilter = contentFilterModel?.NewValue?.ToObject<int>(discord.ApiClient.Serializer);

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

            var before = new GuildInfo(oldAfkTimeout, oldDefaultMessageNotifications,
                oldAfkChannelId, oldName, oldRegionId, oldIconHash, oldVerificationLevel, oldOwner,
                oldMfaLevel, oldContentFilter);
            var after = new GuildInfo(newAfkTimeout, newDefaultMessageNotifications,
                newAfkChannelId, newName, newRegionId, newIconHash, newVerificationLevel, newOwner,
                newMfaLevel, newContentFilter);

            return new GuildUpdateAuditLogData(before, after);
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
