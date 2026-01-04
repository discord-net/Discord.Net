using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

internal readonly unsafe ref struct AllocHandle(void* ptr) : IDisposable
{
    public readonly void* Pointer = ptr;

    public void Dispose() => NativeMemory.Free(Pointer);
}

public readonly unsafe struct AllocBuffer<T>(
    T* ptr,
    int length
) : IDisposable
    where T : unmanaged
{
    public readonly int Length = length;
    public bool HasData => !IsEmpty && !IsNull;

    public bool IsEmpty => Length is 0;
    public bool IsNull => ptr is null;

    public ReadOnlySpan<T> AsSpan => new(ptr, Length);

    public T[] ToArray() => AsSpan.ToArray();

    public ReadOnlyMemory<T> ToMemory() => new(ToArray(), 0, Length);

    public void Dispose() => NativeMemory.Free(ptr);
}

internal readonly unsafe ref struct IdsHandle : IDisposable
{
    public readonly byte** Pointer;
    public readonly int Length;

    public IdsHandle(void* array, int length)
    {
        Length = length;
        Pointer = (byte**)array;
    }

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

internal static class Utils
{
    public const int SNOWFLAKE_MAX_CSTRING_LENGTH = 21;

    public static unsafe AllocHandle ToCString(ulong id, out ReadOnlySpan<CChar> str)
    {
        var ptr = (CChar*)NativeMemory.Alloc(SNOWFLAKE_MAX_CSTRING_LENGTH);
        var span = new Span<byte>(ptr, SNOWFLAKE_MAX_CSTRING_LENGTH);

        if (!id.TryFormat(span, out var sz))
            throw new InvalidOperationException();

        str = new(ptr, sz);

        return new(ptr);
    }

    public static unsafe IdsHandle GetIds(ICollection<ulong> ids)
    {

        var arr = NativeMemory.Alloc((nuint)(ids.Count * sizeof(nuint)));
        var result = new Span<nuint>(arr, ids.Count);
        var i = 0;

        foreach (var id in ids)
        {
            // dispose is ignored here, 'IdsHandle' will clean the underlying alloc
            ToCString(id, out var str);

            // puts the address of the first element in the span
            result[i++] = (nuint)Unsafe.AsPointer(ref MemoryMarshal.GetReference(str));
        }

        return new(arr, ids.Count);
    }
}
