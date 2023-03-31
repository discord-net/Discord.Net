using Discord.API;
using System;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a scheduled event creation.
    /// </summary>
    public class ScheduledEventCreateAuditLogData : IAuditLogData
    {
        private ScheduledEventCreateAuditLogData(ulong id, ulong guildId, ulong? channelId, ulong? creatorId, string name, string description, DateTimeOffset scheduledStartTime, DateTimeOffset? scheduledEndTime, GuildScheduledEventPrivacyLevel privacyLevel, GuildScheduledEventStatus status, GuildScheduledEventType entityType, ulong? entityId, string location, RestUser creator, int userCount, string image)
        {
            Id = id;
            GuildId = guildId;
            ChannelId = channelId;
            CreatorId = creatorId;
            Name = name;
            Description = description;
            ScheduledStartTime = scheduledStartTime;
            ScheduledEndTime = scheduledEndTime;
            PrivacyLevel = privacyLevel;
            Status = status;
            EntityType = entityType;
            EntityId = entityId;
            Location = location;
            Creator = creator;
            UserCount = userCount;
            Image = image;
        }

        internal static ScheduledEventCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var id = entry.TargetId.Value;

            var guildId = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "guild_id")
                    .NewValue.ToObject<ulong>(discord.ApiClient.Serializer);
            var channelId = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "channel_id")
                    .NewValue.ToObject<ulong?>(discord.ApiClient.Serializer);
            var creatorId = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "channel_id")
                    .NewValue.ToObject<Optional<ulong?>>(discord.ApiClient.Serializer)
                    .GetValueOrDefault();
            var name = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name")
                    .NewValue.ToObject<string>(discord.ApiClient.Serializer);
            var description = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "description")
                    .NewValue.ToObject<Optional<string>>(discord.ApiClient.Serializer)
                    .GetValueOrDefault();
            var scheduledStartTime = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "scheduled_start_time")
                    .NewValue.ToObject<DateTimeOffset>(discord.ApiClient.Serializer);
            var scheduledEndTime = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "scheduled_end_time")
                    .NewValue.ToObject<DateTimeOffset?>(discord.ApiClient.Serializer);
            var privacyLevel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "privacy_level")
                    .NewValue.ToObject<GuildScheduledEventPrivacyLevel>(discord.ApiClient.Serializer);
            var status = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "status")
                    .NewValue.ToObject<GuildScheduledEventStatus>(discord.ApiClient.Serializer);
            var entityType = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "entity_type")
                    .NewValue.ToObject<GuildScheduledEventType>(discord.ApiClient.Serializer);
            var entityId = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "entity_id")
                    .NewValue.ToObject<ulong?>(discord.ApiClient.Serializer);
            var entityMetadata = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "entity_metadata")
                    .NewValue.ToObject<GuildScheduledEventEntityMetadata>(discord.ApiClient.Serializer);
            var creator = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "creator")
                    .NewValue.ToObject<Optional<User>>(discord.ApiClient.Serializer)
                    .GetValueOrDefault();
            var userCount = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "user_count")
                    .NewValue.ToObject<Optional<int>>(discord.ApiClient.Serializer)
                    .GetValueOrDefault();
            var image = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "image")
                    .NewValue.ToObject<Optional<string>>(discord.ApiClient.Serializer)
                    .GetValueOrDefault();

            var creatorUser = creator == null ? null : RestUser.Create(discord, creator);

            return new ScheduledEventCreateAuditLogData(id, guildId, channelId, creatorId, name, description, scheduledStartTime, scheduledEndTime, privacyLevel, status, entityType, entityId, entityMetadata.Location.GetValueOrDefault(), creatorUser, userCount, image);
        }

        // Doc Note: Corresponds to the *current* data

        /// <summary>
        ///     Gets the snowflake id of the event.
        /// </summary>
        public ulong Id { get; }
        /// <summary>
        ///     Gets the snowflake id of the guild the event is associated with.
        /// </summary>
        public ulong GuildId { get; }
        /// <summary>
        ///     Gets the snowflake id of the channel the event is associated with.
        /// </summary>
        public ulong? ChannelId { get; }
        /// <summary>
        ///     Gets the snowflake id of the original creator of the event.
        /// </summary>
        public ulong? CreatorId { get; }
        /// <summary>
        ///     Gets name of the event.
        /// </summary>
        public string Name { get; }
        /// <summary>
        ///     Gets the description of the event. null if none is set.
        /// </summary>
        public string Description { get; }
        /// <summary>
        ///     Gets the time the event was scheduled for.
        /// </summary>
        public DateTimeOffset ScheduledStartTime { get; }
        /// <summary>
        ///     Gets the time the event was scheduled to end.
        /// </summary>
        public DateTimeOffset? ScheduledEndTime { get; }
        /// <summary>
        ///     Gets the privacy level of the event.
        /// </summary>
        public GuildScheduledEventPrivacyLevel PrivacyLevel { get; }
        /// <summary>
        ///     Gets the status of the event.
        /// </summary>
        public GuildScheduledEventStatus Status { get; }
        /// <summary>
        ///     Gets the type of the entity associated with the event (stage / void / external).
        /// </summary>
        public GuildScheduledEventType EntityType { get; }
        /// <summary>
        ///     Gets the snowflake id of the entity associated with the event (stage / void / external).
        /// </summary>
        public ulong? EntityId { get; }
        /// <summary>
        ///     Gets the metadata for the entity associated with the event.
        /// </summary>
        public string Location { get; }
        /// <summary>
        ///     Gets the user that originally created the event.
        /// </summary>
        public RestUser Creator { get; }
        /// <summary>
        ///     Gets the count of users interested in this event. 
        /// </summary>
        public int UserCount { get; }
        /// <summary>
        ///     Gets the image hash of the image that was attached to the event. Null if not set.
        /// </summary>
        public string Image { get; }
    }
}
