using Discord.LibDave.Binding;

namespace Discord.LibDave;

/// <summary>
///     Represents a key ratchet within the <see cref="libdave"/> library.
/// </summary>
/// <param name="handle">The underlying handle to the key ratchet object in the <see cref="libdave"/> library.</param>
public sealed class DaveKeyRatchet(KeyRatchetHandle handle) : IDisposable
{
    /// <summary>
    ///     Gets whether the ratchet is null.
    /// </summary>
    public bool IsNull => Handle is 0;

    /// <summary>
    ///     Gets the underlying handle of the ratchet.
    /// </summary>
    public KeyRatchetHandle Handle { get; } = handle;

    /// <inheritdoc/>
    public void Dispose()
    {
        libdave.KeyRatchetDestroy(Handle);
    }
}
