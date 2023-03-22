using Discord.API.AuditLogs;
using System.Collections.Generic;

namespace Discord.WebSocket;

/// <summary>
///     Represents information for a thread.
/// </summary>
public class SocketThreadInfo
{
    /// <summary>
    ///     Gets the name of the thread.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the value that indicates whether the thread is archived.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the property was not updated.
    /// </remarks>
    public bool? IsArchived { get; }

    /// <summary>
    ///     Gets the auto archive duration of thread.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the property was not updated.
    /// </remarks>
    public ThreadArchiveDuration? AutoArchiveDuration { get; }

    /// <summary>
    ///     Gets the value that indicates whether the thread is locked.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the property was not updated.
    /// </remarks>
    public bool? IsLocked { get; }

    /// <summary>
    ///     Gets the slow-mode delay of the thread.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the property was not updated.
    /// </remarks>
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

    /// <summary>
    ///     Gets the type of the thread.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the property was not updated.
    /// </remarks>
    public ThreadType Type { get; }

    internal SocketThreadInfo(ThreadInfoAuditLogModel model)
    {
        Name = model.Name;
        IsArchived = model.IsArchived;
        AutoArchiveDuration = model.ArchiveDuration;
        IsLocked = model.IsLocked;
        SlowModeInterval = model.SlowModeInterval;
        AppliedTags = model.AppliedTags;
        Flags = model.ChannelFlags;
        Type = model.Type;
    }
}
