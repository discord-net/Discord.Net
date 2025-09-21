using System.Collections.Generic;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.CX.Nodes.Components;

public sealed class TextInputComponentNode : ComponentNode
{
    public const string LIBRARY_TEXT_INPUT_STYLE_ENUM = "Discord.TextInputStyle";

    public override string Name => "input";

    public ComponentProperty CustomId { get; }
    public ComponentProperty Style { get; }
    public ComponentProperty MinLength { get; }
    public ComponentProperty MaxLength { get; }
    public ComponentProperty Required { get; }
    public ComponentProperty Value { get; }
    public ComponentProperty Placeholder { get; }

    public override IReadOnlyList<ComponentProperty> Properties { get; }

    public TextInputComponentNode()
    {
        Properties =
        [
            ComponentProperty.Id,
            CustomId = new(
                "customId",
                isOptional: false,
                renderer: Renderers.String
            ),
            Style = new(
                "style",
                isOptional: false,
                renderer: Renderers.RenderEnum(LIBRARY_TEXT_INPUT_STYLE_ENUM)
            ),
            MinLength = new(
                "minLength",
                aliases: ["min"],
                isOptional: true,
                renderer: Renderers.Integer
            ),
            MaxLength = new(
                "maxLength",
                aliases: ["max"],
                isOptional: true,
                renderer: Renderers.Integer
            ),
            Required = new(
                "required",
                isOptional: true,
                renderer: Renderers.Boolean
            ),
            Value = new(
                "value",
                isOptional: true,
                renderer: Renderers.String
            ),
            Placeholder = new(
                "placeholder",
                isOptional: true,
                renderer: Renderers.String
            )
        ];
    }

    public override string Render(ComponentState state, ComponentContext context)
        => $"""
            new {context.KnownTypes.TextInputBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}({
                state.RenderProperties(this, context)
                    .PrefixIfSome(4)
                    .WithNewlinePadding(4)
                    .WrapIfSome("\n")
            })
            """;
}
