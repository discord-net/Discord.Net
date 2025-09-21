using Discord.CX.Parser;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.CX.Nodes.Components;

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
            ComponentProperty.Id,
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
                validators: [Validators.Emote],
                renderer: Renderers.Emoji
            ),
            CustomId = new(
                "customId",
                isOptional: true,
                validators: [Validators.Range(upper: Constants.CUSTOM_ID_MAX_LENGTH)],
                renderer: Renderers.String
            ),
            SkuId = new(
                "skuId",
                aliases: ["sku"],
                isOptional: true,
                validators: [Validators.Snowflake],
                renderer: Renderers.Snowflake
            ),
            Url = new(
                "url",
                isOptional: true,
                validators: [Validators.Range(upper: Constants.BUTTON_URL_MAX_LENGTH)],
                renderer: Renderers.String
            )
        ];
    }

    public override ComponentState? Create(ICXNode source, List<CXNode> children)
    {
        var state = base.Create(source, children);

        if (source is CXElement {Children.Count: 1} element && element.Children[0] is CXValue value)
            state?.SubstitutePropertyValue(Label, value);

        return state;
    }

    public override void Validate(ComponentState state, ComponentContext context)
    {
        var label = state.GetProperty(Label);

        if (
            label.Attribute?.Value is not null &&
            label.Value is not null &&
            label.Value != label.Attribute.Value
        )
        {
            context.AddDiagnostic(
                Diagnostic.Create(
                    Diagnostics.ButtonLabelDuplicate,
                    context.GetLocation(label.Value!)
                )
            );
        }

        if (state.GetProperty(Url)!.IsSpecified && state.GetProperty(CustomId)!.IsSpecified)
        {
            context.AddDiagnostic(
                Diagnostic.Create(
                    Diagnostics.ButtonCustomIdUrlConflict,
                    context.GetLocation(state.Source)
                )
            );
        }

        // TODO: interpolations with constants can be checked
        if (
            state.GetProperty(Style).TryGetLiteralValue(context, out var style)
        )
        {
            switch (style.ToLowerInvariant())
            {
                case "link" when !state.GetProperty(Url).IsSpecified:
                    context.AddDiagnostic(
                        Diagnostic.Create(
                            Diagnostics.LinkButtonUrlMissing,
                            context.GetLocation(state.Source)
                        )
                    );
                    break;
                case "premium" when !state.GetProperty(SkuId).IsSpecified:
                    context.AddDiagnostic(
                        Diagnostic.Create(
                            Diagnostics.PremiumButtonSkuMissing,
                            context.GetLocation(state.Source)
                        )
                    );

                    if (state.GetProperty(CustomId).IsSpecified)
                    {
                        context.AddDiagnostic(
                            Diagnostic.Create(
                                Diagnostics.PremiumButtonPropertyNotAllowed,
                                context.GetLocation(state.Source),
                                "customId"
                            )
                        );
                    }

                    if (state.GetProperty(Label).IsSpecified)
                    {
                        context.AddDiagnostic(
                            Diagnostic.Create(
                                Diagnostics.PremiumButtonPropertyNotAllowed,
                                context.GetLocation(state.Source),
                                "label"
                            )
                        );
                    }

                    if (state.GetProperty(Url).IsSpecified)
                    {
                        context.AddDiagnostic(
                            Diagnostic.Create(
                                Diagnostics.PremiumButtonPropertyNotAllowed,
                                context.GetLocation(state.Source),
                                "url"
                            )
                        );
                    }

                    if (state.GetProperty(Emoji).IsSpecified)
                    {
                        context.AddDiagnostic(
                            Diagnostic.Create(
                                Diagnostics.PremiumButtonPropertyNotAllowed,
                                context.GetLocation(state.Source),
                                "emoji"
                            )
                        );
                    }

                    break;
                default: CheckForCustomIdAndUrl(); break;
            }
        }
        else
        {
            CheckForCustomIdAndUrl();
        }

        base.Validate(state, context);

        void CheckForCustomIdAndUrl()
        {
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
    }

    public override string Render(ComponentState state, ComponentContext context)
        => $"""
            new {context.KnownTypes.ButtonBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}({
                state.RenderProperties(this, context)
                    .WithNewlinePadding(4)
                    .PrefixIfSome(4)
                    .WrapIfSome("\n")
            })
            """;
}
