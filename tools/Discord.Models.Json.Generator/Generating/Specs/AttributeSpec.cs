using System.Text;

namespace Discord.Models.Json.Generator.Specs;


public record AttributeSpec
{
    public string? Target { get; init; }
    public string Name { get; init; }
    public List<string> Arguments { get; init; }
    public List<(string Name, string Value)> NamedArguments { get; init; }

    public bool HasArguments => Arguments.Count > 0 || NamedArguments.Count > 0;
    
    public AttributeSpec(
        string name,
        IEnumerable<string>? arguments = null,
        IEnumerable<(string Name, string Value)>? namedArguments = null
    )
    {
        Name = name;
        Arguments = [..arguments ?? []];
        NamedArguments = [..namedArguments ?? []];
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (Target is not null)
            sb.Append(Target).Append(": ");

        sb.Append(Name);
        
        if (!HasArguments) return sb.ToString();

        foreach (var argument in Arguments)
        {
            sb.Append(argument).Append(", ");
        }

        foreach (var (name, value) in NamedArguments)
        {
            sb.Append(name).Append(" = ").Append(value).Append(", ");
        }
        
        sb.Length -= 2;

        return sb.Append(')').ToString();
    }
}