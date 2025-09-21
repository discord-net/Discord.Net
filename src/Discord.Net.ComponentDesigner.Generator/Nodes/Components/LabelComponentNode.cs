using System.Collections.Generic;

namespace Discord.CX.Nodes.Components;

public sealed class LabelComponentNode : ComponentNode
{
    public override string Name => "label";

    public ComponentProperty Value { get; }
    public ComponentProperty Description { get; }

    public override bool HasChildren => true;

    public override IReadOnlyList<ComponentProperty> Properties { get; }

    public LabelComponentNode()
    {
        Properties =
        [
            ComponentProperty.Id,
            Value = new(
                "value",
                renderer: Renderers.String
            ),
            Description = new(
                "description",
                isOptional: true,
                renderer: Renderers.String
            )
        ];
    }

    public override string Render(ComponentState state, ComponentContext context)
        => string.Empty;
}
