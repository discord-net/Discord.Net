using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Nodes;

partial class ValueParsers
{
    public static ComponentPropertyValue<string>? ParseColorProperty(ComponentProperty<string> property)
    {
        var colorTypeName =
            property.Context.KnownTypes.ColorType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        switch (property.Value)
        {
            case null or CXmlValue.Invalid: return null;

            case CXmlValue.Scalar scalar:
                // check for field name first
                var known = property.Context.KnownTypes.ColorType!
                    .GetMembers()
                    .OfType<IFieldSymbol>()
                    .FirstOrDefault(x => x.Name == scalar.Value);

                if (known is not null)
                    return property.DangerousCreateCode(
                        $"{colorTypeName}.{known.Name}"
                    );

                return CreateParsedColor(ValueCodeGenerator.BuildValue(scalar, property.Context));

            case CXmlValue.Interpolation interpolation:
                var interpolationInfo = property.Context.Interpolations[interpolation.InterpolationIndex];

                if (
                    interpolationInfo.Type.Equals(
                        property.Context.KnownTypes.ColorType,
                        SymbolEqualityComparer.Default
                    )
                )
                {
                    return property.CreateValue(in interpolationInfo);
                }

                if (interpolationInfo.Type.SpecialType is SpecialType.System_String)
                {
                    return CreateParsedColor($"designer.GetValueAsString({interpolationInfo.Id})");
                }

                property.Context.ReportDiagnostic(
                    Diagnostics.PropertyMismatch,
                    property.Context.GetLocation(interpolation),
                    property.Name,
                    property.Context.KnownTypes.ColorType.ToDisplayString(),
                    interpolationInfo.Type.ToDisplayString()
                );
                return null;
            case CXmlValue.Multipart multipart:
                return CreateParsedColor(ValueCodeGenerator.BuildValue(multipart, property.Context));

            default: throw new ArgumentOutOfRangeException();
        }

        ComponentPropertyValue<string>? CreateParsedColor(string? value)
            => value is null ? null : property.DangerousCreateCode($"{colorTypeName}.Parse({value})");
    }
}
