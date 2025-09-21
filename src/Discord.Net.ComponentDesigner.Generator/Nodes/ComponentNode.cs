using Discord.CX.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Discord.CX.Nodes;

public abstract class ComponentNode<TState> : ComponentNode
    where TState : ComponentState
{
    public abstract string Render(TState state, ComponentContext context);

    public virtual void UpdateState(ref TState state) { }

    public sealed override void UpdateState(ref ComponentState state)
        => UpdateState(ref Unsafe.As<ComponentState, TState>(ref state));

    public abstract TState? CreateState(ICXNode source, List<CXNode> children);

    public sealed override ComponentState? Create(ICXNode source, List<CXNode> children)
        => CreateState(source, children);

    public sealed override string Render(ComponentState state, ComponentContext context)
        => Render((TState)state, context);

    public virtual void Validate(TState state, ComponentContext context) { }

    public sealed override void Validate(ComponentState state, ComponentContext context)
        => Validate((TState)state, context);
}

public abstract class ComponentNode
{
    public abstract string Name { get; }
    public virtual IReadOnlyList<string> Aliases { get; } = [];

    public virtual bool HasChildren => false;

    public virtual IReadOnlyList<ComponentProperty> Properties { get; } = [];

    public virtual void Validate(ComponentState state, ComponentContext context)
    {
        // validate properties
        foreach (var property in Properties)
        foreach (var validator in property.Validators)
        {
            var propertyValue = state.GetProperty(property);

            validator(context, propertyValue);

            if (!property.IsOptional && !propertyValue.HasValue)
            {
                context.AddDiagnostic(
                    Diagnostics.MissingRequiredProperty,
                    state.Source,
                    Name,
                    property.Name
                );
            }
        }

        // report any unknown properties
        if (state.Source is CXElement element)
        {
            foreach (var attribute in element.Attributes)
            {
                if (!TryGetPropertyFromName(attribute.Identifier.Value, out _))
                {
                    context.AddDiagnostic(
                        Diagnostics.UnknownProperty,
                        attribute,
                        attribute.Identifier.Value,
                        Name
                    );
                }
            }
        }
    }

    private bool TryGetPropertyFromName(string name, out ComponentProperty result)
    {
        foreach (var property in Properties)
        {
            if (property.Name == name || property.Aliases.Contains(name))
            {
                result = property;
                return true;
            }
        }

        result = null!;
        return false;
    }

    public abstract string Render(ComponentState state, ComponentContext context);

    public virtual void UpdateState(ref ComponentState state) { }

    public virtual ComponentState? Create(ICXNode source, List<CXNode> children)
    {
        if (HasChildren && source is CXElement element)
        {
            children.AddRange(element.Children);
        }

        return new ComponentState() {Source = source};
    }


    private static readonly Dictionary<string, ComponentNode> _nodes;

    static ComponentNode()
    {
        _nodes = typeof(ComponentNode)
            .Assembly
            .GetTypes()
            .Where(x => !x.IsAbstract && typeof(ComponentNode).IsAssignableFrom(x))
            .Select(x => (ComponentNode)Activator.CreateInstance(x)!)
            .SelectMany(x => x
                .Aliases
                .Prepend(x.Name)
                .Select(y => new KeyValuePair<string, ComponentNode>(y, x)))
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public static T GetComponentNode<T>() where T : ComponentNode
        => _nodes.Values.OfType<T>().First();

    public static bool TryGetNode(string name, out ComponentNode node)
        => _nodes.TryGetValue(name, out node);
}
