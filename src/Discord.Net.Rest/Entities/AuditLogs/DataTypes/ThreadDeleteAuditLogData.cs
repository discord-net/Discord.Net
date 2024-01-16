using Discord.API.AuditLogs;
using System.Collections.Generic;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a thread deletion.
/// </summary>
public class ThreadDeleteAuditLogData : IAuditLogData
{
    private ThreadDeleteAuditLogData(ulong id, ThreadInfoAuditLogModel model)
    {
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

    internal static ThreadDeleteAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var changes = entry.Changes;

        var (data, _) = AuditLogHelper.CreateAuditLogEntityInfo<ThreadInfoAuditLogModel>(changes, discord);

        return new ThreadDeleteAuditLogData(entry.TargetId!.Value, data);
    }

    /// <summary>
    ///     Gets the snowflake ID of the deleted thread.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the deleted thread.
    /// 
    /// </returns>
    public ulong ThreadId { get; }

    /// <summary>
    ///     Gets the name of the deleted thread.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the deleted thread.
    /// 
    /// </returns>
    public string ThreadName { get; }

    /// <summary>
    ///     Gets the type of the deleted thread.
    /// </summary>
    /// <returns>
    ///     The type of thread that was deleted.
    /// </returns>
    public ThreadType ThreadType { get; }

    /// <summary>
    ///     Gets the value that indicates whether the deleted thread was archived.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this thread had the Archived flag enabled; otherwise <see langword="false" />.
    /// </returns>
    public bool IsArchived { get; }

    /// <summary>
    ///     Gets the thread auto archive duration of the deleted thread.
    /// </summary>
    /// <returns>
    ///     The thread auto archive duration of the thread that was deleted.
    /// </returns>
    public ThreadArchiveDuration AutoArchiveDuration { get; }

    /// <summary>
    ///     Gets the value that indicates whether the deleted thread was locked.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this thread had the Locked flag enabled; otherwise <see langword="false" />.
    /// </returns>
    public bool IsLocked { get; }

    /// <summary>
    ///     Gets the slow-mode delay of the deleted thread.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the time in seconds required before the user can send another
    ///     message; <c>0</c> if disabled.
    ///     <see langword="null" /> if this is not mentioned in this entry.
    /// </returns>
    public int? SlowModeInterval { get; }

    /// <summary>
    ///     Gets the applied tags of this thread.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if this is not mentioned in this entry.
    /// </remarks>
    public IReadOnlyCollection<ulong> AppliedTags { get; }

    /// <summary>
    ///     Gets the flags of the thread channel.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if this is not mentioned in this entry.
    /// </remarks>
    public ChannelFlags? Flags { get; }
}
