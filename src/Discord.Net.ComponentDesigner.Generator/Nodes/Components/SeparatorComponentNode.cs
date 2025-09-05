using Discord.ComponentDesigner.Generator.Parser;
using System.Xml;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class SeparatorComponentNode : ComponentNode
{
    public override string FriendlyName => "Separator";
    public override NodeKind Kind => NodeKind.Separator;
    public ComponentProperty<bool> IsDivider { get; }

    public ComponentProperty<SeparatorSpacing> Spacing { get; }

    public SeparatorComponentNode(CXmlElement xml, ComponentNodeContext context) : base(xml, context)
    {
        IsDivider = MapProperty<bool>(
            "divider",
            ParseBooleanProperty,
            optional: true,
            defaultValue: true
        );

        Spacing = MapProperty<SeparatorSpacing>(
            "spacing",
            ParseEnumProperty<SeparatorSpacing>,
            optional: true,
            defaultValue: SeparatorSpacing.Small
        );
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.SeparatorBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                isDivider: {IsDivider},
                spacing: {Context.KnownTypes.SeparatorSpacingSizeType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{Spacing}
            )
            """;
}

public enum SeparatorSpacing
{
    Small = 1,
    Large = 2
}
