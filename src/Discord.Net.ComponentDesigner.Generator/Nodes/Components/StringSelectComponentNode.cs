using Discord.ComponentDesigner.Generator.Parser;
using System.Collections.Generic;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Discord.ComponentDesigner.Generator.Nodes;

public sealed class StringSelectComponentNode : BaseSelectComponentNode
{
    public override string FriendlyName => "String Select";
    public override NodeKind Kind => NodeKind.StringSelect;
    public IReadOnlyList<SelectOption> Options { get; }

    public StringSelectComponentNode(CXmlElement xml, ComponentNodeContext context) : base(
        xml,
        context,
        hasDefaultValues: false
    )
    {
        var options = new List<SelectOption>();

        foreach (var childXml in xml.Children)
        {
            if (childXml is not CXmlElement element)
            {
                context.ReportDiagnostic(
                    Diagnostics.InvalidChildNodeType,
                    context.GetLocation(childXml),
                    FriendlyName,
                    "text"
                );

                continue;
            }

            if (element.Name.Value is not "option")
            {
                context.ReportDiagnostic(
                    Diagnostics.InvalidChildComponentType,
                    context.GetLocation(childXml),
                    FriendlyName,
                    element.Name.Value
                );

                continue;
            }

            options.Add(new SelectOption(element, context));
        }

        Options = options;
    }

    public override void ReportValidationErrors()
    {
        base.ReportValidationErrors();

        foreach (var option in Options) option.ReportValidationErrors();
    }

    public override string Render()
        => $"""
            new {Context.KnownTypes.SelectMenuBuilderType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}(
                customId: {CustomId.ToString().WithNewlinePadding(4)},
                placeholder: {Placeholder.ToString().WithNewlinePadding(4)},
                maxValues: {MaxValues.ToString().WithNewlinePadding(4)},
                minValues: {MinValues.ToString().WithNewlinePadding(4)},
                isDisabled: {IsDisabled.ToString().WithNewlinePadding(4)},
                type: {Context.KnownTypes.ComponentTypeEnumType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.StringSelect
            )
            """;
}

public sealed class SelectOption : ComponentNode
{
    public override string FriendlyName => "Select Option";

    public override NodeKind Kind => NodeKind.SelectOption;

    public ComponentProperty<string> Label { get; }

    public ComponentProperty<string> Value { get; }

    public ComponentProperty<string> Description { get; }

    public ComponentProperty<string> Emoji { get; }

    public ComponentProperty<bool> IsDefault { get; }

    public SelectOption(CXmlElement xml, ComponentNodeContext context) : base(xml, context, mapId: false)
    {
        Label = MapProperty(
            "label",
            validators: [Validators.LengthBounds(upper: Constants.STRING_SELECT_OPTION_LABEL_MAX_LENGTH)]
        );

        Value = MapProperty(
            "value",
            validators: [Validators.LengthBounds(upper: Constants.STRING_SELECT_OPTION_VALUE_MAX_LENGTH)]
        );

        Description = MapProperty(
            "description",
            optional: true,
            validators: [Validators.LengthBounds(upper: Constants.STRING_SELECT_OPTION_DESCRIPTION_MAX_LENGTH)]
        );

        Emoji = MapProperty(
            "emoji",
            optional: true,
            parser: ParseEmojiProperty
        );

        IsDefault = MapProperty<bool>(
            "default",
            ParseBooleanProperty,
            optional: true
        );
    }

    public override string Render()
    {
        throw new System.NotImplementedException();
    }
}
