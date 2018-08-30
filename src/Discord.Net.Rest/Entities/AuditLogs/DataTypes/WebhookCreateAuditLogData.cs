using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a webhook creation.
    /// </summary>
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

        // Doc Note: Corresponds to the *current* data

        /// <summary>
        ///     Gets the webhook that was created.
        /// </summary>
        /// <returns>
        ///     A webhook object representing the webhook that was created.
        /// </returns>
        public IWebhook Webhook { get; }

        // Doc Note: Corresponds to the *audit log* data

        /// <summary>
        ///     Gets the type of webhook that was created.
        /// </summary>
        /// <returns>
        ///     The type of webhook that was created.
        /// </returns>
        public WebhookType Type { get; }

        /// <summary>
        ///     Gets the name of the webhook.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the webhook.
        /// </returns>
        public string Name { get; }
        /// <summary>
        ///     Gets the ID of the channel that the webhook could send to.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the channel that the webhook could send
        ///     to.
        /// </returns>
        public ulong ChannelId { get; }
    }
}
