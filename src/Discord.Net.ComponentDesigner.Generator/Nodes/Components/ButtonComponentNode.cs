using Discord.ComponentDesignerGenerator.Parser;
using System;
using System.Xml;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesignerGenerator.Nodes;

public sealed class ButtonComponentNode : ComponentNode
{
    public override string FriendlyName => "Button";
    public override NodeKind Kind => NodeKind.Button;
    public ComponentProperty<ButtonStyle> Style { get; }
    public ComponentProperty<string> Label { get; }
    public ComponentProperty<string> Emoji { get; }
    public ComponentProperty<string> CustomId { get; }
    public ComponentProperty<ulong> SkuId { get; }
    public ComponentProperty<string> Url { get; }
    public ComponentProperty<bool> IsDisabled { get; }

    private readonly CXmlValue? _buttonLabelNode;

    public ButtonComponentNode(CXmlElement xml, ComponentNodeContext context) : base(xml, context)
    {
        Style = MapProperty<ButtonStyle>(
            "style",
            ValueParsers.ParseEnumProperty,
            defaultValue: ButtonStyle.Primary,
            optional: true,
            apiType: context.KnownTypes.ButtonStyleEnumType
        );

        Label = MapProperty(
            "label",
            optional: true,
            validators: [Validators.LengthBounds(upper: Constants.BUTTON_MAX_LABEL_LENGTH)]
        );

        Emoji = MapProperty("emoji", optional: true, parser: ValueParsers.ParseEmojiProperty);

        CustomId = MapProperty(
            "customId",
            optional: true,
            validators: [Validators.LengthBounds(upper: Constants.CUSTOM_ID_MAX_LENGTH)]
        );

        SkuId = MapProperty<ulong>("skuId", ValueParsers.ParseSnowflakeProperty, optional: true, aliases: "sku");

        Url = MapProperty(
            "url",
            optional: true,
            validators: [Validators.LengthBounds(upper: Constants.BUTTON_URL_MAX_LENGTH)]
        );

        IsDisabled = MapProperty<bool>("disabled", ValueParsers.ParseBooleanProperty, optional: true, defaultValue: false);

        if (xml.Children.Count > 1)
        {
            context.ReportDiagnostic(
                Diagnostics.ButtonChildLabelError,
                Location
            );
        }

        if (xml.Children.Count is not 0)
        {
            var childXml = xml.Children[0];

            if (childXml is not CXmlValue valueNode)
            {
                context.ReportDiagnostic(
                    Diagnostics.InvalidChildNodeType,
                    Location,
                    FriendlyName,
                    "Element"
                );
                return;
            }

            _buttonLabelNode = valueNode;
        }
    }

    public override void ReportValidationErrors()
    {
        base.ReportValidationErrors();

        if (Label.IsSpecified && _buttonLabelNode is not null)
        {
            // report on both
            Context.ReportDiagnostic(
                Diagnostics.ButtonDuplicateLabels,
                Context.GetLocation(Label.Attribute!.Span)
            );

            Context.ReportDiagnostic(
                Diagnostics.ButtonDuplicateLabels,
                Context.GetLocation(_buttonLabelNode)
            );
        }

        if (CustomId.IsSpecified && Url.IsSpecified)
        {
            // report on both URL and custom id
            Context.ReportDiagnostic(
                Diagnostics.UrlAndCustomIdBothSpecified,
                Context.GetLocation(CustomId.Attribute!.Span)
            );

            Context.ReportDiagnostic(
                Diagnostics.UrlAndCustomIdBothSpecified,
                Context.GetLocation(Url.Attribute!.Span)
            );
        }

        if (!CustomId.IsSpecified && !Url.IsSpecified)
        {
            Context.ReportDiagnostic(
                Diagnostics.MissingButtonCustumIdOrUrl,
                Location
            );
        }

        if (Style.TryGetScalarValue(out var scalar) && Enum.TryParse<ButtonStyle>(scalar, out var style))
        {
            switch (style)
            {
                case ButtonStyle.Premium when !SkuId.IsSpecified:
                    Context.ReportDiagnostic(
                        Diagnostics.MissingRequiredProperty,
                        Location,
                        "Premium Button",
                        Style.Name
                    );
                    break;
                case ButtonStyle.Link when !Url.IsSpecified:
                    Context.ReportDiagnostic(
                        Diagnostics.MissingRequiredProperty,
                        Location,
                        "Link Button",
                        Url.Name
                    );
                    break;
            }
        }

        // TODO: rest of validation
    }

    private string RenderLabel()
    {
        if (Label.IsSpecified) return Label.ToString();

        return ComponentProperty<string>.BuildValue(_buttonLabelNode) ?? "default";
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.ButtonBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                label: {RenderLabel().WithNewlinePadding(4)},
                customId: {CustomId.ToString().WithNewlinePadding(4)},
                style: {Context.KnownTypes.ButtonStyleEnumType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{Style},
                url: {Url.ToString().WithNewlinePadding(4)},
                emote: {Emoji.ToString().WithNewlinePadding(4)},
                isDisabled: {IsDisabled.ToString().WithNewlinePadding(4)},
                skuId: {SkuId.ToString().WithNewlinePadding(4)},
                id: {Id}
            )
            """;
}

public enum ButtonStyle
{
    Primary = 1,
    Secondary = 2,
    Success = 3,
    Danger = 4,
    Link = 5,
    Premium = 6
}
