using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using System;

namespace Discord.ComponentDesignerGenerator.Nodes;

partial class ValueParsers
{
    public static ComponentPropertyValue<T>? ParseEnumProperty<T>(ComponentProperty<T> property) where T : struct
    {
        switch (property.Value)
        {
            case CXmlValue.Invalid or null: return null;

            case CXmlValue.Interpolation interpolation:
                var interpolationInfo = property.Context.Interpolations[interpolation.InterpolationIndex];

                if (property.ApiType is not null)
                {
                    if (property.ApiType.Equals(interpolationInfo.Type, SymbolEqualityComparer.Default))
                    {
                        return property.CreateValue(in interpolationInfo);
                    }

                    // we'll use the parse method
                    return property.DangerousCreateCode(
                        $"Enum.Parse<{property.ApiType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>(designer.GetValueAsString({interpolationInfo.Id}))"
                    );
                }

                property.Context.ReportDiagnostic(
                    Diagnostics.InvalidPropertyValue,
                    property.Context.GetLocation(interpolation),
                    interpolationInfo.Type.ToDisplayString(),
                    property.Name
                );
                return null;

            case CXmlValue.Multipart multipart:
                property.Context.ReportDiagnostic(
                    Diagnostics.InvalidPropertyValue,
                    property.Context.GetLocation(multipart),
                    "<multipart strings>",
                    property.Name
                );
                return null;
            case CXmlValue.Scalar scalar:
            {
                if (Enum.TryParse<T>(scalar.Value, out var result))
                    return property.CreateValue(result);

                property.Context.ReportDiagnostic(
                    Diagnostics.InvalidEnumProperty,
                    property.Context.GetLocation(scalar),
                    scalar.Value,
                    property.Name,
                    string.Join(", ", Enum.GetNames(typeof(T)))
                );

                return null;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(property.Value));
        }
    }
}
