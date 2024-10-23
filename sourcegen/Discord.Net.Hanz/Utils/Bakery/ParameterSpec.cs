using System.Text;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Utils.Bakery;

public readonly record struct ParameterSpec(
    string Type,
    string Name,
    string? Default = null
)
{
    public static ParameterSpec From(IParameterSymbol symbol)
    {
        return new ParameterSpec(
            symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            symbol.Name,
            symbol.HasExplicitDefaultValue
                ? SyntaxUtils.FormatLiteral(
                    symbol.ExplicitDefaultValue,
                    symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                )
                : null
        );
    }

    public string ToString(bool includeDefault)
    {
        var builder = new StringBuilder()
            .Append(Type)
            .Append(' ')
            .Append(Name);

        if (includeDefault && Default is not null)
            builder.Append(" = ").Append(Default);

        return builder.ToString();
    }

    public override string ToString() => ToString(true);

    public static implicit operator ParameterSpec((string, string) tuple) => new(tuple.Item1, tuple.Item2);

    public static implicit operator ParameterSpec((string, string, string) tuple) =>
        new(tuple.Item1, tuple.Item2, tuple.Item3);
}