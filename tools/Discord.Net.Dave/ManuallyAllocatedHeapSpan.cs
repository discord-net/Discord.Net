using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     Represents a span of manually allocated native memory.
/// </summary>
/// <param name="ptr">The pointer to the first element.</param>
/// <param name="length">The number of elements.</param>
/// <typeparam name="T">The underlying type of the span.</typeparam>
public readonly unsafe struct ManuallyAllocatedHeapSpan<T>(
    T* ptr,
    int length
) : IDisposable
    where T : unmanaged
{
    /// <summary>
    ///     The length of this span.
    /// </summary>
    public readonly int Length = length;

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

    /// <summary>
    ///     Frees the underlying allocated memory.
    /// </summary>
    public void Dispose() => NativeMemory.Free(ptr);
}
