using System.Text;

namespace Discord.Models.Json.Generator.Specs;

public sealed record FieldSpec(
    string Name,
    string Type,
    Accessibility Accessibility = Accessibility.Public,
    IReadOnlyCollection<string>? Modifiers = null,
    string? Initializer = null
)
{
    public IReadOnlyCollection<string> Modifiers { get; init; } = Modifiers ?? [];

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append(Accessibility.ToKeywords())
            .Append(' ');

        if (Modifiers.Count > 0)
            builder
                .Append(string.Join(" ", Modifiers))
                .Append(' ');

        builder.Append(Type).Append(' ').Append(Name);

        if (Initializer is not null)
            builder.Append(" = ").Append(Initializer);

        builder.Append(';');

        return builder.ToString();
    }
}