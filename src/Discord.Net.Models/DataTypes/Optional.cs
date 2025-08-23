namespace Discord.Models;

public readonly struct Optional<T>
{
    public T Value
        => IsSpecified ? field : throw new InvalidOperationException("Optional isn't specified");
    
    public bool IsSpecified { get; }

    public Optional(T value)
    {
        Value = value;
        IsSpecified = true;
    }

    public Optional<U> Map<U>(Func<T, U> mapper)
        => IsSpecified ? mapper(Value) : default!;
    
    public static implicit operator Optional<T>(T value) => new(value);

    public override string ToString()
        => IsSpecified ? $"Some({Value})" : "None";

    public T? ToNullable() => IsSpecified ? Value : default;

    public static T? operator |(Optional<T> left, T right)
        => left.IsSpecified ? left.Value : right;
}

public static class OptionalExtensions
{
    public static Optional<T> Unwrap<T>(this Optional<T?> optional)
        where T : struct
        => optional is {IsSpecified: true, Value: not null} ? optional.Value.Value : default;
}