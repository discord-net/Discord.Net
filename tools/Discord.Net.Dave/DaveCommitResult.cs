using Discord.LibDave.Binding;

namespace Discord.LibDave;

/// <summary>
///     A class representing the result of processing a commit within the <see cref="libdave"/> library.
/// </summary>
/// <param name="handle">The underlying handle to the commit object in the <see cref="libdave"/> library.</param>
public sealed class DaveCommitResult(CommitResultHandle handle) : IRosterProvider, INativeHandle
{
    /// <inheritdoc/>
    public bool IsAlive { get; private set; } = handle is not 0;

    /// <inheritdoc/>
    public CommitResultHandle UnderlyingHandle { get; } = handle;

    /// <summary>
    ///     Gets whether this commit has failed.
    /// </summary>
    public bool IsFailed => libdave.CommitResultIsFailed(UnderlyingHandle);

    /// <summary>
    ///     Gets whether this commit is ignored.
    /// </summary>
    public bool IsIgnored => libdave.CommitResultIsIgnored(UnderlyingHandle);

    private readonly Lock _lock = new();

    /// <inheritdoc/>
    public unsafe ManuallyAllocatedHeapSpan<ulong> GetRosterMemberIds()
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            nuint length;
            ulong* ptr;

            libdave.CommitResultGetRosterMemberIds(
                UnderlyingHandle,
                &ptr,
                &length
            );

            return new(ptr, (int)length);
        }
    }

    /// <inheritdoc/>
    public unsafe ManuallyAllocatedHeapSpan<byte> GetRosterMemberSignature(ulong userId)
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            nint length;
            byte* ptr;

            libdave.CommitResultGetRosterMemberSignature(
                UnderlyingHandle,
                userId,
                &ptr,
                &length
            );

            return new(ptr, (int)length);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            if (!IsAlive) return;

            libdave.CommitResultDestroy(UnderlyingHandle);
            IsAlive = false;
        }
    }
}
