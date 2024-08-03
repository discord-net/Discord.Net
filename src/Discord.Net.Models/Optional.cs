using Discord.Converters;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Discord;

//Based on https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Nullable.cs
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
[JsonConverter(typeof(OptionalConverter))]
public readonly struct Optional<T>
{
    public static readonly Optional<T> Unspecified = default;
    private readonly T _value;

    /// <summary> Gets the value for this parameter. </summary>
    /// <exception cref="InvalidOperationException" accessor="get">This property has no value set.</exception>
    public T Value
    {
        get
        {
            if (!IsSpecified)
                throw new InvalidOperationException("This property has no value set.");
            return _value;
        }
    }
    /// <summary> Returns true if this value has been specified. </summary>
    public bool IsSpecified { get; }

    /// <summary> Creates a new Parameter with the provided value. </summary>
    public Optional(T value)
    {
        _value = value;
        IsSpecified = true;
    }

    public T? GetValueOrDefault() => _value;

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public T? GetValueOrDefault(T? defaultValue) => IsSpecified ? _value : defaultValue;

    public T Or(T other)
        => IsSpecified ? _value ?? other : other;

    public Optional<T> Or(Optional<T> other)
        => IsSpecified ? this : other;

    public override bool Equals(object? other)
    {
        if(other is Optional<T> otherOptional)
        {
            return IsSpecified
                ? otherOptional.IsSpecified &&
                    (_value?.Equals(otherOptional._value) ?? otherOptional._value is null)
                : !otherOptional.IsSpecified;
        }

        if (!IsSpecified)
            return other == null;
        if (other is null)
            return _value is null;
        return _value?.Equals(other) ?? false;
    }
    public override int GetHashCode() => IsSpecified ? _value?.GetHashCode() ?? 0 : 0;
    public override string? ToString() => IsSpecified ? _value?.ToString() : null;

    public U? Map<U>(U value)
        where U : struct
        => IsSpecified ? value : null;

    public Optional<U> Map<U>(Func<T, U> func)
    {
        return IsSpecified ? Optional.Some(func(_value)) : Optional<U>.Unspecified;
    }

    public Optional<U> Map<U>(Func<T, Optional<U>> func)
    {
        return IsSpecified ? func(_value) : Optional<U>.Unspecified;
    }

    private string DebuggerDisplay => IsSpecified ? _value?.ToString() ?? "<null>" : "<unspecified>";

    public static explicit operator T(Optional<T> value) => value.Value;
    public static implicit operator Optional<T>(T value) => new(value);

    [return: NotNullIfNotNull(nameof(other))]
    public static T operator |(Optional<T> value, T other) => value.Or(other);

    public static Optional<T> operator |(Optional<T> value, Optional<T> other) => value.Or(other);

    public static T? operator ~(Optional<T> value) => value.GetValueOrDefault();

    public static bool operator ==(Optional<T> left, Optional<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Optional<T> left, Optional<T> right)
    {
        return !(left == right);
    }

    public static bool operator true(Optional<T> value) => value.IsSpecified;
    public static bool operator false(Optional<T> value) => !value.IsSpecified;
}
public static class Optional
{
    public static Optional<T> FromNullable<T>(T? value)
        where T : struct
        => value.HasValue ? Some(value.Value) : Optional<T>.Unspecified;
    public static Optional<T> FromNullable<T>(T? value)
        where T : class
        => value is null ? Optional<T>.Unspecified : Some<T>(value);
    public static Optional<T> Some<T>() => Optional<T>.Unspecified;
    public static Optional<T> Some<T>(T value) => new(value);

    public static T? ToNullable<T>(this Optional<T> val)
        where T : struct
        => val.IsSpecified ? val.Value : null;
}
