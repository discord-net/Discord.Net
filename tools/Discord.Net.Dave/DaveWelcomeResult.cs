using Discord.LibDave.Binding;

namespace Discord.LibDave;

/// <summary>
///     A class representing the result of processing a welcome message within the <see cref="libdave"/> library.
/// </summary>
/// <param name="handle">The underlying handle to the welcome result within the <see cref="libdave"/> library.</param>
public sealed class DaveWelcomeResult(WelcomeResultHandle handle) : IRosterProvider, INativeHandle
{
    public UIntPtr UnderlyingHandle { get; } = handle;

    public bool IsAlive { get; private set; } = handle is not 0;

    private readonly Lock _lock = new();

    /// <inheritdoc/>
    public unsafe DaveAllocatedSpan<ulong> GetRosterMemberIds()
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            ulong* ptr;
            nint length;

            libdave.WelcomeResultGetRosterMemberIds(
                UnderlyingHandle,
                &ptr,
                &length
            );

            return new(ptr, (int)length);
        }
    }

    /// <inheritdoc/>
    public unsafe DaveAllocatedSpan<byte> GetRosterMemberSignature(ulong userId)
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            byte* ptr;
            nint length;

            libdave.WelcomeResultGetRosterMemberSignature(
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

            libdave.WelcomeResultDestroy(UnderlyingHandle);

            IsAlive = false;
        }
    }
}
