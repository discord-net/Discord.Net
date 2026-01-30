using Discord.LibDave.Binding;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     Represents a region of memory allocated by the <see cref="libdave"/> binding.
/// </summary>
/// <param name="ptr">The pointer to the first element in the span.</param>
/// <param name="length">The number of elements in the span.</param>
/// <typeparam name="T">The underlying element type.</typeparam>
public unsafe struct DaveAllocatedSpan<T>(
    T* ptr,
    int length
) : IDisposable
    where T : unmanaged
{
    /// <summary>
    ///     Gets the length of this span.
    /// </summary>
    public readonly int Length = length;

    /// <summary>
    ///     Gets an element in this span at the specified index.
    /// </summary>
    /// <param name="index">The index of the element to get.</param>
    /// <exception cref="IndexOutOfRangeException">The index is out of bounds.</exception>
    public ref readonly T this[int index]
    {
        get
        {
            if (index >= Length || index < 0) throw new IndexOutOfRangeException();

            return ref Unsafe.AsRef<T>(Unsafe.Add<T>(ptr, index));
        }
    }

    /// <summary>
    ///     Gets whether this span has any data.
    /// </summary>
    public bool HasData => !IsEmpty && !IsNull;

    /// <summary>
    ///     Gets whether the span is empty.
    /// </summary>
    public bool IsEmpty => Length is 0;

    /// <summary>
    ///     Gets whether the underlying pointer is null.
    /// </summary>
    public bool IsNull => ptr is null;

    /// <summary>
    ///     Creates a <see cref="ReadOnlySpan{T}"/> wrapping the underlying pointer.
    /// </summary>
    public ReadOnlySpan<T> AsSpan => new(ptr, Length);

    /// <summary>
    ///     Copies this span to a new array.
    /// </summary>
    /// <returns>The array containing a copy of this spans content.</returns>
    public T[] ToArray() => AsSpan.ToArray();

    /// <summary>
    ///     Copies this span to a new <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <returns>The <see cref="ReadOnlyMemory{T}"/> containing a copy of this spans content.</returns>
    public ReadOnlyMemory<T> ToMemory() => new(ToArray(), 0, Length);

    /// <inheritdoc/>
    public void Dispose()
    {
        var value = (IntPtr)ptr;

        if (value is 0) return;

        // best way I found to ref the 'ptr' field
        ref var ptrField = ref Unsafe.AsRef<IntPtr>(Unsafe.AsPointer(ref this));

        if (Interlocked.CompareExchange(ref ptrField, IntPtr.Zero, value) == value)
        {
            libdave.Free(ptr);
        }
    }
}
