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

            int? oldAfkTimeout = null,
                newAfkTimeout = null;
            DefaultMessageNotifications? oldDefaultMessageNotifications = null,
                newDefaultMessageNotifications = null;
            ulong? oldAfkChannelId = null,
                newAfkChannelId = null;
            string oldName = null,
                newName = null;
            string oldRegionId = null,
                newRegionId = null;
            string oldIconHash = null,
                newIconHash = null;
            VerificationLevel? oldVerificationLevel = null,
                newVerificationLevel = null;
            ulong? oldOwnerId = null,
                newOwnerId = null;
            MfaLevel? oldMfaLevel = null,
                newMfaLevel = null;
            int? oldContentFilter = null,
                newContentFilter = null;

            foreach (var change in changes)
            {
                switch (change.ChangedProperty)
                {
                    case "afk_timeout":
                        oldAfkTimeout = change.OldValue?.ToObject<int>();
                        newAfkTimeout = change.NewValue?.ToObject<int>();
                        break;
                    case "default_message_notifications":
                        oldDefaultMessageNotifications = change.OldValue?.ToObject<DefaultMessageNotifications>();
                        newDefaultMessageNotifications = change.OldValue?.ToObject<DefaultMessageNotifications>();
                        break;
                    case "afk_channel_id":
                        oldAfkChannelId = change.OldValue?.ToObject<ulong>();
                        newAfkChannelId = change.NewValue?.ToObject<ulong>();
                        break;
                    case "name":
                        oldName = change.OldValue?.ToObject<string>();
                        newName = change.NewValue?.ToObject<string>();
                        break;
                    case "region":
                        oldRegionId = change.OldValue?.ToObject<string>();
                        newRegionId = change.NewValue?.ToObject<string>();
                        break;
                    case "icon_hash":
                        oldIconHash = change.OldValue?.ToObject<string>();
                        newIconHash = change.NewValue?.ToObject<string>();
                        break;
                    case "verification_level":
                        oldVerificationLevel = change.OldValue?.ToObject<VerificationLevel>();
                        newVerificationLevel = change.NewValue?.ToObject<VerificationLevel>();
                        break;
                    case "owner":
                        oldOwnerId = change.OldValue?.ToObject<ulong>();
                        newOwnerId = change.NewValue?.ToObject<ulong>();
                        break;
                    case "mfa_level":
                        oldMfaLevel = change.OldValue?.ToObject<MfaLevel>();
                        newMfaLevel = change.OldValue?.ToObject<MfaLevel>();
                        break;
                    case "explicit_content_filter":
                        oldContentFilter = change.OldValue?.ToObject<int>();
                        newContentFilter = change.NewValue?.ToObject<int>();
                        break;
                }
            }

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
