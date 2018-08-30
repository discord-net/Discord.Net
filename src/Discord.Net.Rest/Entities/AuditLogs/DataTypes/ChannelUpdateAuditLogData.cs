using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class ChannelUpdateAuditLogData : IAuditLogData
    {
        private ChannelUpdateAuditLogData(ulong id, ChannelInfo before, ChannelInfo after)
        {
            ChannelId = id;
            Before = before;
            After = after;
        }

        internal static ChannelUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var topicModel = changes.FirstOrDefault(x => x.ChangedProperty == "topic");
            var bitrateModel = changes.FirstOrDefault(x => x.ChangedProperty == "bitrate");
            var userLimitModel = changes.FirstOrDefault(x => x.ChangedProperty == "user_limit");

            string oldName = nameModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer),
                newName = nameModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
            string oldTopic = topicModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer),
                newTopic = topicModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
            int? oldBitrate = bitrateModel?.OldValue?.ToObject<int>(discord.ApiClient.Serializer),
                newBitrate = bitrateModel?.NewValue?.ToObject<int>(discord.ApiClient.Serializer);
            int? oldLimit = userLimitModel?.OldValue?.ToObject<int>(discord.ApiClient.Serializer),
                newLimit = userLimitModel?.NewValue?.ToObject<int>(discord.ApiClient.Serializer);

            var before = new ChannelInfo(oldName, oldTopic, oldBitrate, oldLimit);
            var after = new ChannelInfo(newName, newTopic, newBitrate, newLimit);

            return new ChannelUpdateAuditLogData(entry.TargetId.Value, before, after);
        }

        public ulong ChannelId { get; }
        public ChannelInfo Before { get; }
        public ChannelInfo After { get; }
    }
}
