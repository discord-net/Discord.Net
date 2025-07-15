using System.Diagnostics.CodeAnalysis;

namespace Discord;

public readonly struct Optional<T> :
    IEquatable<Optional<T>>,
    IComparable<Optional<T>>
{
    public static readonly Optional<T> None = default;
    
    public T Value 
        => IsSpecified ? _value : throw new InvalidOperationException("The value is not specified.");
    
    public bool IsSpecified { get; }

    private readonly T _value;
    
    public Optional(T value)
    {
        _value = value;
        IsSpecified = true;
    }

    public bool Equals(Optional<T> other)
    {
        if (!IsSpecified && !other.IsSpecified)
            return true;

        if (IsSpecified && other.IsSpecified)
            return EqualityComparer<T>.Default.Equals(_value, other._value);

        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Optional<T> other && Equals(other);

    public int CompareTo(Optional<T> other)
    {
        return (this, other) switch
        {
            ({IsSpecified: true}, {IsSpecified: false}) => 1,
            ({IsSpecified: false}, {IsSpecified: true}) => -1,
            _ => Comparer<T>.Default.Compare(_value, other._value)
        };
    }

    public override int GetHashCode()
        => HashCode.Combine(IsSpecified, _value);

    public override string ToString()
    {
        if (IsSpecified)
            return $"Some({(_value is null ? "null" : _value)})";

        return "None";
    }
    
    public static implicit operator Optional<T>(T value) => new(value);
    
    public static bool operator true(Optional<T> value) => value.IsSpecified;
    public static bool operator false(Optional<T> value) => !value.IsSpecified;
    
    public static bool operator ==(Optional<T> left, Optional<T> right) => left.Equals(right);
    public static bool operator !=(Optional<T> left, Optional<T> right) => !left.Equals(right);
    
    public static bool operator <(Optional<T> left, Optional<T> right) => left.CompareTo(right) < 0;
    public static bool operator <=(Optional<T> left, Optional<T> right) => left.CompareTo(right) <= 0;
    public static bool operator >(Optional<T> left, Optional<T> right) => left.CompareTo(right) > 0;
    public static bool operator >=(Optional<T> left, Optional<T> right) => left.CompareTo(right) >= 0;
}

public static class Optional
{
    public static Optional<T> As<T, U>(this Optional<U> value, T? defaultVal = default)
        where U : T
        => value.Map(x => (T)x);

    public static Optional<T> Some<T>(this T value) => new(value);
    public static Optional<T> SomeWhen<T>(this T value, Predicate<T> predicate)
        => predicate(value) ? Some(value) : default;
    
    public static Optional<T> None<T>(this T value) => default;
    public static Optional<T> NoneWhen<T>(this T value, Predicate<T> predicate)
        => !predicate(value) ? Some(value) : default;

    public static bool IsSome<T>(this Optional<T> optional, [MaybeNullWhen(false)] out T value)
    {
        if (optional.IsSpecified)
        {
            value = optional.Value;
            return true;
        }
        
        value = default;
        return false;
    }

    public static Optional<T> ToOptional<T>(this T? value)
        where T : struct
        => value is not null ? Some(value.Value) : default;
    
    public static Optional<T> ToOptional<T>(this T? value)
        where T : class
        => value is not null ? Some(value) : default;
    
    public static T? ToNullable<T>(this Optional<T> optional)
        where T : struct
        => optional.IsSome(out var val) ? val : null;

    public static T? AsNullable<T>(this Optional<T> optional)
        where T : class
        => optional.IsSome(out var val) ? val : null;
    
    public static T ValueOr<T>(this Optional<T> optional, T other)
        => optional.IsSome(out var val) ? val : other;
    
    public static T ValueOr<T>(this Optional<T> optional, Func<T> other)
        => optional.IsSome(out var val) ? val : other();

    public static Optional<T> Or<T>(this Optional<T> optional, T other)
        => optional.IsSpecified ? optional : other.Some();
    
    public static Optional<T> Or<T>(this Optional<T> optional, Func<T> other)
        => optional.IsSpecified ? optional : other().Some();

    public static Optional<T> Else<T>(this Optional<T> optional, Optional<T> other)
        => optional.IsSpecified ? optional : other;
    
    public static Optional<T> Else<T>(this Optional<T> optional, Func<Optional<T>> other)
        => optional.IsSpecified ? optional : other();
    
    public static TResult Match<T, TResult>(this Optional<T> optional, Func<T, TResult> some, Func<TResult> none)
        => optional.IsSpecified ? some(optional.Value) : none();

    public static void Match<T>(this Optional<T> optional, Action<T> some, Action none)
    {
        if (optional.IsSome(out var val))
            some(val);
        else none();
    }

    public static void MatchSome<T>(this Optional<T> optional, Action<T> some)
    {
        if (optional.IsSome(out var val)) some(val);
    }

    public static void MatchNone<T>(this Optional<T> optional, Action none)
    {
        if (!optional.IsSpecified) none();
    }

    public static Optional<U> Map<T, U>(this Optional<T> optional, Func<T, U> mapper)
        => optional.IsSome(out var val) ? mapper(val).Some() : default;

    public static Optional<U> FlatMap<T, U>(this Optional<T> optional, Func<T, Optional<U>> mapper)
        => optional.IsSome(out var val) ? mapper(val) : default;

    public static Optional<T> Filter<T>(this Optional<T> optional, bool condition)
        => optional.IsSpecified && !condition ? default : optional;

    public static Optional<T> Filter<T>(this Optional<T> optional, Func<T, bool> predicate)
        => optional.IsSome(out var val) && !predicate(val) ? default : optional;

    public static Optional<T> NotNull<T>(this Optional<T?> optional)
        where T : class
        => optional.IsSome(out var val) && val is not null ? val.Some() : default;
    
    public static Optional<T> NotNull<T>(this Optional<T?> optional)
        where T : struct
        => optional.IsSome(out var val) && val is not null ? val.Value.Some() : default;

    public static Optional<T> Flatten<T>(this Optional<Optional<T>> optional)
        => optional.FlatMap(x => x);
}