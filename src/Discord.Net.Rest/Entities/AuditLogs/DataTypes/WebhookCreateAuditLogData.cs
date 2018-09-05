using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class WebhookCreateAuditLogData : IAuditLogData
    {
        private WebhookCreateAuditLogData(IWebhook webhook, WebhookType type, string name, ulong channelId)
        {
            Webhook = webhook;
            Name = name;
            Type = type;
            ChannelId = channelId;
        }

        internal static WebhookCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var channelIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "channel_id");
            var typeModel = changes.FirstOrDefault(x => x.ChangedProperty == "type");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");

            var channelId = channelIdModel.NewValue.ToObject<ulong>(discord.ApiClient.Serializer);
            var type = typeModel.NewValue.ToObject<WebhookType>(discord.ApiClient.Serializer);
            var name = nameModel.NewValue.ToObject<string>(discord.ApiClient.Serializer);

            var webhookInfo = log.Webhooks?.FirstOrDefault(x => x.Id == entry.TargetId);
            var webhook = RestWebhook.Create(discord, (IGuild)null, webhookInfo);

            return new WebhookCreateAuditLogData(webhook, type, name, channelId);
        }

        //Corresponds to the *current* data
        public IWebhook Webhook { get; }

        //Corresponds to the *audit log* data
        public WebhookType Type { get; }
        public string Name { get; }
        public ulong ChannelId { get; }
    }
}
