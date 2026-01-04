using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     A <see langword="ref"/> <see langword="struct"/> representing an array of snowflakes, encoded as strings,
///     allocated in native memory.
/// </summary>
internal readonly unsafe ref struct IdsHandle : IDisposable
{
    /// <summary>
    ///     The pointer of the first id.
    /// </summary>
    public readonly byte** Pointer;

    /// <summary>
    ///     The number of ids.
    /// </summary>
    public readonly int Length;

    /// <summary>
    ///     Creates a new <see cref="IdsHandle"/>.
    /// </summary>
    /// <param name="pointer">The pointer to the first element.</param>
    /// <param name="length">The number of elements.</param>
    public IdsHandle(void* pointer, int length)
    {
        Length = length;
        Pointer = (byte**)pointer;
    }

    /// <summary>
    ///     Frees the underlying memory.
    /// </summary>
    public void Dispose()
    {
        var span = new Span<nuint>(Pointer, Length);

        for (var i = 0; i < span.Length; i++)
        {
            NativeMemory.Free((void*)span[i]);
        }

        NativeMemory.Free(Pointer);
    }
}
