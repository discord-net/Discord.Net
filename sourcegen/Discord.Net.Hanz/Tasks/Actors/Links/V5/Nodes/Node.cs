using System.Collections.Immutable;
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
    )
    {
        public StatefulGeneration<TNewState> Mutate<TNewState>(TNewState state)
            => new(state, Spec);
    }

    protected IncrementalValuesProvider<Branch<StatefulGeneration<TState>>> ApplyPathNesting<TState>(
        IncrementalValuesProvider<Branch<StatefulGeneration<TState>>> provider
    ) where TState : IPathedState
    {
        return provider.Collect().SelectMany(MapGraphs).SelectMany(BuildGraph);
    }

    private IEnumerable<StatefulGeneration<TState>> BuildGraph<TState>(
        Graph<TState> graph,
        CancellationToken token
    ) where TState : IPathedState
    {
        var roots = new Dictionary<TypePath, StatefulGeneration<TState>>();
        var lookup = new Dictionary<TypePath, TypeSpec>();

        foreach (var generation in graph.Links.OrderBy(x => x.State.Path.CountOfType(GetType())))
        {
            var (state, spec) = generation;

            lookup[state.Path] = spec;

            var pathDepth = state.Path.CountOfType(GetType());

            if (pathDepth == 1)
            {
                roots[state.Path] = generation;
                continue;
            }

            var searchPath = state.Path;
            var searchTotal = pathDepth - 1;
            var toNest = spec;

            for (var i = 0; i < searchTotal; i++)
            {
                searchPath--;

                if (!lookup.TryGetValue(searchPath, out var parentSpec))
                    throw new InvalidOperationException($"Missing spec for {searchPath} from {state.Path}");

                toNest = lookup[searchPath] = parentSpec.AddNestedType(toNest);

                if (roots.ContainsKey(searchPath))
                    roots[searchPath] = roots[searchPath] with {Spec = toNest};
            }
        }

        return roots.Values;
    }

    private IEnumerable<Branch<Graph<TState>>> MapGraphs<TState>(
        ImmutableArray<Branch<StatefulGeneration<TState>>> states,
        CancellationToken token
    ) where TState : IPathedState
    {
        foreach (var link in states.GroupBy(x => x.SourceVersion))
        {
            yield return new Branch<Graph<TState>>(
                link.Key,
                new Graph<TState>(link.Select(x => x.Value).ToImmutableEquatableArray())
            );
        }
    }

    private readonly record struct Graph<TState>(
        ImmutableEquatableArray<StatefulGeneration<TState>> Links
    ) where TState : IPathedState;

    protected IncrementalValuesProvider<Branch<TResult>> AddNestedTypes<TSource, TState, TIn, TResult>(
        INestedTypeProducerNode<TIn> node,
        IncrementalValuesProvider<Branch<TSource>> provider,
        Func<TSource, CancellationToken, TIn> parameterMapper,
        Func<TSource, ImmutableArray<TypeSpec>, CancellationToken, TResult> resultMapper,
        Func<TSource, TState> stateMapper
    )
    {
        return node
            .Create(
                provider
                    .Select((branch, token) => branch
                        .CreateNestedBranch(
                            (parameterMapper(branch.Value, token), stateMapper(branch.Value))
                        )
                    )
            )
            .Collect(
                provider,
                (source, spec, token) => source.Mutate(resultMapper(source.Value, spec, token))
            );
    }

    protected IncrementalValuesProvider<Branch<StatefulGeneration<TState>>> AddNestedTypes<TState, TIn>(
        IncrementalValuesProvider<Branch<StatefulGeneration<TState>>> provider,
        Func<TState, CancellationToken, TIn> parameterMapper,
        params INestedTypeProducerNode<TIn>[] nodes
    )
    {
        return provider
            .Select((branch, token) => branch
                .CreateNestedBranch(
                    (parameterMapper(branch.Value.State, token), branch.Value.State)
                )
            )
            .ForEach(
                nodes,
                (provider, node) => node.Create(provider)
            )
            .Collect(
                provider,
                (branch, specs, _) =>
                    branch.Mutate(branch.Value with {Spec = branch.Value.Spec.AddNestedTypes(specs)})
            );
    }

    protected IncrementalValuesProvider<StatefulGeneration<TState>> AddNestedTypes<TState, TIn>(
        IncrementalValuesProvider<StatefulGeneration<TState>> provider,
        Func<TState, CancellationToken, TIn> parameterMapper,
        params INestedTypeProducerNode<TIn>[] nodes
    )
    {
        return provider
            .Branch()
            .Select(
                (branch, token) =>
                    branch.Mutate(
                        (parameterMapper(branch.Value.State, token), branch.Value.State)
                    )
            )
            .ForEach(
                nodes,
                (provider, node) => node.Create(provider)
            )
            .Collect(
                provider,
                (source, specs, _) =>
                    source with {Spec = source.Spec.AddNestedTypes(specs)}
            );
    }

    protected IncrementalValuesProvider<StatefulGeneration<TState>> AddNestedTypes<TState, TIn>(
        INestedTypeProducerNode<TIn> node,
        IncrementalValuesProvider<StatefulGeneration<TState>> provider,
        Func<TState, CancellationToken, TIn> parameterMapper
    )
    {
        return node
            .Create(
                provider
                    .Branch()
                    .Select(
                        (branch, token) =>
                            branch.Mutate(
                                (parameterMapper(branch.Value.State, token), branch.Value.State)
                            )
                    )
            )
            .Collect(
                provider,
                (source, specs, _) =>
                    source with {Spec = source.Spec.AddNestedTypes(specs)}
            );
    }

    protected IncrementalValuesProvider<Branch<TResult>> Branch<TResult, TIn, TOut>(
        IncrementalValuesProvider<Branch<TIn>> provider,
        Func<TIn, ImmutableArray<TOut>, CancellationToken, TResult> mapper,
        params IBranchNode<TIn, TOut>[] nodes
    )
    {
        return provider
            .ForEach(
                nodes,
                (branch, node) => node.Branch(branch)
            )
            .Collect(
                provider,
                (branch, results, token) => branch.Mutate(mapper(branch.Value, results, token))
            );
    }

    protected IncrementalValuesProvider<TResult> Branch<TResult, TIn, TOut>(
        IncrementalValuesProvider<TIn> provider,
        Func<TIn, ImmutableArray<TOut>, CancellationToken, TResult> mapper,
        params IBranchNode<TIn, TOut>[] nodes
    )
    {
        return provider
            .Branch()
            .ForEach(
                nodes,
                (branch, node) => node.Branch(branch)
            )
            .Collect(
                provider,
                mapper
            );
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

    public static IEnumerable<IEnumerable<T>> GetProduct<T>(IEnumerable<T> source, bool removeLast = false)
    {
        var arr = source.ToArray();

        if (arr.Length == 0) return [];

        return Enumerable.Range(1, (1 << arr.Length) - (removeLast ? 2 : 1))
            .Select(index => arr
                .Where((_, i) => (index & (1 << i)) != 0)
            );
    }
}

public readonly struct NodeProviders
{
    public readonly record struct Hierarchy(
        string Actor,
        ImmutableEquatableArray<ActorInfo> Parents,
        ImmutableEquatableArray<ActorInfo> Children
    );

    public IncrementalValueProvider<Grouping<string, ActorInfo>> ActorAncestors { get; }

    public IncrementalValuesProvider<Hierarchy> ActorHierarchy { get; }

    public IncrementalValueProvider<Grouping<string, ActorInfo>> ActorInfos { get; }

    public IncrementalValuesProvider<LinkSchematics.Schematic> Schematics { get; }

    public IncrementalValuesProvider<LinkActorTargets.GenerationTarget> Actors { get; }

    public IncrementalValuesProvider<LinksV5.NodeContext> Context { get; }

    public NodeProviders(
        IncrementalValuesProvider<LinkSchematics.Schematic> Schematics,
        IncrementalValuesProvider<LinkActorTargets.GenerationTarget> Actors,
        IncrementalValuesProvider<LinksV5.NodeContext> Context)
    {
        this.Schematics = Schematics;
        this.Actors = Actors;
        this.Context = Context;

        ActorHierarchy = Actors
            .Collect()
            .SelectMany(GetHierarchy);

        ActorInfos = Actors
            .Select((x, _) => ActorInfo.Create(x))
            .GroupBy(x => x.Actor.DisplayString);

        ActorAncestors = ActorHierarchy
            .Collect()
            .GroupBy(x => x.Actor, x => x.Parents);
    }


    private static IEnumerable<Hierarchy> GetHierarchy(
        ImmutableArray<LinkActorTargets.GenerationTarget> targets,
        CancellationToken token)
    {
        foreach (var target in targets)
        {
            yield return new Hierarchy(
                target.Actor.ToDisplayString(),
                targets
                    .Where(x => Net.Hanz.Hierarchy.Implements(target.Actor, x.Actor))
                    .Select((x, _) => ActorInfo.Create(x))
                    .ToImmutableEquatableArray(),
                targets
                    .Where(x => Net.Hanz.Hierarchy.Implements(x.Actor, target.Actor))
                    .Select((x, _) => ActorInfo.Create(x))
                    .ToImmutableEquatableArray()
            );

            token.ThrowIfCancellationRequested();
        }
    }
}

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
    IncrementalValuesProvider<Branch<TOut>> Branch(
        IncrementalValuesProvider<Branch<TIn>> provider
    );
}

public readonly record struct NestedTypeProducerContext(
    ActorInfo ActorInfo,
    TypePath Path
);

public interface INestedTypeProducerNode : INestedTypeProducerNode<NestedTypeProducerContext>;

public interface INestedTypeProducerNode<TParameters>
{
    IncrementalValuesProvider<Branch<TypeSpec>> Create<TSource>(
        IncrementalValuesProvider<Branch<(TParameters Parameters, TSource Source)>> provider
    );
}