using Discord.API.AuditLogs;
using Discord.Rest;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a thread update.
/// </summary>
public class SocketThreadUpdateAuditLogData : ISocketAuditLogData
{
    private SocketThreadUpdateAuditLogData(ThreadType type, ThreadInfo before, ThreadInfo after)
    {
        ThreadType = type;
        Before = before;
        After = after;
    }

    internal static SocketThreadUpdateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;
            
        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<ThreadInfoAuditLogModel>(changes, discord);

        return new SocketThreadUpdateAuditLogData(before.Type, new(before), new (after));
    }

    /// <summary>
    ///     Gets the type of the thread.
    /// </summary>
    /// <returns>
    ///     The type of thread.
    /// </returns>
    public ThreadType ThreadType { get; }

    /// <summary>
    ///     Gets the thread information before the changes.
    /// </summary>
    /// <returns>
    ///     A thread information object representing the thread before the changes were made.
    /// </returns>
    public ThreadInfo Before { get; }

    /// <summary>
    ///     Gets the thread information after the changes.
    /// </summary>
    /// <returns>
    ///     A thread information object representing the thread after the changes were made.
    /// </returns>
    public ThreadInfo After { get; }
}
