using System.Diagnostics;

namespace Discord;

[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public readonly struct Optional<T>
{
    public static Optional<T> Unspecified => default;

    /// <summary>
    ///     Gets the value for this parameter.
    /// </summary>
    /// <exception cref="InvalidOperationException" accessor="get">This property has no value set.</exception>
    public T Value { get; }

    /// <summary>
    ///     Returns true if this value has been specified.
    /// </summary>
    public bool IsSpecified { get; }

    /// <summary>
    ///     Creates a new Parameter with the provided value.
    /// </summary>
    public Optional(T value)
    {
        Value = value;
        IsSpecified = true;
    }

    public T GetValueOrDefault() => Value!;
    public T GetValueOrDefault(T defaultValue) => IsSpecified ? Value : defaultValue;

    public override int GetHashCode() => IsSpecified ? Value?.GetHashCode() ?? 0 : 0;

    public override string? ToString() => IsSpecified ? Value?.ToString() : null;
    private string DebuggerDisplay => IsSpecified ? Value?.ToString() ?? "<null>" : "<unspecified>";

    public static implicit operator Optional<T>(T value) => new(value);
    public static explicit operator T(Optional<T> value) => value.Value;
}

public static class Optional
{
    public static Optional<T> Create<T>() => Optional<T>.Unspecified;
    public static Optional<T> Create<T>(T value) => new(value);

    public static T? ToNullable<T>(this Optional<T> val)
        where T : struct
        => val.IsSpecified ? val.Value : null;
}
