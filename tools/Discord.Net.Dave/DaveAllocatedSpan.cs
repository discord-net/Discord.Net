using Discord.LibDave.Binding;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     Represents a region of memory allocated by the <see cref="libdave"/> binding.
/// </summary>
/// <typeparam name="T">The underlying element type.</typeparam>
public unsafe struct DaveAllocatedSpan<T> : IDisposable
    where T : unmanaged
{
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

            if (IsNull) throw new NullReferenceException();

            return ref Unsafe.AsRef<T>(Unsafe.Add<T>((void*)_pointer, index));
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
    public bool IsNull => _pointer is 0;

    /// <summary>
    ///     Creates a <see cref="ReadOnlySpan{T}"/> wrapping the underlying pointer.
    /// </summary>
    public ReadOnlySpan<T> AsSpan
    {
        get
        {
            if (IsNull) throw new NullReferenceException();

            return new((void*)_pointer, Length);
        }
    }

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

    /// <summary>
    ///     Gets the length of this span.
    /// </summary>
    public readonly int Length;

    private IntPtr _pointer;

    /// <summary>
    ///     Constructs a new <see cref="DaveAllocatedSpan{T}"/>
    /// </summary>
    /// <param name="pointer">The pointer to the first element in the span.</param>
    /// <param name="length">The number of elements in the span.</param>
    public DaveAllocatedSpan(T* pointer, int length)
    {
        _pointer = (IntPtr)pointer;
        Length = length;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        var value = _pointer;

        if (value is 0) return;

        if(Interlocked.CompareExchange(ref _pointer, 0, value) is 0)
            libdave.Free((void*)value);
    }
}
