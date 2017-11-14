using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class WebhookUpdateAuditLogData : IAuditLogData
    {
        private WebhookUpdateAuditLogData(IWebhookUser user, string token, WebhookInfo before, WebhookInfo after)
        {
            Webhook = user;
            Token = token;
            Before = before;
            After = after;
        }

        internal static WebhookUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var channelIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "channel_id");
            var avatarHashModel = changes.FirstOrDefault(x => x.ChangedProperty == "avatar_hash");

            var oldName = nameModel?.OldValue?.ToObject<string>();
            var oldChannelId = channelIdModel?.OldValue?.ToObject<ulong>();
            var oldAvatar = avatarHashModel?.OldValue?.ToObject<string>();
            var before = new WebhookInfo(oldName, oldChannelId, oldAvatar);

            var newName = nameModel?.NewValue?.ToObject<string>();
            var newChannelId = channelIdModel?.NewValue?.ToObject<ulong>();
            var newAvatar = avatarHashModel?.NewValue?.ToObject<string>();
            var after = new WebhookInfo(newName, newChannelId, newAvatar);

            var webhookInfo = log.Webhooks?.FirstOrDefault(x => x.Id == entry.TargetId);
            var userInfo = RestWebhookUser.Create(discord, null, webhookInfo, entry.TargetId.Value);

            return new WebhookUpdateAuditLogData(userInfo, webhookInfo.Token, before, after);
        }

        //Again, the *current* data
        public IWebhookUser Webhook { get; }
        public string Token { get; }

        //And the *audit log* data
        public WebhookInfo Before { get; }
        public WebhookInfo After { get; }

        public struct WebhookInfo
        {
            internal WebhookInfo(string name, ulong? channelId, string avatar)
            {
                Name = name;
                ChannelId = channelId;
                Avatar = avatar;
            }

            public string Name { get; }
            public ulong? ChannelId { get; }
            public string Avatar { get; }
        }
    }
}
