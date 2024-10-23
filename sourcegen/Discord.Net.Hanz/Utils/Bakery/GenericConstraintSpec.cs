namespace Discord.Net.Hanz.Utils.Bakery;

public readonly record struct GenericConstraintSpec(
    string Name
)
{
    public ImmutableEquatableArray<string> Constraints { get; init; } = ImmutableEquatableArray<string>.Empty;

    public override string ToString()
    {
        if (Constraints.Count == 0)
            return string.Empty;

        return $"where {Name} : {string.Join(", ", Constraints)}";
    }

    public static implicit operator GenericConstraintSpec((string, string[]) tuple) => new(tuple.Item1)
    {
        Constraints = new(tuple.Item2)
    };
}