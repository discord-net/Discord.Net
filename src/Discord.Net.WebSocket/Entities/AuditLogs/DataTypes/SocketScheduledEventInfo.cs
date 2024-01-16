using Discord.API.AuditLogs;
using System;

namespace Discord.WebSocket;

/// <summary>
///     Represents information for a scheduled event.
/// </summary>
public class SocketScheduledEventInfo
{
    /// <summary>
    ///     Gets the snowflake id of the channel the event is associated with.
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
    ///     Gets the metadata for the entity associated with the event.
    /// </summary>
    public string Location { get; }

    /// <summary>
    ///     Gets the image hash of the image that was attached to the event. Null if not set.
    /// </summary>
    public string Image { get; }

    internal SocketScheduledEventInfo(ScheduledEventInfoAuditLogModel model)
    {
        ChannelId = model.ChannelId;
        Name = model.Name;
        Description = model.Description;
        ScheduledStartTime = model.StartTime;
        ScheduledEndTime = model.EndTime;
        PrivacyLevel = model.PrivacyLevel;
        Status = model.EventStatus;
        EntityType = model.EventType;
        EntityId = model.EntityId;
        Location = model.Location;
        Image = model.Image;
    }
}
