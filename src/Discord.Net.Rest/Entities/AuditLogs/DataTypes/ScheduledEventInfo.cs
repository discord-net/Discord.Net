using System;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents information for a scheduled event.
    /// </summary>
    public class ScheduledEventInfo
    {
        /// <summary>
        ///     Gets the snowflake id of the guild the event is associated with.
        /// </summary>
        public ulong? GuildId { get; }
        /// <summary>
        ///     Gets the snowflake id of the channel the event is associated with. 0 for events with external location.
        /// </summary>
        public ulong? ChannelId { get; }
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
        public DateTimeOffset? ScheduledStartTime { get; }
        /// <summary>
        ///     Gets the time the event was scheduled to end.
        /// </summary>
        public DateTimeOffset? ScheduledEndTime { get; }
        /// <summary>
        ///     Gets the privacy level of the event.
        /// </summary>
        public GuildScheduledEventPrivacyLevel? PrivacyLevel { get; }
        /// <summary>
        ///     Gets the status of the event.
        /// </summary>
        public GuildScheduledEventStatus? Status { get; }
        /// <summary>
        ///     Gets the type of the entity associated with the event (stage / void / external).
        /// </summary>
        public GuildScheduledEventType? EntityType { get; }
        /// <summary>
        ///     Gets the snowflake id of the entity associated with the event (stage / void / external).
        /// </summary>
        public ulong? EntityId { get; }
        /// <summary>
        ///     Gets the metadata for the entity associated with the event. <see cref="Optional{T}.Unspecified"/> if there was no change.
        /// </summary>
        public Optional<string> Location { get; }
        /// <summary>
        ///     Gets the count of users interested in this event. 
        /// </summary>
        public int? UserCount { get; }
        /// <summary>
        ///     Gets the image hash of the image that was attached to the event. Null if not set.
        /// </summary>
        public string Image { get; }

        internal ScheduledEventInfo(ulong? guildId, ulong? channelId, string name, string description, DateTimeOffset? scheduledStartTime, DateTimeOffset? scheduledEndTime, GuildScheduledEventPrivacyLevel? privacyLevel, GuildScheduledEventStatus? status, GuildScheduledEventType? entityType, ulong? entityId, Optional<string> location, int? userCount, string image)
        {
            GuildId = guildId;
            ChannelId = channelId;
            Name = name;
            Description = description;
            ScheduledStartTime = scheduledStartTime;
            ScheduledEndTime = scheduledEndTime;
            PrivacyLevel = privacyLevel;
            Status = status;
            EntityType = entityType;
            EntityId = entityId;
            Location = location;
            UserCount = userCount;
            Image = image;
        }
    }
}
