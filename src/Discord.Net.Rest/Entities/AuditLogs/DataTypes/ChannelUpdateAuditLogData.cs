using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class ChannelUpdateAuditLogData : IAuditLogData
    {
        private ChannelUpdateAuditLogData(GuildChannelProperties before, GuildChannelProperties after)
        {
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

            if (topicModel != null) //If topic is supplied, we must be a text channel
            {
                var before = new TextChannelProperties
                {
                    Name = nameModel?.OldValue?.ToObject<string>(),
                    Topic = topicModel.OldValue?.ToObject<string>()
                };
                var after = new TextChannelProperties
                {
                    Name = nameModel?.NewValue?.ToObject<string>(),
                    Topic = topicModel.NewValue?.ToObject<string>()
                };

                return new ChannelUpdateAuditLogData(before, after);
            }
            else //By process of elimination, we must be a voice channel
            {
                var beforeBitrate = bitrateModel?.OldValue?.ToObject<int>();
                var afterBitrate = bitrateModel?.NewValue?.ToObject<int>();
                var beforeUserLimit = userLimitModel?.OldValue?.ToObject<int>();
                var afterUserLimit = userLimitModel?.NewValue?.ToObject<int>();

                var before = new VoiceChannelProperties
                {
                    Name = nameModel?.OldValue?.ToObject<string>(),
                    Bitrate = beforeBitrate ?? Optional<int>.Unspecified,
                    UserLimit = beforeUserLimit
                };
                var after = new VoiceChannelProperties
                {
                    Name = nameModel?.NewValue?.ToObject<string>(),
                    Bitrate = afterBitrate ?? Optional<int>.Unspecified,
                    UserLimit = afterUserLimit
                };

                return new ChannelUpdateAuditLogData(before, after);
            }
        }

        public GuildChannelProperties Before { get; set; }
        public GuildChannelProperties After { get; set; }
    }
}
