using Discord.ComponentDesigner.Generator.Parser;
using Microsoft.CodeAnalysis;
using System;

namespace Discord.ComponentDesigner.Generator.Nodes;

partial class ValueParsers
{
    public static ComponentPropertyValue<bool>? ParseBooleanProperty(ComponentProperty<bool> property)
    {
        if (property is {IsSpecified: true, Value: null})
            return property.CreateValue(true);

        if (!property.IsSpecified)
            return property.CreateValue(false);

        switch (property.Value)
        {
            case null or CXmlValue.Invalid: return null;

            case CXmlValue.Interpolation interpolation:
                return ValidateInterpolationType(property, interpolation, SpecialType.System_Boolean);

            // multi-parts are strings
            case CXmlValue.Multipart multipart:
                property.Context.ReportDiagnostic(
                    Diagnostics.PropertyMismatch,
                    property.Context.GetLocation(multipart),
                    property.Name,
                    nameof(Boolean),
                    typeof(string)
                );
                return null;
            case CXmlValue.Scalar scalar:
                var str = scalar.Value.ToLowerInvariant();

                if (str is not "true" and not "false")
                {
                    property.Context.ReportDiagnostic(
                        Diagnostics.PropertyMismatch,
                        property.Context.GetLocation(scalar),
                        property.Name,
                        nameof(Boolean),
                        typeof(string)
                    );
                    return null;
                }

                return property.CreateValue(str is "true");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
