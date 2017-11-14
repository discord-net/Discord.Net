using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class WebhookCreateAuditLogData : IAuditLogData
    {
        private WebhookCreateAuditLogData(IWebhookUser user, string token, string name, ulong channelId)
        {
            Webhook = user;
            Token = token;
            Name = name;
            ChannelId = channelId;
        }

        internal static WebhookCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var channelIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "channel_id");
            var typeModel = changes.FirstOrDefault(x => x.ChangedProperty == "type");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");

            var channelId = channelIdModel.NewValue.ToObject<ulong>();
            var type = typeModel.NewValue.ToObject<int>(); //TODO: what on *earth* is this for
            var name = nameModel.NewValue.ToObject<string>();

            var webhookInfo = log.Webhooks?.FirstOrDefault(x => x.Id == entry.TargetId);
            var userInfo = RestWebhookUser.Create(discord, null, webhookInfo, entry.TargetId.Value);

            return new WebhookCreateAuditLogData(userInfo, webhookInfo.Token, name, channelId);
        }

        //Corresponds to the *current* data
        public IWebhookUser Webhook { get; }
        public string Token { get; }

        //Corresponds to the *audit log* data
        public string Name { get; }
        public ulong ChannelId { get; }
    }
}
