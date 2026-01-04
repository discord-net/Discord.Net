using Discord.LibDave.Binding;

namespace Discord.LibDave;

/// <summary>
///     A class representing the result of processing a welcome message within the <see cref="libdave"/> library.
/// </summary>
/// <param name="handle">The underlying handle to the welcome result within the <see cref="libdave"/> library.</param>
public sealed class DaveWelcomeResult(WelcomeResultHandle handle) :  IRosterProvider
{
    /// <summary>
    ///     Gets whether the result is null.
    /// </summary>
    public bool IsNull => handle is 0;

    /// <inheritdoc/>
    public unsafe ManuallyAllocatedHeapSpan<ulong> GetRosterMemberIds()
    {
        ulong* ptr;
        nint length;

        libdave.WelcomeResultGetRosterMemberIds(
            handle,
            &ptr,
            &length
        );

        return new(ptr, (int)length);
    }

    /// <inheritdoc/>
    public unsafe ManuallyAllocatedHeapSpan<byte> GetRosterMemberSignature(ulong userId)
    {
        byte* ptr;
        nint length;

        libdave.WelcomeResultGetRosterMemberSignature(
            handle,
            userId,
            &ptr,
            &length
        );

        return new(ptr, (int)length);
    }

    /// <inheritdoc/>
    public void Dispose()
        => libdave.WelcomeResultDestroy(handle);
}
