using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesignerGenerator.Nodes.Components;

public sealed class ActionRowComponentNode : ComponentNode
{
    public override string Name => "row";

    public override bool HasChildren => true;

    public override IReadOnlyList<ComponentProperty> Properties { get; } = [ComponentProperty.Id];

    public override void Validate(ComponentState state, ComponentContext context)
    {
        if (!state.HasChildren)
        {
            context.AddDiagnostic(
                Diagnostics.EmptyActionRow,
                state.Source
            );

            base.Validate(state, context);
            return;
        }

        for (var i = 0; i < state.Children.Count; i++)
        {
            var child = state.Children[i];
            // TODO: validate children types

        }

        base.Validate(state, context);
    }

    public override string Render(ComponentState state, ComponentContext context)
        => $$"""
            new {{context.KnownTypes.ActionRowBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}}{{
                $"{
                    state
                        .RenderProperties(this, context, asInitializers: true)
                        .PostfixIfSome("\n")
                }{
                    state.RenderChildren(context)
                        .Map(x =>
                            $"""
                             Components =
                             [
                                 {x.WithNewlinePadding(4)}
                             ]
                             """
                        )
                }"
                    .TrimEnd()
                    .WithNewlinePadding(4)
                    .PrefixIfSome("\n{\n".Postfix(4))
                    .PostfixIfSome("\n}")
            }}
            """;
}
