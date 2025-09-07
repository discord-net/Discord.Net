using Discord.ComponentDesignerGenerator.Parser;

namespace Discord.ComponentDesignerGenerator.Nodes;

public abstract record ComponentPropertyValue<T>(ComponentProperty<T> Property)
{
    public sealed record Serializable(ComponentProperty<T> Property, T Value) : ComponentPropertyValue<T>(Property);

    public sealed record InlineCode(ComponentProperty<T> Property, string Code) : ComponentPropertyValue<T>(Property);

    public sealed record Interpolated(
        ComponentProperty<T> Property,
        int InterpolationId
    ) : ComponentPropertyValue<T>(Property);

    public sealed record MultiPartInterpolation(
        ComponentProperty<T> Property,
        CXmlValue.Multipart Multipart
    ) : ComponentPropertyValue<T>(Property);
}
