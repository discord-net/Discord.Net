using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;

public abstract class Node
{
    private static readonly Dictionary<Type, Node> _nodes = [];

    private readonly IncrementalValuesProvider<LinksV5.NodeContext> _context;

    protected Node(IncrementalValuesProvider<LinksV5.NodeContext> context)
    {
        _context = context;
    }

    public readonly record struct StatefulGeneration<TState>(
        TState State,
        TypeSpec Spec
    );

    protected IncrementalValuesProvider<StatefulGeneration<TState>> AddChildren<TState>(
        IncrementalValuesProvider<StatefulGeneration<TState>> provider,
        params Type[] children
    )
        where TState : IHasActorInfo
    {
        foreach (var child in children)
        {
            if (!typeof(Node).IsAssignableFrom(child)) continue;

            if (GetInstance(child, _context) is not INestedNode nested)
                continue;

            provider = nested.From(provider);
        }

        return provider;
    }

    public static TNode GetInstance<TNode>(IncrementalValuesProvider<LinksV5.NodeContext> context)
        where TNode : Node
        => (TNode) GetInstance(typeof(TNode), context);

    public static Node GetInstance(Type type, IncrementalValuesProvider<LinksV5.NodeContext> context)
    {
        if (!_nodes.TryGetValue(type, out var node))
            _nodes[type] = node = (Node) Activator.CreateInstance(type, context);

        return node;
    }
}

public interface INestedNode
{
    IncrementalValuesProvider<Node.StatefulGeneration<TState>> From<TState>(
        IncrementalValuesProvider<Node.StatefulGeneration<TState>> provider
    ) where TState : IHasActorInfo;
}