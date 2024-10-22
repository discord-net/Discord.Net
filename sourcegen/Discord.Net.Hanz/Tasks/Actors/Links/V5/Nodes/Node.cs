using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;

public abstract class Node
{
    private static readonly Dictionary<Type, Node> _nodes = [];

    private readonly IncrementalValuesProvider<LinksV5.NodeContext> _context;

    protected Logger Logger { get; }

    protected Node(IncrementalValuesProvider<LinksV5.NodeContext> context, Logger logger)
    {
        Logger = logger;
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

    public static IncrementalValuesProvider<StatefulGeneration<ActorNode.IntrospectedBuildState>> Create(
        IncrementalValuesProvider<LinksV5.NodeContext> context
    ) => GetInstance<ActorNode>(context).TypeProvider;

    private static TNode GetInstance<TNode>(IncrementalValuesProvider<LinksV5.NodeContext> context)
        where TNode : Node
        => (TNode) GetInstance(typeof(TNode), context);

    private static Node GetInstance(Type type, IncrementalValuesProvider<LinksV5.NodeContext> context)
    {
        if (!_nodes.TryGetValue(type, out var node))
            _nodes[type] = node = (Node) Activator.CreateInstance(type, context, Logger.CreateForTask(type.Name));

        return node;
    }
    
    public static string GetFriendlyName(TypeRef type, bool forceInterfaceRules = false)
    {
        var name = type.Name;

        if (forceInterfaceRules || type.TypeKind is TypeKind.Interface)
            name = type.Name.Remove(0, 1);

        return name
            .Replace("Trait", string.Empty)
            .Replace("Actor", string.Empty)
            .Replace("Gateway", string.Empty)
            .Replace("Rest", string.Empty);
    }
}

public interface INestedNode
{
    IncrementalValuesProvider<Node.StatefulGeneration<TState>> From<TState>(
        IncrementalValuesProvider<Node.StatefulGeneration<TState>> provider
    ) where TState : IHasActorInfo;
}