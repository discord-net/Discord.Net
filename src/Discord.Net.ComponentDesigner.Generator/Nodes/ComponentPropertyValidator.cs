namespace Discord.ComponentDesigner.Generator.Nodes;

public delegate void ComponentPropertyValidator<T>(
    ComponentNode node,
    ComponentProperty<T> property,
    ComponentNodeContext context
);
