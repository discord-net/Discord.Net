namespace Discord.Models.Json.Generator.Specs;

public sealed record GenericConstraintSpec(
    string Name,
    IReadOnlyCollection<string>? Constraints = null
)
{
    public IReadOnlyCollection<string> Constraints { get; init; } 
        = Constraints ?? [];
    
    public override string ToString()
    {
        if (Constraints.Count == 0)
            return string.Empty;

        return $"where {Name} : {string.Join(", ", Constraints)}";
    }

    public static implicit operator GenericConstraintSpec((string, string[]) tuple) => new(tuple.Item1)
    {
        Constraints = tuple.Item2
    };
}