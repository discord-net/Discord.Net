using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a stage instance update.
    /// </summary>
    public class StageInstanceUpdatedAuditLogData
    {
        /// <summary>
        ///     Gets the Id of the stage channel.
        /// </summary>
        public ulong StageChannelId { get; }

        /// <summary>
        ///     Gets the stage information before the changes.
        /// </summary>
        public StageInfo Before { get; }

        /// <summary>
        ///     Gets the stage information after the changes.
        /// </summary>
        public StageInfo After { get; }

        internal StageInstanceUpdatedAuditLogData(ulong channelId, StageInfo before, StageInfo after)
        {
            StageChannelId = channelId;
            Before = before;
            After = after;
        }

        internal static StageInstanceUpdatedAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var channelId = entry.Options.ChannelId.Value;

            var topic = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "topic");
            var privacy = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "privacy");

            var user = RestUser.Create(discord, log.Users.FirstOrDefault(x => x.Id == entry.UserId));

            var oldTopic = topic?.OldValue.ToObject<string>();
            var newTopic = topic?.NewValue.ToObject<string>();
            var oldPrivacy = privacy?.OldValue.ToObject<StagePrivacyLevel>();
            var newPrivacy = privacy?.NewValue.ToObject<StagePrivacyLevel>();

            return new StageInstanceUpdatedAuditLogData(channelId, new StageInfo(user, oldPrivacy, oldTopic), new StageInfo(user, newPrivacy, newTopic));
        }
    }
}
