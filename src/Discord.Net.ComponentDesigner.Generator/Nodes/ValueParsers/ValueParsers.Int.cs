using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using System;

namespace Discord.ComponentDesignerGenerator.Nodes;

partial class ValueParsers
{
    public static ComponentPropertyValue<int>? ParseIntProperty(ComponentProperty<int> property)
    {
        switch (property.Value)
        {
            case null or CXmlValue.Invalid: return null;

            case CXmlValue.Interpolation interpolation:
                return ValidateInterpolationType(property, interpolation, SpecialType.System_Int32);

            case CXmlValue.Multipart multipart:
                // we'll use int.Parse
                return property.DangerousCreateCode(
                    $"int.Parse({ValueCodeGenerator.BuildValue(multipart, property.Context)})"
                );
            case CXmlValue.Scalar scalar:
                if (int.TryParse(scalar.Value, out var result))
                    return property.CreateValue(result);

                property.Node.Context.ReportDiagnostic(
                    Diagnostics.InvalidPropertyValue,
                    property.Node.Context.GetLocation(scalar),
                    scalar.Value,
                    nameof(Int32)
                );
                return null;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
