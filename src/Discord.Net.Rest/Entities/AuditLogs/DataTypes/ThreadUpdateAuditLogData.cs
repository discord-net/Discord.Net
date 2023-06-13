using Discord.API.AuditLogs;

using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a thread update.
/// </summary>
public class ThreadUpdateAuditLogData : IAuditLogData
{
    private ThreadUpdateAuditLogData(IThreadChannel thread, ThreadType type, ThreadInfo before, ThreadInfo after)
    {
        Thread = thread;
        ThreadType = type;
        Before = before;
        After = after;
    }

    internal static ThreadUpdateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;
            
        var (before, after) = AuditLogHelper.CreateAuditLogEntityInfo<ThreadInfoAuditLogModel>(changes, discord);

        var threadInfo = log.Threads.FirstOrDefault(x => x.Id == entry.TargetId!.Value);
        var threadChannel = threadInfo == null ? null : RestThreadChannel.Create(discord, (IGuild)null, threadInfo);

        return new ThreadUpdateAuditLogData(threadChannel, before.Type, new(before), new (after));
    }

    // Doc Note: Corresponds to the *current* data

    /// <summary>
    ///     Gets the thread that was created if it still exists.
    /// </summary>
    /// <returns>
    ///     A thread object representing the thread that was created if it still exists, otherwise returns <c>null</c>.
    /// </returns>
    public IThreadChannel Thread { get; }

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
