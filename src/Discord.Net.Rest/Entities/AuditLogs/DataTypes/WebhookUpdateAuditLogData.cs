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
        private WebhookUpdateAuditLogData(IWebhook webhook, WebhookInfo before, WebhookInfo after)
        {
            Webhook = webhook;
            Before = before;
            After = after;
        }

        internal static WebhookUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var channelIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "channel_id");
            var avatarHashModel = changes.FirstOrDefault(x => x.ChangedProperty == "avatar_hash");

            var oldName = nameModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer);
            var oldChannelId = channelIdModel?.OldValue?.ToObject<ulong>(discord.ApiClient.Serializer);
            var oldAvatar = avatarHashModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer);
            var before = new WebhookInfo(oldName, oldChannelId, oldAvatar);

            var newName = nameModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
            var newChannelId = channelIdModel?.NewValue?.ToObject<ulong>(discord.ApiClient.Serializer);
            var newAvatar = avatarHashModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
            var after = new WebhookInfo(newName, newChannelId, newAvatar);

            var webhookInfo = log.Webhooks?.FirstOrDefault(x => x.Id == entry.TargetId);
            var webhook = webhookInfo != null ? RestWebhook.Create(discord, (IGuild)null, webhookInfo) : null;

            return new WebhookUpdateAuditLogData(webhook, before, after);
        }

        //Again, the *current* data
        public IWebhook Webhook { get; }

        //And the *audit log* data
        public WebhookInfo Before { get; }
        public WebhookInfo After { get; }
    }
}
