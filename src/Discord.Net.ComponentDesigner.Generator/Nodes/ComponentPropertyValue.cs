using Discord.CX.Parser;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Discord.CX.Nodes;

public sealed record ComponentPropertyValue(
    ComponentProperty Property,
    CXAttribute? Attribute
)
{
    private CXValue? _value;

    public CXValue? Value
    {
        get => _value ??= Attribute?.Value;
        init => _value = value;
    }

    public bool IsSpecified => Attribute is not null;

    public bool HasValue => Value is not null;

    private readonly List<Diagnostic> _diagnostics = [];

    public void AddDiagnostic(Diagnostic diagnostic) => _diagnostics.Add(diagnostic);

    public bool TryGetLiteralValue(ComponentContext context, out string value)
    {
        switch (Value)
        {
            case CXValue.Scalar scalar:
                value = scalar.Value;
                return true;
            case CXValue.StringLiteral {HasInterpolations: false} literal:
                value = literal.Tokens.ToString();
                return true;
            // case CXValue.Interpolation interpolation:
            //     var info = context.GetInterpolationInfo(interpolation);
            //
            //     break;

            default:
                value = string.Empty;
                return false;
        }
    }
}
