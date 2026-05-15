using Discord.LibDave.Binding;

namespace Discord.LibDave;

/// <summary>
///     Represents a key ratchet within the <see cref="libdave"/> library.
/// </summary>
/// <param name="handle">The underlying handle to the key ratchet object in the <see cref="libdave"/> library.</param>
public sealed class DaveKeyRatchet(KeyRatchetHandle handle) : INativeHandle
{
    /// <inheritdoc/>
    public KeyRatchetHandle UnderlyingHandle { get; } = handle;

    /// <inheritdoc/>
    public bool IsAlive { get; private set; } = handle is not 0;

    private readonly Lock _lock = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            if (!IsAlive) return;

            libdave.KeyRatchetDestroy(UnderlyingHandle);
            IsAlive = false;
        }
    }
}
