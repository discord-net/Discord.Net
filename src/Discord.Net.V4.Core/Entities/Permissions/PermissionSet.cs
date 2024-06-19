using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord;

[StructLayout(LayoutKind.Sequential)]
public struct PermissionSet(ulong lower, ulong upper) : IEquatable<PermissionSet>
{
    private readonly ulong _lower = lower;
    private readonly ulong _upper = upper;

    /// <summary>
    ///     Creates a new permission set with the nth bit set.
    /// </summary>
    /// <remarks>
    ///     The <paramref name="setBit"/> parameter is 0-based, ex <c>new PermissionSet(0)</c> will set the 0th bit
    ///     (first) to <c>1</c>
    /// </remarks>
    /// <param name="setBit">The 0-based bit index to set.</param>
    public PermissionSet(byte setBit) :
        this(
            setBit is <= sizeof(ulong) << 3 and > 0
                ? 1UL << setBit
                : 0L,
            setBit is <= sizeof(ulong) << 4 and > sizeof(ulong) << 3
                ? 1UL << setBit
                : 0L
        ) { }

    public bool Has(in PermissionSet other)
        => (this | other) == other;

    public unsafe bool IsSet(byte bit)
    {
        if (bit << 3 > sizeof(PermissionSet))
            throw new InvalidOperationException($"offset is outside the defined size of {nameof(PermissionSet)}");

        // find the byte that this offset is in
        var byteOffset = bit >> 3;
        var bitOffset = 1 << (bit % 8 - 1);

        // get this as a byte ref
        ref var bytes = ref Unsafe.As<PermissionSet, byte>(ref this);

        // get the byte containing the bit
        ref var setByte = ref Unsafe.Add(ref bytes, byteOffset);

        // return the bit mask of the offset
        return (setByte & bitOffset) > 0;
    }

    public static PermissionSet operator ~(PermissionSet set)
        => new(~set._lower, ~set._upper);

    public static PermissionSet operator &(PermissionSet a, PermissionSet b)
        => new(a._lower & b._lower, a._upper & b._upper);

    public static PermissionSet operator |(PermissionSet a, PermissionSet b)
        => new(a._lower | b._lower, a._upper | b._upper);

    public static PermissionSet operator ^(PermissionSet a, PermissionSet b)
        => new(a._lower ^ b._lower, a._upper ^ b._upper);

    public bool Equals(PermissionSet other) => _lower == other._lower && _upper == other._upper;

    public override bool Equals(object? obj) => obj is PermissionSet other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_lower, _upper);

    public static bool operator ==(PermissionSet left, PermissionSet right) => left.Equals(right);

    public static bool operator !=(PermissionSet left, PermissionSet right) => !left.Equals(right);

#if NET7_0_OR_GREATER
    public static implicit operator Int128(PermissionSet set)
        => new(set._lower, set._upper);

    public static implicit operator PermissionSet(Int128 num)
    {
        ref var numRef = ref Unsafe.As<Int128, ulong>(ref num);

        if (BitConverter.IsLittleEndian)
        {
            return new PermissionSet(
                numRef,
                Unsafe.Add(ref numRef, 1)
            );
        }
        else
        {
            return new PermissionSet(
                Unsafe.Add(ref numRef, 1),
                numRef
            );
        }
    }
#endif


    public static unsafe implicit operator BigInteger(PermissionSet set)
        => new(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<PermissionSet, byte>(ref set), sizeof(PermissionSet)));

    public static unsafe implicit operator PermissionSet(BigInteger set)
    {
        Span<byte> bytes = stackalloc byte[sizeof(PermissionSet)];

        if(!set.TryWriteBytes(bytes, out _))
            throw new ArgumentOutOfRangeException(nameof(set));


        ref var lower = ref Unsafe.As<byte, ulong>(ref bytes[0]);            // 0..4
        ref var upper = ref Unsafe.As<byte, ulong>(ref bytes[sizeof(long)]); // 4..8

        return new PermissionSet(lower, upper);
    }

    public override string ToString()
        => ((BigInteger)this).ToString("D");
}
