namespace Discord.Models.Json.Generator.Specs;

public enum VarianceKind
{
    None,
    Out,
    In
}

public sealed record GenericSpec(
    string Name,
    VarianceKind Variance = VarianceKind.None
)
{
    public override string ToString()
        => $"{(Variance is not VarianceKind.None ? $"{Variance.ToString().ToLower()} " : string.Empty)}{Name}";

    public static implicit operator GenericSpec(string str) => new(str);
    public static implicit operator GenericSpec((string, VarianceKind) tuple) => new(tuple.Item1, tuple.Item2);
}