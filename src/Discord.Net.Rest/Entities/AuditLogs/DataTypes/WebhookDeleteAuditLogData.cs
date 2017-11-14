using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class WebhookDeleteAuditLogData : IAuditLogData
    {
        private WebhookDeleteAuditLogData(ulong id, ulong channel, string name, string avatar)
        {
            WebhookId = id;
            ChannelId = channel;
            Name = name;
            Avatar = avatar;
        }

        internal static WebhookDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var channelIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "channel_id");
            var typeModel = changes.FirstOrDefault(x => x.ChangedProperty == "type");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var avatarHashModel = changes.FirstOrDefault(x => x.ChangedProperty == "avatar_hash");

            var channelId = channelIdModel.OldValue.ToObject<ulong>();
            var type = typeModel.OldValue.ToObject<int>();
            var name = nameModel.OldValue.ToObject<string>();
            var avatarHash = avatarHashModel?.OldValue?.ToObject<string>();

            return new WebhookDeleteAuditLogData(entry.TargetId.Value, channelId, name, avatarHash);
        }

        public ulong WebhookId { get; }
        public ulong ChannelId { get; }
        public string Name { get; }
        public string Avatar { get; }
    }
}
