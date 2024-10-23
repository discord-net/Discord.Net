using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;

public abstract class Node
{
    private static readonly Dictionary<Type, Node> _nodes = [];

    private readonly NodeProviders _providers;

    protected Logger Logger { get; }

    protected Node(NodeProviders providers, Logger logger)
    {
        Logger = logger;
        _providers = providers;
    }

    public readonly record struct StatefulGeneration<TState>(
        TState State,
        TypeSpec Spec
    );

    protected IncrementalValuesProvider<TResult> Branch<TResult, TIn, TOut>(
        IncrementalValuesProvider<TIn> provider,
        Func<TIn, TOut?, CancellationToken, TResult> mapper,
        params IBranchNode<TIn, TOut>[] nodes
    )
    {
        Logger.Log($"Creating branch: {GetType().Name} -> [{string.Join(" | ", nodes.Select(x => x.GetType().Name))}]");
        
        var branch = provider
            .Branch()
            .ForEach(
                nodes,
                (branch, node) => node.Branch(branch)
            );

        return branch.Merge(provider, mapper);
    }

    protected IncrementalValuesProvider<StatefulGeneration<TState>> AddChildren<TState>(
        IncrementalValuesProvider<StatefulGeneration<TState>> provider,
        params Type[] children
    )
        where TState : IHasActorInfo
    {
        Logger.Log($"Registering {children.Length} children for {typeof(TState).Name}");

        try
        {
            foreach (var child in children)
            {
                if (!typeof(Node).IsAssignableFrom(child))
                {
                    Logger.Log($"{child}: skipping, not a node");
                    continue;
                }

                switch (GetInstance(child, _providers))
                {
                    case Nodes.INestedNode nested:
                        provider = nested.From(provider);
                        break;
                    case INestedNode<TState> nested:
                        provider = nested.From(provider);
                        break;
                    default:
                        Logger.Log($"{child}: skipping, not a nested node");
                        break;
                }
            }

            return provider;
        }
        catch (Exception x)
        {
            Logger.Log(LogLevel.Error, $"Failed to initialize children: {x}");
            throw;
        }
        finally
        {
            Logger.Flush();
        }
    }

    public static IncrementalValuesProvider<StatefulGeneration<ActorNode.IntrospectedBuildState>> Create(
        NodeProviders providers
    )
    {
        _nodes.Clear();

        return GetInstance<ActorNode>(providers).TypeProvider;
    }

    protected TNode GetInstance<TNode>()
        where TNode : Node
        => GetInstance<TNode>(_providers);

    private static TNode GetInstance<TNode>(NodeProviders providers)
        where TNode : Node
        => (TNode) GetInstance(typeof(TNode), providers);

    private static Node GetInstance(Type type, NodeProviders providers)
    {
        if (!_nodes.TryGetValue(type, out var node))
            _nodes[type] = node = (Node) Activator.CreateInstance(
                type,
                providers,
                Logger.CreateForTask(type.Name).WithCleanLogFile()
            );

        return node;
    }

    protected static string GetFriendlyName(TypeRef type, bool forceInterfaceRules = false)
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

    protected static IEnumerable<IEnumerable<T>> GetProduct<T>(IEnumerable<T> source, bool removeLast = false)
    {
        var arr = source.ToArray();

        if (arr.Length == 0) return [];

        return Enumerable.Range(1, (1 << arr.Length) - (removeLast ? 2 : 1))
            .Select(index => arr
                .Where((_, i) => (index & (1 << i)) != 0)
            );
    }
}

public readonly record struct NodeProviders(
    IncrementalValuesProvider<LinkSchematics.Schematic> Schematics,
    IncrementalValuesProvider<LinkActorTargets.GenerationTarget> Actors,
    IncrementalValuesProvider<LinksV5.NodeContext> Context
);

public interface INestedNode
{
    IncrementalValuesProvider<Node.StatefulGeneration<TState>> From<TState>(
        IncrementalValuesProvider<Node.StatefulGeneration<TState>> provider
    ) where TState : IHasActorInfo;
}

public interface INestedNode<TState>
    where TState : IHasActorInfo
{
    IncrementalValuesProvider<Node.StatefulGeneration<TState>> From(
        IncrementalValuesProvider<Node.StatefulGeneration<TState>> provider
    );
}

public interface IBranchNode<TIn, TOut>
{
    IncrementalValuesProvider<Branch<TOut, TIn>> Branch(
        IncrementalValuesProvider<Branch<TIn, TIn>> provider
    );
}