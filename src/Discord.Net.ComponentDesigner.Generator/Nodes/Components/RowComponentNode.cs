using Discord.ComponentDesignerGenerator.Parser;
using System.Collections.Generic;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesignerGenerator.Nodes.Components;

public sealed class RowComponentNode : ComponentNode
{
    public override string Name => "row";

    public override ComponentState? Create(ICXNode source, List<CXNode> children)
    {
        if (source is not CXElement element) return null;

        children.AddRange(element.Children);

        return base.Create(source, children);
    }

    public override string Render(ComponentState state, ComponentContext context)
        => $"""
            new {context.KnownTypes.ActionRowBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                {state.RenderChildren(context).WithNewlinePadding(4)}
            )
            """;
}
