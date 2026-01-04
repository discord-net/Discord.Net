using Discord.LibDave.Binding;

namespace Discord.LibDave;

/// <summary>
///     A class representing the result of processing a commit within the <see cref="libdave"/> library.
/// </summary>
/// <param name="handle">The underlying handle to the commit object in the <see cref="libdave"/> library.</param>
public sealed class DaveCommitResult(CommitResultHandle handle) : IRosterProvider
{
    /// <summary>
    ///     Gets whether this commit has failed.
    /// </summary>
    public bool IsFailed => libdave.CommitResultIsFailed(handle);

    /// <summary>
    ///     Gets whether this commit is ignored.
    /// </summary>
    public bool IsIgnored => libdave.CommitResultIsIgnored(handle);

    /// <inheritdoc/>
    public unsafe ManuallyAllocatedHeapSpan<ulong> GetRosterMemberIds()
    {
        nuint length;
        ulong* ptr;

        libdave.CommitResultGetRosterMemberIds(
            handle,
            &ptr,
            &length
        );

        return new(ptr, (int)length);
    }

    /// <inheritdoc/>
    public unsafe ManuallyAllocatedHeapSpan<byte> GetRosterMemberSignature(ulong userId)
    {
        nint length;
        byte* ptr;

        libdave.CommitResultGetRosterMemberSignature(
            handle,
            userId,
            &ptr,
            &length
        );

        return new(ptr, (int)length);
    }

    /// <inheritdoc/>
    public void Dispose() => libdave.CommitResultDestroy(handle);
}
