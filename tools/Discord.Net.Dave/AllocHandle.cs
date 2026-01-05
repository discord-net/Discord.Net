using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     A <see langword="ref"/> <see langword="struct"/> wrapping a pointer to native memory, providing <c>free</c> as
///     a disposable.
/// </summary>
/// <param name="ptr">The underlying pointer to native memory.</param>
internal unsafe ref struct AllocHandle(void* ptr) : IDisposable
{
    private int _freed = ptr is null ? 1 : 0;

    /// <summary>
    ///     The underlying pointer.
    /// </summary>
    public readonly void* Pointer = ptr;

    /// <summary>
    ///     Frees the underlying pointer.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _freed, 1, 0) is 0)
            NativeMemory.Free(Pointer);
    }
}
