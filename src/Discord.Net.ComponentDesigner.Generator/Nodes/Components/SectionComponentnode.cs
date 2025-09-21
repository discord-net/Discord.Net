using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.CX.Nodes.Components;

public sealed class SectionComponentnode : ComponentNode
{
    public override string Name => "section";

    public override bool HasChildren => true;

    public override IReadOnlyList<ComponentProperty> Properties { get; } = [ComponentProperty.Id];

    public override void Validate(ComponentState state, ComponentContext context)
    {
        if (!state.HasChildren)
        {
            context.AddDiagnostic(
                Diagnostics.EmptySection,
                state.Source
            );

            base.Validate(state, context);
            return;
        }

        var accessoryCount = state.Children.Count(x => x.Inner is AccessoryComponentNode);
        var nonAccessoryCount = state.Children.Count - accessoryCount;

        switch (accessoryCount)
        {
            case 0:
                context.AddDiagnostic(
                    Diagnostics.MissingAccessory,
                    state.Source
                );
                break;
            case > 1:
                foreach (var accessory in state.Children.Where(x => x.Inner is AccessoryComponentNode).Skip(1))
                {
                    context.AddDiagnostic(
                        Diagnostics.TooManyAccessories,
                        accessory.State.Source
                    );
                }

                break;
        }

        switch (nonAccessoryCount)
        {
            case 0:
                context.AddDiagnostic(
                    Diagnostics.MissingSectionChild,
                    state.Source
                );
                break;
            case > 3:
                foreach (var child in state.Children.Where(x => x.Inner is not AccessoryComponentNode).Skip(3))
                {
                    context.AddDiagnostic(
                        Diagnostics.TooManySectionChildren,
                        child.State.Source
                    );
                }
                break;
        }

        foreach (var child in state.Children.Where(x => x.Inner is not AccessoryComponentNode))
        {
            if (!IsValidChildType(child.Inner))
            {
                context.AddDiagnostic(
                    Diagnostics.InvalidSectionChildComponentType,
                    child.State.Source,
                    child.Inner.Name
                );
            }
        }

        static bool IsValidChildType(ComponentNode node)
            => node is TextDisplayComponentNode;
    }

    public override string Render(ComponentState state, ComponentContext context)
    {
        var accessory = state.Children
            .FirstOrDefault(x => x.Inner is AccessoryComponentNode);

        return
            $"""
             new {context.KnownTypes.SectionBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                 accessory: {accessory?.Render(context).WithNewlinePadding(4) ?? "null"},
                 components:
                 [
                     {
                         state
                             .RenderChildren(context, x => x.Inner is not AccessoryComponentNode)
                             .WithNewlinePadding(4)
                     }
                 ]
             ){state.RenderInitializer(this, context).PrefixIfSome("\n")}
             """;
    }
}

public sealed class AccessoryComponentNode : ComponentNode
{
    public override string Name => "accessory";

    public override bool HasChildren => true;

    public override void Validate(ComponentState state, ComponentContext context)
    {
        if (!state.HasChildren)
        {
            context.AddDiagnostic(
                Diagnostics.EmptyAccessory,
                state.Source
            );

            base.Validate(state, context);
            return;
        }


        if (state.Children.Count is not 1)
        {
            var start = state.OwningNode!.Children[0].State.Source.Span.Start;
            var end = state.OwningNode!.Children[state.Children.Count - 1].State.Source.Span.End;

            context.AddDiagnostic(
                Diagnostics.TooManyAccessoryChildren,
                TextSpan.FromBounds(start, end)
            );

            base.Validate(state, context);
            return;
        }

        if (!IsAllowedChild(state.Children[0].Inner))
        {
            context.AddDiagnostic(
                Diagnostics.InvalidAccessoryChild,
                state.Children[0].State.Source,
                state.Children[0].Inner.Name
            );
        }

        base.Validate(state, context);
    }

    private static bool IsAllowedChild(ComponentNode node)
        => node is ButtonComponentNode or ThumbnailComponentNode;

    public override string Render(ComponentState state, ComponentContext context)
        => state.RenderChildren(context);
}
