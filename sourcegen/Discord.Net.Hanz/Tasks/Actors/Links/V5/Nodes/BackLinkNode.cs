using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;

public class BackLinkNode : Node, INestedNode
{
    public readonly record struct State<TParentState>(
        TParentState Parent
    ) where TParentState : IHasActorInfo
    {
        public bool IsRoot => Parent is ActorNode.State;
    }

    public BackLinkNode(IncrementalValuesProvider<LinksV5.NodeContext> context, Logger logger) : base(context, logger)
    {
    }

    public IncrementalValuesProvider<StatefulGeneration<TState>> From<TState>(
        IncrementalValuesProvider<StatefulGeneration<TState>> provider
    ) where TState : IHasActorInfo
    {
        return provider
            .Select(CreateState)
            .Select((context, token) =>
                Build(context.State, context.Spec, token)
            );
    }

    private static StatefulGeneration<State<TParentState>> CreateState<TParentState>(
        StatefulGeneration<TParentState> parent,
        CancellationToken token)
        where TParentState : IHasActorInfo
    {
        return new StatefulGeneration<State<TParentState>>(new State<TParentState>(parent.State), parent.Spec);
    }

    private static StatefulGeneration<TParent> Build<TParent>(
        State<TParent> state,
        TypeSpec parent,
        CancellationToken token)
        where TParent : IHasActorInfo
    {
        return new StatefulGeneration<TParent>(
            state.Parent,
            parent.AddNestedType(CreateBackLinkInterface())
        );
        
        // if (state.Parent.ActorInfo.IsCore)
        // {
        //     return new StatefulGeneration<TParent>(
        //         state.Parent,
        //         parent.AddNestedType(CreateBackLinkInterface())
        //     );
        // }
    }

    private static TypeSpec CreateBackLinkInterface()
        => new TypeSpec("BackLink", TypeKind.Interface)
        {
            Generics = new(["TSource"]),
            GenericConstraints = new([
                ("TSource", ["class", "IPathable"])
            ])
        };
}