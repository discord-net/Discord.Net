using Discord.API.AuditLogs;

using System.Collections.Generic;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a thread creation.
/// </summary>
public class ThreadCreateAuditLogData : IAuditLogData
{
    private ThreadCreateAuditLogData(IThreadChannel thread, ulong id, ThreadInfoAuditLogModel model)
    {
        Thread = thread;
        ThreadId = id;

        ThreadName = model.Name;
        IsArchived = model.IsArchived!.Value;
        AutoArchiveDuration = model.ArchiveDuration!.Value;
        IsLocked = model.IsLocked!.Value;
        SlowModeInterval = model.SlowModeInterval;
        AppliedTags = model.AppliedTags;
        Flags = model.ChannelFlags;
        ThreadType = model.Type;
    }

    internal static ThreadCreateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (_, data) = AuditLogHelper.CreateAuditLogEntityInfo<ThreadInfoAuditLogModel>(changes, discord);

        var threadInfo = log.Threads.FirstOrDefault(x => x.Id == entry.TargetId!.Value);
        var threadChannel = threadInfo == null ? null : RestThreadChannel.Create(discord, (IGuild)null, threadInfo);

        return new ThreadCreateAuditLogData(threadChannel, entry.TargetId!.Value, data);
    }
        
    /// <summary>
    ///     Gets the thread that was created if it still exists.
    /// </summary>
    /// <returns>
    ///     A thread object representing the thread that was created if it still exists, otherwise returns <c>null</c>.
    /// </returns>
    public IThreadChannel Thread { get; }

    /// <summary>
    ///     Gets the snowflake ID of the thread.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the thread.
    /// </returns>
    public ulong ThreadId { get; }

    /// <summary>
    ///     Gets the name of the thread.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the thread.
    /// </returns>
    public string ThreadName { get; }

    /// <summary>
    ///     Gets the type of the thread.
    /// </summary>
    /// <returns>
    ///     The type of thread.
    /// </returns>
    public ThreadType ThreadType { get; }

    /// <summary>
    ///     Gets the value that indicates whether the thread is archived.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if this thread has the Archived flag enabled; otherwise <c>false</c>.
    /// </returns>
    public bool IsArchived { get; }

    /// <summary>
    ///     Gets the auto archive duration of the thread.
    /// </summary>
    /// <returns>
    ///     The thread auto archive duration of the thread.
    /// </returns>
    public ThreadArchiveDuration AutoArchiveDuration { get; }

    /// <summary>
    ///     Gets the value that indicates whether the thread is locked.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if this thread has the Locked flag enabled; otherwise <c>false</c>.
    /// </returns>
    public bool IsLocked { get; }

    /// <summary>
    ///     Gets the slow-mode delay of the thread.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the time in seconds required before the user can send another
    ///     message; <c>0</c> if disabled.
    ///     <c>null</c> if this is not mentioned in this entry.
    /// </returns>
    public int? SlowModeInterval { get; }

    /// <summary>
    ///     Gets the applied tags of this thread.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the property was not updated.
    /// </remarks>
    public IReadOnlyCollection<ulong> AppliedTags { get; }

    /// <summary>
    ///     Gets the flags of the thread channel.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the property was not updated.
    /// </remarks>
    public ChannelFlags? Flags { get; }
}
