using Discord.ComponentDesigner.Generator.Parser;
using System;

namespace Discord.ComponentDesigner.Generator.Nodes;

partial class ValueParsers
{
    public static ComponentPropertyValue<string>? ParseStringProperty(ComponentProperty<string> property)
    {
        switch (property.Value)
        {
            case CXmlValue.Invalid or null: return null;

            case CXmlValue.Interpolation interpolation:
                // any type automatically gets a .ToString() call, so we don't even have to check this
                return property.CreateValue(interpolation);

            case CXmlValue.Multipart multipart:
                return property.CreateValue(multipart);

            case CXmlValue.Scalar scalar:
                return property.CreateValue(scalar.Value);

            default:
                throw new ArgumentOutOfRangeException(nameof(property.Value));
        }
    }
}
