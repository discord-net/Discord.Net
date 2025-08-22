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

    public static implicit operator Optional<T>(T value) => new(value);

    public override string ToString()
        => IsSpecified ? $"Some({Value})" : "None";

    public T? ToNullable() => IsSpecified ? Value : default;
}