using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord.Models;

[StructLayout(LayoutKind.Sequential)]
public readonly struct PermissionBitSet(ulong lower, ulong upper) : IEquatable<PermissionBitSet>
{
    private readonly ulong _lower = lower;
    private readonly ulong _upper = upper;

    /// <summary>
    ///     Creates a new permission set with the nth bit set.
    /// </summary>
    /// <remarks>
    ///     The <paramref name="setBit" /> parameter is 0-based, ex <c>new PermissionSet(0)</c> will set the 0th bit
    ///     (first) to <c>1</c>
    /// </remarks>
    /// <param name="setBit">The 0-based bit index to set.</param>
    public PermissionBitSet(byte setBit) :
        this(
            setBit is <= sizeof(ulong) << 3 and > 0
                ? 1UL << setBit
                : 0L,
            setBit is <= sizeof(ulong) << 4 and > sizeof(ulong) << 3
                ? 1UL << setBit
                : 0L
        )
    {
    }

    public bool Has(in PermissionBitSet other)
        => (this | other) == other;

    public unsafe bool IsSet(byte bit)
    {
        if (bit << 3 > sizeof(PermissionBitSet))
            throw new InvalidOperationException($"offset is outside the defined size of {nameof(PermissionBitSet)}");

        // find the byte that this offset is in
        var byteOffset = bit >> 3;
        var bitOffset = 1 << (bit % 8 - 1);

        // get this as a byte ref
        ref var bytes = ref Unsafe.As<PermissionBitSet, byte>(ref Unsafe.AsRef(in this));

        // get the byte containing the bit
        ref var setByte = ref Unsafe.Add(ref bytes, byteOffset);

        // return the bit mask of the offset
        return (setByte & bitOffset) > 0;
    }

    public static PermissionBitSet operator ~(PermissionBitSet bitSet)
        => new(~bitSet._lower, ~bitSet._upper);

    public static PermissionBitSet operator &(PermissionBitSet a, PermissionBitSet b)
        => new(a._lower & b._lower, a._upper & b._upper);

    public static PermissionBitSet operator |(PermissionBitSet a, PermissionBitSet b)
        => new(a._lower | b._lower, a._upper | b._upper);

    public static PermissionBitSet operator ^(PermissionBitSet a, PermissionBitSet b)
        => new(a._lower ^ b._lower, a._upper ^ b._upper);

    public bool Equals(PermissionBitSet other) => _lower == other._lower && _upper == other._upper;

    public override bool Equals(object? obj) => obj is PermissionBitSet other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_lower, _upper);

    public static bool operator ==(PermissionBitSet left, PermissionBitSet right) => left.Equals(right);

    public static bool operator !=(PermissionBitSet left, PermissionBitSet right) => !left.Equals(right);

#if NET7_0_OR_GREATER
    public static implicit operator Int128(PermissionBitSet bitSet)
        => new(bitSet._lower, bitSet._upper);

    public static implicit operator PermissionBitSet(Int128 num)
    {
        ref var numRef = ref Unsafe.As<Int128, ulong>(ref num);

        if (BitConverter.IsLittleEndian)
        {
            return new PermissionBitSet(
                numRef,
                Unsafe.Add(ref numRef, 1)
            );
        }
        else
        {
            return new PermissionBitSet(
                Unsafe.Add(ref numRef, 1),
                numRef
            );
        }
    }
#endif

    public static unsafe implicit operator BigInteger(PermissionBitSet bitSet)
        => new(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<PermissionBitSet, byte>(ref bitSet), sizeof(PermissionBitSet)));

    public static unsafe implicit operator PermissionBitSet(BigInteger set)
    {
        Span<byte> bytes = stackalloc byte[sizeof(PermissionBitSet)];

        if (!set.TryWriteBytes(bytes, out _))
            throw new ArgumentOutOfRangeException(nameof(set));


        ref var lower = ref Unsafe.As<byte, ulong>(ref bytes[0]); // 0..4
        ref var upper = ref Unsafe.As<byte, ulong>(ref bytes[sizeof(long)]); // 4..8

        return new PermissionBitSet(lower, upper);
    }

    public static implicit operator PermissionBitSet(string str)
    {
#if NET7_0_OR_GREATER
        return Int128.Parse(str);
#else
        return BigInteger.Parse(str);
#endif
    }

    public override string ToString()
    {
#if NET7_0_OR_GREATER
        return Unsafe.As<PermissionBitSet, Int128>(ref Unsafe.AsRef(in this)).ToString();
#else
        return ((BigInteger)this).ToString("D");
#endif
    }
}