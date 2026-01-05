using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     A class containing utilities related to <c>libdave</c>.
/// </summary>
internal static class Utils
{
    /// <summary>
    ///     The max size in bytes of a c-string for a snowflake identifier.
    /// </summary>
    public const int SnowflakeMaxCStringLength = 21;

    /// <summary>
    ///     Converts a snowflake to a C string.
    /// </summary>
    /// <param name="id">The snowflake to convert.</param>
    /// <param name="str">The span containing the C string of the snowflake.</param>
    /// <returns>A handle used to free the C string.</returns>
    /// <exception cref="InvalidOperationException">The ID couldn't be formatted.</exception>
    public static unsafe AllocHandle ToCString(ulong id, out ReadOnlySpan<byte> str)
    {
        var ptr = (byte*)NativeMemory.Alloc(SnowflakeMaxCStringLength);
        var span = new Span<byte>(ptr, SnowflakeMaxCStringLength);

        if (!id.TryFormat(span, out var sz))
            throw new InvalidOperationException();

        str = new(ptr, sz);

        return new(ptr);
    }

    /// <summary>
    ///     Converts a collection of snowflake identifiers into the C-ABI format used by <c>libdave</c>.
    /// </summary>
    /// <param name="ids">The ids to convert.</param>
    /// <returns>The converted ids.</returns>
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
