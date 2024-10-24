using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Utils.Bakery;

public readonly record struct GenericConstraintSpec(
    string Name,
    ImmutableEquatableArray<string>? Constraints = null
)
{
    public ImmutableEquatableArray<string> Constraints { get; init; } 
        = Constraints ?? ImmutableEquatableArray<string>.Empty;

    public static GenericConstraintSpec From(ITypeParameterSymbol parameter)
    {
        var constraints = new List<string>();
        
        if(parameter.HasReferenceTypeConstraint)
            constraints.Add("class");
        else if(parameter.HasValueTypeConstraint)
            constraints.Add("struct");
        else if(parameter.HasUnmanagedTypeConstraint)
            constraints.Add("unmanaged");
        
        if(parameter.HasNotNullConstraint)
            constraints.Add("notnull");
        
        if(parameter.HasConstructorConstraint)
            constraints.Add("new()");

        constraints.AddRange(parameter.ConstraintTypes.Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

        if (constraints.Count == 0) return default;

        return new(parameter.Name, new(constraints));
    }
    
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