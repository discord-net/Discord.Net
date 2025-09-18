using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Text;

namespace Discord.ComponentDesignerGenerator.Nodes;

public static class Renderers
{
    public static PropertyRenderer CreateDefault(ComponentProperty property)
    {
        return (context, value) =>
        {
            return string.Empty;
        };
    }

    public static string String(ComponentContext context, ComponentPropertyValue propertyValue)
    {
        switch (propertyValue.Value)
        {
            default:
            case null or CXValue.Invalid: return "string.Empty";

            case CXValue.StringLiteral literal:
            {
                var sb = new StringBuilder();
                //var value = scalar.Value;

                var parts = literal.Tokens
                    .Where(x => x.Kind is CXTokenKind.Text)
                    .Select(x => x.Value)
                    .ToArray();

                var quoteCount = parts.Select(x => x.Count(x => x is '"')).Max() + 1;

                var dollars = new string(
                    '$',
                    parts.Select(GetInterpolationDollarRequirement).Max() +
                    (
                        literal.Tokens.Any(x => x.Kind is CXTokenKind.Interpolation)
                            ? 1
                            : 0
                    )
                );

                var startInterpolation = dollars.Length > 0
                    ? new string('{', dollars.Length)
                    : string.Empty;

                var endInterpolation = dollars.Length > 0
                    ? new string('}', dollars.Length)
                    : string.Empty;

                var isMultiline = parts.Any(x => x.Contains('\n'));

                if (isMultiline)
                {
                    sb.AppendLine();
                    quoteCount = Math.Max(quoteCount, 3);
                }

                var quotes = new string('"', quoteCount);

                sb.Append(dollars).Append(quotes);

                if (isMultiline) sb.AppendLine();

                foreach (var token in literal.Tokens)
                {
                    switch (token.Kind)
                    {
                        case CXTokenKind.Text:
                            sb.Append(token.Value);
                            break;
                        case CXTokenKind.Interpolation:
                            var index = Array.IndexOf(literal.Document.InterpolationTokens, token);

                            // TODO: handle better
                            if (index is -1) throw new InvalidOperationException();

                            sb.Append(startInterpolation).Append($"designer.GetValueAsString({index})")
                                .Append(endInterpolation);
                            break;

                        default: continue;
                    }
                }

                if (isMultiline) sb.AppendLine();
                sb.Append(quotes);

                return sb.ToString();
            }
            case CXValue.Scalar scalar:
            {
                var sb = new StringBuilder();
                var value = scalar.Value;

                var quoteCount = value.Count(x => x is '"') + 1;

                var isMultiline = value.Contains('\n');

                if (isMultiline)
                {
                    sb.AppendLine();
                    quoteCount = Math.Max(quoteCount, 3);
                }

                var quotes = new string('"', quoteCount);

                sb.Append(quotes);

                if (isMultiline) sb.AppendLine();

                sb.Append(value);

                if (isMultiline) sb.AppendLine();
                sb.Append(quotes);

                return sb.ToString();
            }
        }

        static int GetInterpolationDollarRequirement(string part)
        {
            var result = 0;

            var count = 0;
            char? last = null;

            foreach (var ch in part)
            {
                if (ch is '{' or '}')
                {
                    if (last is null)
                    {
                        last = ch;
                        count = 1;
                        continue;
                    }

                    if (last == ch)
                    {
                        count++;
                        continue;
                    }
                }

                if (count > 0)
                {
                    result = Math.Max(result, count);
                    last = null;
                    count = 0;
                }
            }

            return result;
        }
    }


    public static PropertyRenderer RenderEnum(string fullyQualifiedName)
    {
        ITypeSymbol? symbol = null;
        IFieldSymbol[]? variants = null;

        return (context, value) =>
        {
            if (symbol is null || variants is null)
            {
                symbol = context.Compilation.GetTypeByMetadataName(fullyQualifiedName);

                if (symbol is null) throw new InvalidOperationException($"Unknown type '{fullyQualifiedName}'");

                if (symbol.TypeKind is not TypeKind.Enum)
                    throw new InvalidOperationException($"'{symbol}' is not an enum type.");

                variants = symbol
                    .GetMembers()
                    .OfType<IFieldSymbol>()
                    .ToArray();
            }

            return string.Empty;
        };
    }
}
