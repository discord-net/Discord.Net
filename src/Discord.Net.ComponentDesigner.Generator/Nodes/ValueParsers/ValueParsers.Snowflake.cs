using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using System;

namespace Discord.ComponentDesignerGenerator.Nodes;

partial class ValueParsers
{
    public static ComponentPropertyValue<ulong>? ParseSnowflakeProperty(ComponentProperty<ulong> property)
    {
        switch (property.Value)
        {
            case null: return null;

            case CXmlValue.Interpolation interpolation:
                return ValidateInterpolationType(property, interpolation, SpecialType.System_UInt64);
            case CXmlValue.Invalid: return null;
            case CXmlValue.Multipart multipart:
                return property.DangerousCreateCode(
                    $"ulong.Parse({ValueCodeGenerator.BuildValue(multipart, property.Context)})"
                );
            case CXmlValue.Scalar scalar:
                if (ulong.TryParse(scalar.Value, out var snowflake))
                {
                    return property.CreateValue(snowflake);
                }

                property.Context.ReportDiagnostic(
                    Diagnostics.InvalidSnowflakeIdentifier,
                    property.Context.GetLocation(scalar),
                    scalar.Value
                );

                return null;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
