using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a stage going live.
    /// </summary>
    public class StageInstanceCreateAuditLogData : IAuditLogData
    {
        /// <summary>
        ///     Gets the topic of the stage channel.
        /// </summary>
        public string Topic { get; }

        /// <summary>
        ///     Gets the privacy level of the stage channel.
        /// </summary>
        public StagePrivacyLevel PrivacyLevel { get; }

        /// <summary>
        ///     Gets the user who started the stage channel.
        /// </summary>
        public IUser User { get; }

        /// <summary>
        ///     Gets the Id of the stage channel.
        /// </summary>
        public ulong StageChannelId { get; }

        internal StageInstanceCreateAuditLogData(string topic, StagePrivacyLevel privacyLevel, IUser user, ulong channelId)
        {
            Topic = topic;
            PrivacyLevel = privacyLevel;
            User = user;
            StageChannelId = channelId;
        }

        internal static StageInstanceCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var topic = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "topic").NewValue.ToObject<string>(discord.ApiClient.Serializer);
            var privacyLevel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "privacy_level").NewValue.ToObject<StagePrivacyLevel>(discord.ApiClient.Serializer);
            var user = log.Users.FirstOrDefault(x => x.Id == entry.UserId);
            var channelId = entry.Options.ChannelId;

            return new StageInstanceCreateAuditLogData(topic, privacyLevel, RestUser.Create(discord, user), channelId ?? 0);
        }
    }
}
