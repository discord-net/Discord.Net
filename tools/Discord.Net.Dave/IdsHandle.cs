using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     A <see langword="ref"/> <see langword="struct"/> representing an array of snowflakes, encoded as strings,
///     allocated in native memory.
/// </summary>
internal unsafe ref struct IdsHandle : IDisposable
{
    /// <summary>
    ///     The pointer of the first id.
    /// </summary>
    public byte** Pointer => (byte**)_pointer;

    /// <summary>
    ///     The number of ids.
    /// </summary>
    public readonly int Length;

    private IntPtr _pointer;

    /// <summary>
    ///     Creates a new <see cref="IdsHandle"/>.
    /// </summary>
    /// <param name="pointer">The pointer to the first element.</param>
    /// <param name="length">The number of elements.</param>
    public IdsHandle(void* pointer, int length)
    {
        Length = length;
        _pointer = (IntPtr)pointer;
    }

    /// <summary>
    ///     Frees the underlying memory.
    /// </summary>
    public void Dispose()
    {
        var value = _pointer;

        if (value is 0) return;

        if (Interlocked.CompareExchange(ref _pointer, 0, value) is 0)
        {
            var span = new Span<nuint>((void*)value, Length);

            for (var i = 0; i < span.Length; i++)
            {
                NativeMemory.Free((void*)span[i]);
            }

            NativeMemory.Free((void*)value);
        }
    }
}
