using Discord.ComponentDesigner.Generator.Parser;

namespace Discord.ComponentDesigner.Generator.Nodes;

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
            parser: ValueParsers.ParseEmojiProperty
        );

        IsDefault = MapProperty<bool>(
            "default",
            ValueParsers.ParseBooleanProperty,
            optional: true
        );
    }

    public override string Render()
    {
        throw new System.NotImplementedException();
    }
}
