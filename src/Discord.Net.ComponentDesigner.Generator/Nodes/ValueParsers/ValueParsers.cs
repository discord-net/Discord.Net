using Discord.ComponentDesigner.Generator.Parser;
using Microsoft.CodeAnalysis;
using System;

namespace Discord.ComponentDesigner.Generator.Nodes;

public static partial class ValueParsers
{
    private static ComponentPropertyValue<T>? ValidateInterpolationType<T>(
        ComponentProperty<T> property,
        CXmlValue.Interpolation value,
        Func<ITypeSymbol, bool> validator
    )
    {
        var interpolationInfo = property.Context.Interpolations[value.InterpolationIndex];

        if (!validator(interpolationInfo.Type))
            return null;

        return property.CreateValue(in interpolationInfo);
    }

    private static ComponentPropertyValue<T>? ValidateInterpolationType<T>(
        ComponentProperty<T> property,
        CXmlValue.Interpolation value,
        SpecialType specialType
    ) => ValidateInterpolationType<T>(
        property,
        value,
        (symbol) =>
        {
            if (symbol.SpecialType != specialType)
            {
                property.Context.ReportDiagnostic(
                    Diagnostics.PropertyMismatch,
                    property.Context.GetLocation(value),
                    property.Name,
                    nameof(Boolean),
                    symbol.ToDisplayString()
                );
                return false;
            }

            return true;
        }
    );
}
