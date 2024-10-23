using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Discord.Net.Hanz.Utils.Bakery;

public readonly record struct FieldSpec(
    string Name,
    string Type,
    Accessibility Accessibility
)
{
    public ImmutableEquatableArray<string> Modifiers { get; init; } = ImmutableEquatableArray<string>.Empty;

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append(SyntaxFacts.GetText(Accessibility))
            .Append(' ');

        if (Modifiers.Count > 0)
            builder
                .Append(string.Join(" ", Modifiers))
                .Append(' ');

        builder.Append(Name).Append(';');

        return builder.ToString();
    }
}