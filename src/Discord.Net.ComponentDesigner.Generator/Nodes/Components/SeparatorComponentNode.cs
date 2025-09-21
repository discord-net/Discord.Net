using System.Collections.Generic;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.CX.Nodes.Components;

public sealed class SeparatorComponentNode : ComponentNode
{
    public const string SEPARATOR_SPACING_QUALIFIED_NAME = "Discord.SeparatorSpacingSize";

    public override string Name => "separator";

    public ComponentProperty Id { get; }
    public ComponentProperty Divider { get; }
    public ComponentProperty Spacing { get; }

    public override IReadOnlyList<ComponentProperty> Properties { get; }

    public SeparatorComponentNode()
    {
        Properties =
        [
            Id = ComponentProperty.Id,
            Divider = new(
                "divider",
                isOptional: true,
                renderer: Renderers.Boolean
            ),
            Spacing = new(
                "spacing",
                isOptional: true,
                renderer: Renderers.RenderEnum(SEPARATOR_SPACING_QUALIFIED_NAME)
            )
        ];
    }

    public override string Render(ComponentState state, ComponentContext context)
        => $"""
            new {context.KnownTypes.SeparatorBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}({
                state.RenderProperties(this, context)
                    .WithNewlinePadding(4)
                    .PrefixIfSome(4)
                    .WrapIfSome("\n")
            })
            """;
}
