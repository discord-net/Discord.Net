using System;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a scheduled event updates.
    /// </summary>
    public class ScheduledEventUpdateAuditLogData : IAuditLogData
    {
        private ScheduledEventUpdateAuditLogData(ulong id, ScheduledEventInfo before, ScheduledEventInfo after)
        {
            Id = id;
            Before = before;
            After = after;
        }

        internal static ScheduledEventUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var id = entry.TargetId.Value;

            var guildId = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "guild_id");
            var channelId = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "channel_id");
            var name = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var description = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "description");
            var scheduledStartTime = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "scheduled_start_time");
            var scheduledEndTime = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "scheduled_end_time");
            var privacyLevel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "privacy_level");
            var status = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "status");
            var entityType = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "entity_type");
            var entityId = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "entity_id");
            var location = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "location");
            var userCount = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "user_count");
            var image = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "image");

            var before = new ScheduledEventInfo(
                guildId?.OldValue.ToObject<ulong>(discord.ApiClient.Serializer),
                channelId == null ? null : channelId.OldValue?.ToObject<ulong?>(discord.ApiClient.Serializer) ?? 0,
                name?.OldValue.ToObject<string>(discord.ApiClient.Serializer),
                description?.OldValue.ToObject<Optional<string>>(discord.ApiClient.Serializer)
                    .GetValueOrDefault(),
                scheduledStartTime?.OldValue.ToObject<DateTimeOffset>(discord.ApiClient.Serializer),
                scheduledEndTime?.OldValue.ToObject<DateTimeOffset?>(discord.ApiClient.Serializer),
                privacyLevel?.OldValue.ToObject<GuildScheduledEventPrivacyLevel>(discord.ApiClient.Serializer),
                status?.OldValue.ToObject<GuildScheduledEventStatus>(discord.ApiClient.Serializer),
                entityType?.OldValue.ToObject<GuildScheduledEventType>(discord.ApiClient.Serializer),
                entityId?.OldValue.ToObject<ulong?>(discord.ApiClient.Serializer),
                location == null ? Optional<string>.Unspecified : new Optional<string>(location.OldValue?.ToObject<string>(discord.ApiClient.Serializer)),
                userCount?.OldValue.ToObject<Optional<int>>(discord.ApiClient.Serializer)
                    .GetValueOrDefault(),
                image?.OldValue.ToObject<Optional<string>>(discord.ApiClient.Serializer)
                    .GetValueOrDefault()
            );
            var after = new ScheduledEventInfo(
                guildId?.NewValue.ToObject<ulong>(discord.ApiClient.Serializer),
                channelId == null ? null : channelId.NewValue?.ToObject<ulong?>(discord.ApiClient.Serializer) ?? 0,
                name?.NewValue.ToObject<string>(discord.ApiClient.Serializer),
                description?.NewValue.ToObject<Optional<string>>(discord.ApiClient.Serializer)
                    .GetValueOrDefault(),
                scheduledStartTime?.NewValue.ToObject<DateTimeOffset>(discord.ApiClient.Serializer),
                scheduledEndTime?.NewValue.ToObject<DateTimeOffset?>(discord.ApiClient.Serializer),
                privacyLevel?.NewValue.ToObject<GuildScheduledEventPrivacyLevel>(discord.ApiClient.Serializer),
                status?.NewValue.ToObject<GuildScheduledEventStatus>(discord.ApiClient.Serializer),
                entityType?.NewValue.ToObject<GuildScheduledEventType>(discord.ApiClient.Serializer),
                entityId?.NewValue.ToObject<ulong?>(discord.ApiClient.Serializer),
                location == null ? Optional<string>.Unspecified : new Optional<string>(location.NewValue?.ToObject<string>(discord.ApiClient.Serializer)),
                userCount?.NewValue.ToObject<Optional<int>>(discord.ApiClient.Serializer)
                    .GetValueOrDefault(),
                image?.NewValue.ToObject<Optional<string>>(discord.ApiClient.Serializer)
                    .GetValueOrDefault()
            );

            return new ScheduledEventUpdateAuditLogData(id, before, after);
        }

        // Doc Note: Corresponds to the *current* data

        /// <summary>
        ///     Gets the snowflake id of the event.
        /// </summary>
        public ulong Id { get; }
        /// <summary>
        ///     Gets the state before the change.
        /// </summary>
        public ScheduledEventInfo Before { get; }
        /// <summary>
        ///     Gets the state after the change.
        /// </summary>
        public ScheduledEventInfo After { get; }
    }
}
