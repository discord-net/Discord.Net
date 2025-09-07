using Discord.ComponentDesignerGenerator.Parser;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Nodes;

public abstract class BaseSelectComponentNode : ComponentNode
{
    public ComponentProperty<string> CustomId { get; }
    public ComponentProperty<string> Placeholder { get; }
    public ComponentProperty<int> MinValues { get; }
    public ComponentProperty<int> MaxValues { get; }
    public ComponentProperty<bool> IsDisabled { get; }

    public IReadOnlyList<SelectDefaultValue> DefaultValues { get; }

    protected BaseSelectComponentNode(
        CXmlElement xml,
        ComponentNodeContext context,
        bool hasDefaultValues = true
    ) : base(xml, context)
    {
        CustomId = MapProperty(
            "customId",
            validators: [Validators.LengthBounds(upper: Constants.CUSTOM_ID_MAX_LENGTH)]
        );

        Placeholder = MapProperty(
            "placeholder",
            optional: true,
            validators: [Validators.LengthBounds(upper: Constants.PLACEHOLDER_MAX_LENGTH)]
        );

        MinValues = MapProperty(
            "minValues",
            optional: true,
            parser: ValueParsers.ParseIntProperty,
            validators:
            [
                Validators.Bounds(
                    Constants.SELECT_MIN_VALUES,
                    Constants.SELECT_MAX_VALUES
                )
            ],
            aliases: ["min"]
        );

        MaxValues = MapProperty(
            "maxValues",
            optional: true,
            parser: ValueParsers.ParseIntProperty,
            validators:
            [
                Validators.Bounds(
                    Constants.SELECT_MIN_VALUES + 1,
                    Constants.SELECT_MAX_VALUES
                )
            ],
            aliases: ["max"]
        );

        IsDisabled = MapProperty<bool>("disabled", optional: true, parser: ValueParsers.ParseBooleanProperty);

        if (!hasDefaultValues)
        {
            DefaultValues = [];
            return;
        }

        var defaultValues = new List<SelectDefaultValue>();

        foreach (var child in xml.Children)
        {
            if (child is not CXmlElement element)
            {
                context.ReportDiagnostic(
                    Diagnostics.InvalidChildNodeType,
                    context.GetLocation(child),
                    FriendlyName,
                    "text"
                );

                continue;
            }

            if (element.Name.Value is not "default")
            {
                context.ReportDiagnostic(
                    Diagnostics.InvalidChildComponentType,
                    context.GetLocation(child),
                    FriendlyName,
                    element.Name.Value
                );

                continue;
            }

            defaultValues.Add(new SelectDefaultValue(element, context));
        }

        DefaultValues = defaultValues;
    }

    public override void ReportValidationErrors()
    {
        if (DefaultValues.Count > Constants.SELECT_MAX_VALUES)
        {
            Context.ReportDiagnostic(
                Diagnostics.TooManyChildren,
                Location,
                FriendlyName,
                Constants.SELECT_MAX_VALUES
            );
        }
    }
}
