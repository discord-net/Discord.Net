using Discord.CX.Parser;
using System.Collections.Generic;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.CX.Nodes.Components;

public sealed class InterleavedComponentNode : ComponentNode
{
    public override string Name => "<interpolated component>";

    public override ComponentState? Create(ICXNode source, List<CXNode> children)
    {
        if (source is not CXValue.Interpolation interpolation) return null;

        return base.Create(source, children);
    }

    public override string Render(ComponentState state, ComponentContext context)
        => context.GetDesignerValue(
            (CXValue.Interpolation)state.Source,
            context.KnownTypes.IMessageComponentBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
        );
}
