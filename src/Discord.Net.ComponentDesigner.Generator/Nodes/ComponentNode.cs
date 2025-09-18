using Discord.ComponentDesignerGenerator.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Discord.ComponentDesignerGenerator.Nodes;

public abstract class ComponentNode<TState> : ComponentNode where TState : ComponentState, IEquatable<TState>, new()
{
    public abstract string Render(TState state);

    public virtual void UpdateState(ref TState state) { }

    public sealed override void UpdateState(ref ComponentState state)
        => UpdateState(ref Unsafe.As<ComponentState, TState>(ref state));

    public override ComponentState? Create(ICXNode source, List<CXNode> children)
        => new TState() {Source = source};

    public sealed override string Render(ComponentState state, ComponentContext context)
        => Render((TState)state);

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

    public virtual void Validate(ComponentState state, ComponentContext context) { }

    public abstract string Render(ComponentState state, ComponentContext context);

    public virtual void UpdateState(ref ComponentState state) { }

    public virtual ComponentState? Create(ICXNode source, List<CXNode> children)
        => new() {Source = source};


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

    public static bool TryGetNode(string name, out ComponentNode node)
        => _nodes.TryGetValue(name, out node);
}
