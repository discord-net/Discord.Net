using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
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

            int? oldAfkTimeout = afkTimeoutModel?.OldValue?.ToObject<int>(),
                newAfkTimeout = afkTimeoutModel?.NewValue?.ToObject<int>();
            DefaultMessageNotifications? oldDefaultMessageNotifications = defaultMessageNotificationsModel?.OldValue?.ToObject<DefaultMessageNotifications>(),
                newDefaultMessageNotifications = defaultMessageNotificationsModel?.NewValue?.ToObject<DefaultMessageNotifications>();
            ulong? oldAfkChannelId = afkChannelModel?.OldValue?.ToObject<ulong>(),
                newAfkChannelId = afkChannelModel?.NewValue?.ToObject<ulong>();
            string oldName = nameModel?.OldValue?.ToObject<string>(),
                newName = nameModel?.NewValue?.ToObject<string>();
            string oldRegionId = regionIdModel?.OldValue?.ToObject<string>(),
                newRegionId = regionIdModel?.NewValue?.ToObject<string>();
            string oldIconHash = iconHashModel?.OldValue?.ToObject<string>(),
                newIconHash = iconHashModel?.NewValue?.ToObject<string>();
            VerificationLevel? oldVerificationLevel = verificationLevelModel?.OldValue?.ToObject<VerificationLevel>(),
                newVerificationLevel = verificationLevelModel?.NewValue?.ToObject<VerificationLevel>();
            ulong? oldOwnerId = ownerIdModel?.OldValue?.ToObject<ulong>(),
                newOwnerId = ownerIdModel?.NewValue?.ToObject<ulong>();
            MfaLevel? oldMfaLevel = mfaLevelModel?.OldValue?.ToObject<MfaLevel>(),
                newMfaLevel = mfaLevelModel?.NewValue?.ToObject<MfaLevel>();
            int? oldContentFilter = contentFilterModel?.OldValue?.ToObject<int>(),
                newContentFilter = contentFilterModel?.NewValue?.ToObject<int>();

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

        public GuildInfo Before { get; }
        public GuildInfo After { get; }
    }
}
