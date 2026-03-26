using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     A <see langword="ref"/> <see langword="struct"/> wrapping a pointer to native memory, providing <c>free</c> as
///     a disposable.
/// </summary>
/// <param name="ptr">The underlying pointer to native memory.</param>
internal unsafe ref struct AllocHandle(IntPtr ptr) : IDisposable
{
    /// <summary>
    ///     The underlying pointer.
    /// </summary>
    public IntPtr Pointer => _pointer;

    private IntPtr _pointer = ptr;

    /// <summary>
    ///     Frees the underlying pointer.
    /// </summary>
    public void Dispose()
    {
        var value = _pointer;

        if (value is 0) return;

        if (Interlocked.CompareExchange(ref _pointer, 0, value) is 0)
            NativeMemory.Free((void*)value);
    }
}
