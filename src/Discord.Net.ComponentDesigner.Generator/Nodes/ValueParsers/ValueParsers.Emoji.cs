using Discord.ComponentDesigner.Generator.Parser;
using Microsoft.CodeAnalysis;
using System;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesigner.Generator.Nodes;

partial class ValueParsers
{
    public static ComponentPropertyValue<string>? ParseEmojiProperty(ComponentProperty<string> property)
    {
        switch (property.Value)
        {
            case null or CXmlValue.Invalid: return null;

            case CXmlValue.Scalar or CXmlValue.Multipart:
                return CreateDiscordParserCode(ValueCodeGenerator.BuildValue(property.Value, property.Context));

            case CXmlValue.Interpolation interpolation:
                var interpolationInfo = property.Context.Interpolations[interpolation.InterpolationIndex];

                if (
                    property.Context.Compilation.HasImplicitConversion(
                        interpolationInfo.Type,
                        property.Context.KnownTypes.IEmoteType
                    )
                )
                {
                    return property.CreateValue(in interpolationInfo);
                }

                // if its a string interpolation, do the same parse
                if (interpolationInfo.Type.SpecialType is SpecialType.System_String)
                {
                    return CreateDiscordParserCode(
                        $"designer.GetValueAsString({interpolationInfo.Id})"
                    );
                }

                // otherwise, unknown way to parse it
                property.Context.ReportDiagnostic(
                    Diagnostics.PropertyMismatch,
                    property.Context.GetLocation(interpolation),
                    property.Name,
                    property.Context.KnownTypes.IEmoteType!.ToDisplayString(),
                    interpolationInfo.Type.ToDisplayString()
                );
                return null;

            default:
                throw new ArgumentOutOfRangeException();
        }

        ComponentPropertyValue<string>? CreateDiscordParserCode(string? value)
        {
            if (value is null) return null;

            var emoteType =
                property.Context.KnownTypes.EmoteType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var emojiType =
                property.Context.KnownTypes.EmojiType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            return property.DangerousCreateCode(
                $"""
                 {emoteType}.TryParse({value}, out var emote)
                     ? emote
                     : {emojiType}.Parse({value})
                 """
            );
        }
    }
}
