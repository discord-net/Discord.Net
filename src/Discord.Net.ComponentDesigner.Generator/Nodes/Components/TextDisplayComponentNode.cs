using Discord.CX.Parser;
using System.Collections.Generic;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.CX.Nodes.Components;

public sealed class TextDisplayComponentNode : ComponentNode
{
    public override string Name => "text";

    public ComponentProperty Content { get; }
    public override IReadOnlyList<ComponentProperty> Properties { get; }

    public TextDisplayComponentNode()
    {
        Properties =
        [
            ComponentProperty.Id,
            Content = new(
                "content",
                renderer: Renderers.String
            )
        ];
    }

    public override ComponentState? Create(ICXNode source, List<CXNode> children)
    {
        var state = base.Create(source, children)!;

        if (source is CXElement {Children.Count: 1} element && element.Children[0] is CXValue value)
            state.SubstitutePropertyValue(Content, value);

        return state;
    }

    public override string Render(ComponentState state, ComponentContext context)
        => $"""
            new {context.KnownTypes.TextDisplayBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}({
                state.RenderProperties(this, context)
                    .WithNewlinePadding(4)
                    .PrefixIfSome(4)
                    .WrapIfSome("\n")
            })
            """;
}
