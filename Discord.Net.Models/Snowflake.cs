using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Discord.Models;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Snowflake(ulong value) :
    IBinaryInteger<Snowflake>,
    IMinMaxValue<Snowflake>,
    IUnsignedNumber<Snowflake>
{
    private readonly ulong _value = value;

    public static implicit operator ulong(Snowflake snowflake) => snowflake._value;
    public static implicit operator Snowflake(ulong snowflake) => new(snowflake);

    public override string ToString()
        => _value.ToString();

    public override bool Equals(object? obj)
    {
        return obj is Snowflake other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public int CompareTo(object? obj)
        => obj is Snowflake snowflake ? CompareTo(snowflake) : 1;

    public int CompareTo(Snowflake other)
        => _value.CompareTo(other._value);

    public bool Equals(Snowflake other)
        => _value == other._value;

    public string ToString(string? format, IFormatProvider? formatProvider)
        => _value.ToString(format, formatProvider);

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider
    ) => _value.TryFormat(destination, out charsWritten, format, provider);

    public static Snowflake Parse(string s, IFormatProvider? provider)
        => ulong.Parse(s, provider);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Snowflake result)
    {
        if (ulong.TryParse(s, provider, out var value))
        {
            result = value;
            return true;
        }

        result = default;
        return false;
    }

    public static Snowflake Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
        => ulong.Parse(s, provider);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Snowflake result)
    {
        if (ulong.TryParse(s, provider, out var value))
        {
            result = value;
            return true;
        }

        result = default;
        return false;
    }

    public static Snowflake operator +(Snowflake left, Snowflake right)
        => left._value + right._value;

    public static Snowflake AdditiveIdentity => Zero;

    public static Snowflake operator &(Snowflake left, Snowflake right)
        => left._value & right._value;

    public static Snowflake operator |(Snowflake left, Snowflake right)
        => left._value | right._value;

    public static Snowflake operator ^(Snowflake left, Snowflake right)
        => left._value ^ right._value;

    public static Snowflake operator ~(Snowflake value)
        => ~value._value;

    public static bool operator ==(Snowflake left, Snowflake right)
        => left._value == right._value;

    public static bool operator !=(Snowflake left, Snowflake right)
        => left._value != right._value;

    public static bool operator >(Snowflake left, Snowflake right)
        => left._value > right._value;

    public static bool operator >=(Snowflake left, Snowflake right)
        => left._value >= right._value;

    public static bool operator <(Snowflake left, Snowflake right)
        => left._value < right._value;

    public static bool operator <=(Snowflake left, Snowflake right)
        => left._value <= right._value;

    public static Snowflake operator --(Snowflake value)
        => value - One;

    public static Snowflake operator /(Snowflake left, Snowflake right)
        => left._value / right._value;

    public static Snowflake operator ++(Snowflake value)
        => value + One;

    public static Snowflake operator %(Snowflake left, Snowflake right)
        => left._value % right._value;

    public static Snowflake MultiplicativeIdentity => One;

    public static Snowflake operator *(Snowflake left, Snowflake right)
        => left._value * right._value;

    public static Snowflake operator -(Snowflake left, Snowflake right)
        => left._value - right._value;

    public static Snowflake operator -(Snowflake value)
        => Zero - value;

    public static Snowflake operator +(Snowflake value)
        => +value._value;

    public static Snowflake Abs(Snowflake value)
        => value;

    public static bool IsCanonical(Snowflake value) => true;

    public static bool IsComplexNumber(Snowflake value) => false;

    public static bool IsEvenInteger(Snowflake value) => (value & One) == Zero;

    public static bool IsFinite(Snowflake value) => true;

    public static bool IsImaginaryNumber(Snowflake value) => false;

    public static bool IsInfinity(Snowflake value) => false;

    public static bool IsInteger(Snowflake value) => true;

    public static bool IsNaN(Snowflake value) => false;

    public static bool IsNegative(Snowflake value) => false;

    public static bool IsNegativeInfinity(Snowflake value) => false;

    public static bool IsNormal(Snowflake value) => value != Zero;

    public static bool IsOddInteger(Snowflake value) => (value & One) != Zero;

    public static bool IsPositive(Snowflake value) => true;

    public static bool IsPositiveInfinity(Snowflake value) => false;

    public static bool IsRealNumber(Snowflake value) => true;

    public static bool IsSubnormal(Snowflake value) => false;

    public static bool IsZero(Snowflake value) => value == Zero;

    public static Snowflake MaxMagnitude(Snowflake x, Snowflake y) => ulong.Max(x, y);

    public static Snowflake MaxMagnitudeNumber(Snowflake x, Snowflake y) => ulong.Max(x, y);

    public static Snowflake MinMagnitude(Snowflake x, Snowflake y) => ulong.Min(x, y);

    public static Snowflake MinMagnitudeNumber(Snowflake x, Snowflake y) => ulong.Min(x, y);

    public static Snowflake Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
        => ulong.Parse(s, style, provider);

    public static Snowflake Parse(string s, NumberStyles style, IFormatProvider? provider)
        => ulong.Parse(s, style, provider);

    public static bool TryConvertFromChecked<TOther>(TOther value, out Snowflake result)
        where TOther : INumberBase<TOther>
    {
        if (Inner<ulong, TOther>(value, out var snowflake))
        {
            result = snowflake;
            return true;
        }

        result = default;
        return false;

        static bool Inner<T, TOther>(
            TOther value,
            [MaybeNullWhen(false)] out T result
        ) where T : INumberBase<T> where TOther : INumberBase<TOther>
            => T.TryConvertFromChecked(value, out result);
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, out Snowflake result)
        where TOther : INumberBase<TOther>
    {
        if (Inner<ulong, TOther>(value, out var snowflake))
        {
            result = snowflake;
            return true;
        }

        result = default;
        return false;

        static bool Inner<T, TOther>(
            TOther value,
            [MaybeNullWhen(false)] out T result
        ) where T : INumberBase<T> where TOther : INumberBase<TOther>
            => T.TryConvertFromSaturating(value, out result);
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, out Snowflake result)
        where TOther : INumberBase<TOther>
    {
        if (Inner<ulong, TOther>(value, out var snowflake))
        {
            result = snowflake;
            return true;
        }

        result = default;
        return false;

        static bool Inner<T, TOther>(
            TOther value,
            [MaybeNullWhen(false)] out T result
        ) where T : INumberBase<T> where TOther : INumberBase<TOther>
            => T.TryConvertFromTruncating(value, out result);
    }

    public static bool TryConvertToChecked<TOther>(Snowflake value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        return Inner<ulong, TOther>(value, out result);

        static bool Inner<T, TOther>(
            T value,
            [MaybeNullWhen(false)] out TOther result
        ) where T : INumberBase<T> where TOther : INumberBase<TOther>
            => T.TryConvertToChecked(value, out result);
    }

    public static bool TryConvertToSaturating<TOther>(Snowflake value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        return Inner<ulong, TOther>(value, out result);

        static bool Inner<T, TOther>(
            T value,
            [MaybeNullWhen(false)] out TOther result
        ) where T : INumberBase<T> where TOther : INumberBase<TOther>
            => T.TryConvertToSaturating(value, out result);
    }

    public static bool TryConvertToTruncating<TOther>(Snowflake value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        return Inner<ulong, TOther>(value, out result);

        static bool Inner<T, TOther>(
            T value,
            [MaybeNullWhen(false)] out TOther result
        ) where T : INumberBase<T> where TOther : INumberBase<TOther>
            => T.TryConvertToTruncating(value, out result);
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        NumberStyles style,
        IFormatProvider? provider,
        out Snowflake result
    )
    {
        if (ulong.TryParse(s, style, provider, out var snowflake))
        {
            result = snowflake;
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        NumberStyles style,
        IFormatProvider? provider,
        out Snowflake result
    )
    {
        if (ulong.TryParse(s, style, provider, out var snowflake))
        {
            result = snowflake;
            return true;
        }

        result = default;
        return false;
    }

    public static Snowflake One => 1;

    public static int Radix => 2;

    public static Snowflake Zero => 0;

    public static bool IsPow2(Snowflake value)
        => ulong.IsPow2(value);

    public static Snowflake Log2(Snowflake value)
        => ulong.Log2(value);

    public static Snowflake operator <<(Snowflake value, int shiftAmount)
        => value._value << shiftAmount;

    public static Snowflake operator >> (Snowflake value, int shiftAmount)
        => value._value >> shiftAmount;

    public static Snowflake operator >>> (Snowflake value, int shiftAmount)
        => value._value >>> shiftAmount;

    public int GetByteCount()
        => sizeof(ulong);

    public int GetShortestBitLength()
        => (sizeof(ulong) * 8) - BitOperations.LeadingZeroCount(_value);

    public static Snowflake PopCount(Snowflake value)
        => ulong.PopCount(value);

    public static Snowflake TrailingZeroCount(Snowflake value)
        => ulong.TrailingZeroCount(value);

    public static bool TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Snowflake value)
    {
        if (Inner<ulong>(source, isUnsigned, out var snowflake))
        {
            value = snowflake;
            return true;
        }

        value = default;
        return false;

        static bool Inner<T>(ReadOnlySpan<byte> source, bool isUnsigned, out T value)
            where T : IBinaryInteger<T>
            => T.TryReadBigEndian(source, isUnsigned, out value);
    }

    public static bool TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Snowflake value)
    {
        if (Inner<ulong>(source, isUnsigned, out var snowflake))
        {
            value = snowflake;
            return true;
        }

        value = default;
        return false;

        static bool Inner<T>(ReadOnlySpan<byte> source, bool isUnsigned, out T value)
            where T : IBinaryInteger<T>
            => T.TryReadLittleEndian(source, isUnsigned, out value);
    }

    public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
    {
        return Inner(_value, destination, out bytesWritten);

        static bool Inner<T>(T value, Span<byte> destination, out int bytesWritten)
            where T : IBinaryInteger<T>
            => value.TryWriteBigEndian(destination, out bytesWritten);
    }

    public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
    {
        return Inner(_value, destination, out bytesWritten);

        static bool Inner<T>(T value, Span<byte> destination, out int bytesWritten)
            where T : IBinaryInteger<T>
            => value.TryWriteLittleEndian(destination, out bytesWritten);
    }

    public static Snowflake MaxValue => ulong.MaxValue;

    public static Snowflake MinValue => ulong.MinValue;
}