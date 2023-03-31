using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a channel update.
    /// </summary>
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
            var rateLimitPerUserModel = changes.FirstOrDefault(x => x.ChangedProperty == "rate_limit_per_user");
            var nsfwModel = changes.FirstOrDefault(x => x.ChangedProperty == "nsfw");
            var bitrateModel = changes.FirstOrDefault(x => x.ChangedProperty == "bitrate");
            var typeModel = changes.FirstOrDefault(x => x.ChangedProperty == "type");

            string oldName = nameModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer),
                newName = nameModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
            string oldTopic = topicModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer),
                newTopic = topicModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
            int? oldRateLimitPerUser = rateLimitPerUserModel?.OldValue?.ToObject<int>(discord.ApiClient.Serializer),
                newRateLimitPerUser = rateLimitPerUserModel?.NewValue?.ToObject<int>(discord.ApiClient.Serializer);
            bool? oldNsfw = nsfwModel?.OldValue?.ToObject<bool>(discord.ApiClient.Serializer),
                newNsfw = nsfwModel?.NewValue?.ToObject<bool>(discord.ApiClient.Serializer);
            int? oldBitrate = bitrateModel?.OldValue?.ToObject<int>(discord.ApiClient.Serializer),
                newBitrate = bitrateModel?.NewValue?.ToObject<int>(discord.ApiClient.Serializer);
            ChannelType? oldType = typeModel?.OldValue?.ToObject<ChannelType>(discord.ApiClient.Serializer),
                newType = typeModel?.NewValue?.ToObject<ChannelType>(discord.ApiClient.Serializer);

            var before = new ChannelInfo(oldName, oldTopic, oldRateLimitPerUser, oldNsfw, oldBitrate, oldType);
            var after = new ChannelInfo(newName, newTopic, newRateLimitPerUser, newNsfw, newBitrate, newType);

            return new ChannelUpdateAuditLogData(entry.TargetId.Value, before, after);
        }

        /// <summary>
        ///     Gets the snowflake ID of the updated channel.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the updated channel.
        /// </returns>
        public ulong ChannelId { get; }
        /// <summary>
        ///     Gets the channel information before the changes.
        /// </summary>
        /// <returns>
        ///     An information object containing the original channel information before the changes were made.
        /// </returns>
        public ChannelInfo Before { get; }
        /// <summary>
        ///     Gets the channel information after the changes.
        /// </summary>
        /// <returns>
        ///     An information object containing the channel information after the changes were made.
        /// </returns>
        public ChannelInfo After { get; }
    }
}
