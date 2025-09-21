using System.Collections.Generic;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.CX.Nodes.Components;

public sealed class ThumbnailComponentNode : ComponentNode
{
    public override string Name => "thumbnail";

    public ComponentProperty Id { get; }
    public ComponentProperty Media { get; }
    public ComponentProperty Description { get; }
    public ComponentProperty Spoiler { get; }

    public override IReadOnlyList<ComponentProperty> Properties { get; }

    public ThumbnailComponentNode()
    {
        Properties =
        [
            Id = ComponentProperty.Id,
            Media = new(
                "media",
                aliases: ["href", "url"],
                renderer: Renderers.UnfurledMediaItem
            ),
            Description = new(
                "description",
                isOptional: true,
                renderer: Renderers.String
            ),
            Spoiler = new(
                "spoiler",
                isOptional: true,
                renderer: Renderers.Boolean,
                dotnetParameterName: "isSpoiler"
            )
        ];
    }

    public override string Render(ComponentState state, ComponentContext context)
        => $"""
            new {context.KnownTypes.ThumbnailBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}({
                state.RenderProperties(this, context)
                    .PrefixIfSome(4)
                    .WrapIfSome("\n")
            })
            """;
}
