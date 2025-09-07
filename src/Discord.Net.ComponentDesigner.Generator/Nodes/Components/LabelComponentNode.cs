using Discord.ComponentDesignerGenerator.Parser;
using System.Collections.Generic;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Nodes;

public sealed class LabelComponentNode : ComponentNode
{
    public override string FriendlyName => "Label";

    public override NodeKind Kind => NodeKind.Label;

    public ComponentProperty<string> Description { get; }

    public ComponentNode? Component { get; }
    public IReadOnlyList<CXmlValue> LabelValues { get; }

    public LabelComponentNode(CXmlElement xml, ComponentNodeContext context) : base(xml, context)
    {
        Description = MapProperty("description", optional: true);

        var values = new List<CXmlValue>();
        LabelValues = values;

        if (xml.Children.Count is 0)
        {
            context.ReportDiagnostic(
                Diagnostics.MissingLabelChildren,
                context.GetLocation(xml)
            );

            return;
        }

        var lastChild = xml.Children.Last();

        var lastChildComponent = Create(lastChild, context);

        if (lastChildComponent is null)
        {
            context.ReportDiagnostic(
                Diagnostics.InvalidChildComponentType,
                context.GetLocation(lastChild),
                FriendlyName,
                lastChild.GetType().Name
            );

            return;
        }

        Component = lastChildComponent;

        if (!IsValidLabelChild(Component.Kind))
        {
            context.ReportDiagnostic(
                Diagnostics.InvalidChildComponentType,
                context.GetLocation(lastChild),
                FriendlyName,
                Component.FriendlyName
            );

            return;
        }


        foreach (var child in xml.Children.Take(xml.Children.Count - 1))
        {
            // we don't allow any elements
            if (child is CXmlElement element)
            {
                context.ReportDiagnostic(
                    Diagnostics.InvalidChildComponentType,
                    context.GetLocation(child),
                    FriendlyName,
                    element.Name.Value
                );

                continue;
            }

            if (child is not CXmlValue value)
            {
                context.ReportDiagnostic(
                    Diagnostics.InvalidChildComponentType,
                    context.GetLocation(child),
                    FriendlyName,
                    child.GetType().Name
                );

                continue;
            }

            values.Add(value);
        }
    }

    private static bool IsValidLabelChild(NodeKind kind)
        => (
            kind & (
                NodeKind.TextInput |
                NodeKind.StringSelect
            )
        ) is not 0;

    private string BuildLabelValues()
    {
        if (LabelValues.Count is 1)
            return ComponentProperty<string>.BuildValue(LabelValues[0]) ?? "default";

        return ComponentProperty<string>.BuildMultipart(LabelValues);
    }

    public override string Render()
        => $"""
            new LabelBuilder(
                label: {BuildLabelValues().WithNewlinePadding(4)},
                component: {Component?.Render().WithNewlinePadding(4)},
                description: {Description.ToString().WithNewlinePadding(4)},
                id: {Id}
            )
            """;
}
