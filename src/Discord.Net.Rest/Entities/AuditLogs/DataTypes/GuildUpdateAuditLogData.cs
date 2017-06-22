using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class GuildUpdateAuditLogData : IAuditLogData
    {
        private GuildUpdateAuditLogData()
        { }

        internal static GuildUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            int? oldAfkTimeout, newAfkTimeout;
            DefaultMessageNotifications? oldDefaultMessageNotifications, newDefaultMessageNotifications;
            ulong? oldAfkChannelId, newAfkChannelId;
            string oldName, newName;
            string oldRegionId, newRegionId;
            string oldIconHash, newIconHash;

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
                    //TODO: owner, verification level
                }
            }

            //TODO: implement this
            return null;
        }
    }
}
