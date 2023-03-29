using Discord.API.AuditLogs;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a scheduled event updates.
/// </summary>
public class ScheduledEventUpdateAuditLogData : IAuditLogData
{
    private ScheduledEventUpdateAuditLogData(ulong id, ScheduledEventInfo before, ScheduledEventInfo after, IGuildScheduledEvent scheduledEvent)
    {
        Id = id;
        Before = before;
        After = after;
        ScheduledEvent = scheduledEvent;

    }

    internal static ScheduledEventUpdateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<ScheduledEventInfoAuditLogModel>(changes, discord);

        var scheduledEvent = log.GuildScheduledEvents.FirstOrDefault(x => x.Id == entry.TargetId);

        return new ScheduledEventUpdateAuditLogData(entry.TargetId!.Value, new(before), new(after), RestGuildEvent.Create(discord, null, scheduledEvent));
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
    ///     Gets the state before the change.
    /// </summary>
    public ScheduledEventInfo Before { get; }

    /// <summary>
    ///     Gets the state after the change.
    /// </summary>
    public ScheduledEventInfo After { get; }
}
