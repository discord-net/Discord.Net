using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Types;

using ActorStateProvider = IncrementalValuesProvider<Node.StatefulGeneration<ActorNode.IntrospectedBuildState>>;
using ActorState = ActorNode.IntrospectedBuildState;

public class LinkNode :
    Node,
    INestedNode<ActorState>
{
    public record ImplementedState(
        State Link,
        ILinkImplmenter.LinkImplementation? Implementation
    ) : IHasActorInfo
    {
        public ActorInfo ActorInfo => Link.ActorInfo;
    }

    public record State(
        StatefulGeneration<ActorState> Actor,
        LinkSchematics.Entry Entry,
        ImmutableEquatableArray<LinkSchematics.Entry>? Parents = null
    ) :
        IHasActorInfo,
        ITypePath
    {
        public bool IsTemplate => Parents.Count == 0;
        public ActorInfo ActorInfo => Actor.State.ActorInfo;
        public LinkSchematics.Entry? ParentEntry => Parents.Count == 0 ? null : Parents[Parents.Count - 1];

        public ImmutableEquatableArray<LinkSchematics.Entry> Parents { get; init; }
            = Parents ?? ImmutableEquatableArray<LinkSchematics.Entry>.Empty;

        public IEnumerable<string> GetCartesianBases()
        {
            foreach (var entry in GetProduct(Parents.Add(Entry), removeLast: true))
            {
                yield return
                    $"{Actor.State.ActorInfo.Actor}.{string.Join(".", entry.Select(x => x.Type.ReferenceName))}";
            }
        }

        public string ToReferenceName()
        {
            var builder = new StringBuilder()
                .Append(Actor.State.ActorInfo.Actor.FullyQualifiedName)
                .Append('.');

            foreach (var parent in Parents)
            {
                builder
                    .Append(parent.Type.ReferenceName)
                    .Append('.');
            }

            return builder.Append(Entry.Type.ReferenceName).ToString();
        }

        public ImmutableEquatableArray<string> Parts
            => new([..Parents.Select(x => x.Type.ReferenceName), Entry.Type.ReferenceName]);
    }

    public record Graph(
        StatefulGeneration<ActorState> Parent,
        ImmutableEquatableArray<StatefulGeneration<ImplementedState>> Entries
    );

    private readonly IncrementalValueProvider<ImmutableArray<LinkSchematics.Schematic>> _schematics;

    public LinkNode(NodeProviders providers, Logger logger) : base(providers, logger)
    {
        _schematics = providers.Schematics.Collect();
    }

    public ActorStateProvider From(ActorStateProvider provider)
        => AddChildren(
                Branch(
                        provider
                            .Combine(_schematics)
                            .SelectMany(CreateState),
                        (state, implementation, _) => new ImplementedState(state, implementation),
                        GetInstance<IndexableLink>(),
                        GetInstance<EnumerableLink>()
                    )
                    .Select(CreateInitialLink),
                typeof(BackLinkNode)
            )
            .Collect()
            .SelectMany(CreateGraph)
            .Select(BuildGraph);

    private StatefulGeneration<ActorState> BuildGraph(
        Graph graph,
        CancellationToken token
    )
    {
        var logger = Logger
            .GetSubLogger(graph.Parent.State.ActorInfo.Assembly.ToString())
            .GetSubLogger(nameof(BuildGraph))
            .GetSubLogger(graph.Parent.State.ActorInfo.Actor.MetadataName)
            .WithCleanLogFile();

        var result = graph.Parent.Spec;

        var stack = new Stack<StatefulGeneration<ImplementedState>>();

        foreach (var entry in graph.Entries)
        {
            logger.Log($"{graph.Parent.State.ActorInfo.Actor}: {entry.State.Link.Entry.Type}");

            if (stack.Count == 0 || entry.State.Link.ParentEntry is not null)
            {
                stack.Push(entry);
                logger.Log($" - pushed to stack");
                continue;
            }

            logger.Log(" - building tree:");
            BuildTree(ref result);
            stack.Push(entry);
        }

        if (stack.Count > 0)
            BuildTree(ref result);

        logger.Flush();

        return graph.Parent with
        {
            Spec = result
        };

        void BuildTree(ref TypeSpec root)
        {
            var group = new List<TypeSpec>();

            var node = stack.Pop();
            group.Add(node.Spec);

            logger.Log($"   - Head: {node.State.Link.Entry.Type.Name} : {node.State.Link.ParentEntry?.Type.Name}");

            if (stack.Count == 0)
            {
                root = root.AddNestedType(node.Spec);
                logger.Log($"   - Added as root node");
                return;
            }

            while (stack.Count > 0)
            {
                var next = stack.Pop();

                logger.Log($"   - Next: {next.State.Link.Entry.Type.Name} : {next.State.Link.ParentEntry?.Type.Name}");

                if (next.State.Link.ParentEntry == node.State.Link.ParentEntry)
                {
                    group.Add(next.Spec);
                    logger.Log($"     += group ({group.Count})");
                    continue;
                }

                if (node.State.Link.ParentEntry == next.State.Link.Entry)
                {
                    logger.Log($"     - Ancestor of {group.Count} nodes");

                    next = next with
                    {
                        Spec = next.Spec.AddNestedTypes(group)
                    };

                    group.Clear();

                    node = next;

                    if (node.State.Link.ParentEntry is null)
                    {
                        logger.Log($"     - Adding to result, root node");

                        // we can add to the result
                        root = root.AddNestedType(node.Spec);

                        if (stack.Count > 0)
                        {
                            logger.Log($"     - Continuing to build tree");
                            BuildTree(ref root);
                        }

                        return;
                    }

                    node = next;
                    group.Add(node.Spec);
                }
            }
        }
    }

    private static IEnumerable<Graph> CreateGraph(
        ImmutableArray<StatefulGeneration<ImplementedState>> states,
        CancellationToken token
    )
    {
        foreach
        (
            var group
            in states.GroupBy(x => x.State.Link.Actor)
        )
        {
            yield return new Graph(group.Key, new(group));
        }
    }


    private StatefulGeneration<ImplementedState> CreateInitialLink(
        ImplementedState state,
        CancellationToken token
    )
    {
        using var logger = Logger
            .GetSubLogger(state.ActorInfo.Assembly.ToString())
            .GetSubLogger(nameof(CreateInitialLink))
            .GetSubLogger(state.ActorInfo.Actor.MetadataName);

        logger.Log($"{state.ActorInfo.Actor}:");
        logger.Log($" - {state.Implementation}");
        logger.Log($" - {state.Link.Entry.Type.ReferenceName} : {state.Link.ParentEntry?.Type.ReferenceName}");
        logger.Log($" - {state.Link.Parents}");

        var type = TypeSpec
            .From(state.Link.Entry.Type)
            .AddModifiers("partial");

        state.Implementation?.Interface.Apply(ref type);

        if (state.Link.Parents.Count == 0)
        {
            type = type.AddBases(
                $"{state.Link.Actor.State.ActorInfo.Actor}.Link"
            );

            switch (state.Link.ActorInfo.Assembly)
            {
                case LinkActorTargets.AssemblyTarget.Core:
                    type = type.AddBases(
                        $"{state.Link.ActorInfo.FormattedLinkType}.{string.Join(".", state.Link.Parts)}"
                    );
                    break;
                case LinkActorTargets.AssemblyTarget.Rest:
                    type = type.AddBases(
                        state.Link.ActorInfo.FormattedRestLinkType
                    );
                    break;
            }
        }
        else
        {
            type = type.AddBases(state.Link.GetCartesianBases());
        }

        return new StatefulGeneration<ImplementedState>(
            state,
            type
        );
    }

    private static IEnumerable<State> CreateState(
        (StatefulGeneration<ActorState> State, ImmutableArray<LinkSchematics.Schematic> Schematics) tuple,
        CancellationToken token
    )
    {
        foreach (var state in from schematic in tuple.Schematics
                 from entry in schematic.Root.Children
                 from state in CreateStateForEntry(entry)
                 select state)
            yield return state;
        yield break;

        IEnumerable<State> CreateStateForEntry(
            LinkSchematics.Entry entry,
            ImmutableEquatableArray<LinkSchematics.Entry>? parents = null)
        {
            var state = new State(tuple.State, entry, parents);

            yield return state;

            var childParents = parents?.Add(entry) ?? new([entry]);

            foreach (var child in entry.Children)
            foreach (var childState in CreateStateForEntry(child, childParents))
                yield return childState;
        }
    }
}