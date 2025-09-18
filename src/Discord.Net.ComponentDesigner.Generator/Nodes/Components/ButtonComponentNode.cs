using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesignerGenerator.Nodes.Components;

public sealed class ButtonComponentNode : ComponentNode
{
    public const string BUTTON_STYLE_ENUM = "Discord.ButtonStyle";
    public override string Name => "button";

    public override IReadOnlyList<ComponentProperty> Properties { get; }

    public ComponentProperty Style { get; }
    public ComponentProperty Label { get; }
    public ComponentProperty Emoji { get; }
    public ComponentProperty CustomId { get; }
    public ComponentProperty SkuId { get; }
    public ComponentProperty Url { get; }

    public ButtonComponentNode()
    {
        Properties =
        [
            Style = new ComponentProperty(
                "style",
                isOptional: true,
                validators: [Validators.EnumVariant(BUTTON_STYLE_ENUM)],
                renderer: Renderers.RenderEnum(BUTTON_STYLE_ENUM)
            ),
            Label = new ComponentProperty(
                "label",
                isOptional: true,
                validators: [Validators.Range(upper: Constants.BUTTON_MAX_LABEL_LENGTH)],
                renderer: Renderers.String
            ),
            Emoji = new ComponentProperty(
                "emoji",
                isOptional: true,
                aliases: ["emote"],
                validators: [Validators.Emote]
            ),
            CustomId = new(
                "customId",
                isOptional: true,
                validators: [Validators.Range(upper: Constants.CUSTOM_ID_MAX_LENGTH)]
            ),
            SkuId = new(
                "skuId",
                aliases: ["sku"],
                isOptional: true,
                validators: [Validators.Snowflake]
            ),
            Url = new(
                "url",
                isOptional: true,
                validators: [Validators.Range(upper: Constants.BUTTON_URL_MAX_LENGTH)]
            )
        ];
    }

    public override void Validate(ComponentState state, ComponentContext context)
    {
        if (state.GetProperty(Url)!.IsSpecified && state.GetProperty(CustomId)!.IsSpecified)
        {
            context.AddDiagnostic(
                Diagnostic.Create(
                    Diagnostics.ButtonCustomIdUrlConflict,
                    context.GetLocation(state.Source)
                )
            );
        }

        if (!state.GetProperty(Url)!.IsSpecified && !state.GetProperty(CustomId)!.IsSpecified)
        {
            context.AddDiagnostic(
                Diagnostic.Create(
                    Diagnostics.ButtonCustomIdOrUrlMissing,
                    context.GetLocation(state.Source)
                )
            );
        }
    }

    public override string Render(ComponentState state, ComponentContext context)
        => $"""
            new {context.KnownTypes.ButtonBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                {state.RenderProperties(this, context).WithNewlinePadding(4)}
            )
            """;
}
