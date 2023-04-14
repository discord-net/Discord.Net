using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a scheduled event updates.
/// </summary>
public class SocketScheduledEventUpdateAuditLogData : ISocketAuditLogData
{
    private SocketScheduledEventUpdateAuditLogData(ulong id, SocketScheduledEventInfo before, SocketScheduledEventInfo after)
    {
        Id = id;
        Before = before;
        After = after;
    }

    internal static SocketScheduledEventUpdateAuditLogData Create(BaseDiscordClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<ScheduledEventInfoAuditLogModel>(changes, discord);

        return new SocketScheduledEventUpdateAuditLogData(entry.TargetId!.Value, new(before), new(after));
    }

    // Doc Note: Corresponds to the *current* data

    /// <summary>
    ///     Gets the snowflake id of the event.
    /// </summary>
    public ulong Id { get; }

    /// <summary>
    ///     Gets the state before the change.
    /// </summary>
    public SocketScheduledEventInfo Before { get; }

    /// <summary>
    ///     Gets the state after the change.
    /// </summary>
    public SocketScheduledEventInfo After { get; }
}
