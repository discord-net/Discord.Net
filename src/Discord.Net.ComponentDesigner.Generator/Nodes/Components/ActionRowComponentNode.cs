using Discord.CX.Parser;
using Discord.CX.Nodes.Components.SelectMenus;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.CX.Nodes.Components;

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

        switch (state.Children[0].Inner)
        {
            case ButtonComponentNode:
                foreach (var rest in state.Children.Skip(1))
                {
                    if (rest.Inner is not ButtonComponentNode)
                    {
                        context.AddDiagnostic(
                            Diagnostics.ActionRowInvalidChild,
                            rest.State.Source
                        );
                    }
                }

                break;
            case SelectMenuComponentNode:
                foreach (var rest in state.Children.Skip(1))
                {
                    context.AddDiagnostic(
                        Diagnostics.ActionRowInvalidChild,
                        rest.State.Source
                    );
                }

                break;

            case InterleavedComponentNode: break;

            default:
                foreach (
                    var rest
                    in state.Children.Where(x => !IsValidChild(x.Inner))
                )
                {
                    context.AddDiagnostic(
                        Diagnostics.ActionRowInvalidChild,
                        rest.State.Source
                    );
                }

                break;
        }

        base.Validate(state, context);
    }

    private static bool IsValidChild(ComponentNode node)
        => node is ButtonComponentNode
            or SelectMenuComponentNode
            or InterleavedComponentNode;

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
