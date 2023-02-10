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

        internal static GuildUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var afkTimeoutModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_timeout");
            var defaultMessageNotificationsModel = changes.FirstOrDefault(x => x.ChangedProperty == "default_message_notifications");
            var afkChannelModel = changes.FirstOrDefault(x => x.ChangedProperty == "afk_channel_id");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var regionIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "region");
            var iconHashModel = changes.FirstOrDefault(x => x.ChangedProperty == "icon_hash");
            var verificationLevelModel = changes.FirstOrDefault(x => x.ChangedProperty == "verification_level");
            var ownerIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "owner_id");
            var mfaLevelModel = changes.FirstOrDefault(x => x.ChangedProperty == "mfa_level");
            var contentFilterModel = changes.FirstOrDefault(x => x.ChangedProperty == "explicit_content_filter");
            var systemChannelIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "system_channel_id");
            var widgetChannelIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "widget_channel_id");
            var widgetEnabledModel = changes.FirstOrDefault(x => x.ChangedProperty == "widget_enabled");

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
            ExplicitContentFilterLevel? oldContentFilter = contentFilterModel?.OldValue?.ToObject<ExplicitContentFilterLevel>(discord.ApiClient.Serializer),
                newContentFilter = contentFilterModel?.NewValue?.ToObject<ExplicitContentFilterLevel>(discord.ApiClient.Serializer);
            ulong? oldSystemChannelId = systemChannelIdModel?.OldValue?.ToObject<ulong>(discord.ApiClient.Serializer),
                newSystemChannelId = systemChannelIdModel?.NewValue?.ToObject<ulong>(discord.ApiClient.Serializer);
            ulong? oldWidgetChannelId = widgetChannelIdModel?.OldValue?.ToObject<ulong>(discord.ApiClient.Serializer),
                newWidgetChannelId = widgetChannelIdModel?.NewValue?.ToObject<ulong>(discord.ApiClient.Serializer);
            bool? oldWidgetEnabled = widgetEnabledModel?.OldValue?.ToObject<bool>(discord.ApiClient.Serializer),
                newWidgetEnabled = widgetEnabledModel?.NewValue?.ToObject<bool>(discord.ApiClient.Serializer);

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
                oldMfaLevel, oldContentFilter, oldSystemChannelId, oldWidgetChannelId, oldWidgetEnabled);
            var after = new GuildInfo(newAfkTimeout, newDefaultMessageNotifications,
                newAfkChannelId, newName, newRegionId, newIconHash, newVerificationLevel, newOwner,
                newMfaLevel, newContentFilter, newSystemChannelId, newWidgetChannelId, newWidgetEnabled);

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
