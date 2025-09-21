using Discord.CX.Parser;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Discord.CX.Nodes;

public static class Validators
{

    public static void Snowflake(ComponentContext context, ComponentPropertyValue propertyValue)
    {
        switch (propertyValue.Value)
        {
            case null or CXValue.Invalid: return;

            case CXValue.Scalar scalar:
                if (!ulong.TryParse(scalar.Value, out _))
                {
                    context.AddDiagnostic(
                        Diagnostics.TypeMismatch,
                        scalar,
                        scalar.Value,
                        "Snowflake"
                    );
                }

                return;
            case CXValue.Interpolation interpolation:
                var symbol = context.GetInterpolationInfo(interpolation).Symbol;

                if (
                    symbol?.SpecialType is not SpecialType.System_UInt64
                )
                {
                    context.AddDiagnostic(
                        Diagnostics.TypeMismatch,
                        interpolation,
                        symbol,
                        "Snowflake"
                    );
                }

                return;
        }
    }

    public static void Emote(ComponentContext context, ComponentPropertyValue propertyValue)
    {
        switch (propertyValue.Value)
        {
            case null or CXValue.Invalid: return;

            case CXValue.Scalar scalar:

                return;
        }
    }

    public static PropertyValidator Range(int? lower = null, int? upper = null)
    {
        Debug.Assert(lower.HasValue || upper.HasValue);

        var bounds = (lower, upper) switch
        {
            (not null, null) => $"at least {lower}",
            (null, not null) => $"at most {lower}",
            (not null, not null) => $"between {lower} and {upper}",
            _ => string.Empty
        };

        return (context, propertyValue) =>
        {
            switch (propertyValue.Value)
            {
                case null or CXValue.Invalid: return;
                case CXValue.Interpolation interpolation:
                    if (context.GetInterpolationInfo(interpolation).Constant.Value is string constantValue)
                        Check(constantValue.Length);
                    break;

                case CXValue.StringLiteral literal:
                    int? length = null;

                    foreach (var token in literal.Tokens)
                    {
                        switch (token.Kind)
                        {
                            case CXTokenKind.Text:
                                length += token.Span.Length;
                                break;
                            case CXTokenKind.Interpolation
                                when literal.Document.TryGetInterpolationIndex(token, out var index):
                                var info = context.GetInterpolationInfo(index);
                                if (info.Constant.Value is string str)
                                    length += str.Length;
                                break;
                        }
                    }

                    if (length.HasValue) Check(length.Value);

                    break;
                case CXValue.Scalar scalar:
                    Check(scalar.Value.Length);

                    return;
            }

            void Check(int length)
            {
                if (
                    length > upper || length < lower
                )
                {
                    context.AddDiagnostic(Diagnostics.OutOfRange, propertyValue.Value, propertyValue.Property.Name,
                        bounds);
                }
            }
        };
    }

    public static PropertyValidator EnumVariant(string fullyQualifiedName)
    {
        ITypeSymbol? symbol = null;
        IFieldSymbol[]? variants = null;

        return (context, propertyValue) =>
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

            switch (propertyValue.Value)
            {
                case null or CXValue.Invalid: return;

                case CXValue.Scalar scalar:
                    if (variants.All(x =>
                            !string.Equals(x.Name, scalar.Value, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        context.AddDiagnostic(
                            Diagnostics.InvalidEnumVariant,
                            scalar,
                            scalar.Value,
                            string.Join(", ", variants.Select(x => x.Name))
                        );
                    }

                    return;
                case CXValue.Interpolation interpolation:
                    // verify the value is the correct type
                    var interpolationInfo = context.GetInterpolationInfo(interpolation);

                    if (
                        interpolationInfo.Symbol is not null &&
                        !symbol.Equals(interpolationInfo.Symbol, SymbolEqualityComparer.Default)
                    )
                    {
                        context.AddDiagnostic(
                            Diagnostics.TypeMismatch,
                            interpolation,
                            interpolationInfo.Symbol,
                            symbol
                        );
                    }

                    return;
            }
        };
    }
}
