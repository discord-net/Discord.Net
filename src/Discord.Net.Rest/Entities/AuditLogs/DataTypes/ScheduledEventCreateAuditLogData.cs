using Discord.API.AuditLogs;
using System;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a scheduled event creation.
/// </summary>
public class ScheduledEventCreateAuditLogData : IAuditLogData
{
    private ScheduledEventCreateAuditLogData(ulong id, ScheduledEventInfoAuditLogModel model, IGuildScheduledEvent scheduledEvent)
    {
        Id = id;
        ChannelId = model.ChannelId;
        Name = model.Name;
        Description = model.Description;
        ScheduledStartTime = model.StartTime;
        ScheduledEndTime = model.EndTime;
        PrivacyLevel = model.PrivacyLevel!.Value;
        Status = model.EventStatus!.Value;
        EntityType = model.EventType!.Value;
        EntityId = model.EntityId;
        Location = model.Location;
        Image = model.Image;
        ScheduledEvent = scheduledEvent;
    }

    internal static ScheduledEventCreateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<ScheduledEventInfoAuditLogModel>(changes, discord);

        var scheduledEvent = log.GuildScheduledEvents.FirstOrDefault(x => x.Id == entry.TargetId);

        return new ScheduledEventCreateAuditLogData(entry.TargetId!.Value, data, RestGuildEvent.Create(discord, null, scheduledEvent));
    }

    /// <summary>
    ///     Gets the scheduled event this log corresponds to.
    /// </summary>
    public IGuildScheduledEvent ScheduledEvent { get; }

    // Doc Note: Corresponds to the *current* data

    /// <summary>
    ///     Gets the snowflake id of the event.
    /// </summary>
    public ulong Id { get; }
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
    ///     Gets the image hash of the image that was attached to the event. Null if not set.
    /// </summary>
    public string Image { get; }
}
