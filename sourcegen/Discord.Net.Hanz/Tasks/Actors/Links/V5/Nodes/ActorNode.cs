using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;

public class ActorNode : Node
{
    public readonly record struct State(
        string? UserSpecifiedRelationshipName,
        ActorInfo ActorInfo
    ) : IHasActorInfo
    {
        public static State Create(LinksV5.NodeContext context, CancellationToken token)
        {
            return new State(
                UserSpecifiedRelationshipName: context.Target.Actor
                    .GetAttributes()
                    .FirstOrDefault(x => x.AttributeClass?.Name == "RelationshipNameAttribute")
                    ?.ConstructorArguments[0].Value as string,
                ActorInfo: ActorInfo.Create(context)
            );
        }
    }

    public readonly record struct BuildState(
        State State,
        ActorAncestralInfo AncestralInfo
    ) : IHasActorInfo
    {
        public ActorInfo ActorInfo => State.ActorInfo;
    }
    
    public readonly record struct ActorAncestralInfo(
        string Actor,
        ImmutableEquatableArray<string> EntityAssignableAncestors,
        ImmutableEquatableArray<string> Ancestors
    );

    public IncrementalValuesProvider<State> StateProvider { get; }
    public IncrementalValuesProvider<StatefulGeneration<BuildState>> TypeProvider { get; }


    public ActorNode(
        IncrementalValuesProvider<LinksV5.NodeContext> context
    ) : base(context)
    {
        StateProvider = context.Select(State.Create);

        TypeProvider = StateProvider
            .Combine(context
                .Collect()
                .Select(MapAncestralInfo)
            )
            .Select((x, _) =>
                new BuildState(
                    x.Left,
                    x.Right.FirstOrDefault(y => y.Actor == x.Left.ActorInfo.Actor.DisplayString)
                )
            )
            .Select(Build);

        TypeProvider = AddChildren(
            TypeProvider,
            typeof(BackLinkNode),
            typeof(ExtensionNode)
        );
    }

    private static ImmutableEquatableArray<ActorAncestralInfo> MapAncestralInfo(
        ImmutableArray<LinksV5.NodeContext> contexts,
        CancellationToken token)
    {
        return new ImmutableEquatableArray<ActorAncestralInfo>(
            from context in contexts
            let ancestors = contexts.Where(x =>
                Hierarchy.Implements(context.Target.Actor, x.Target.Actor)
            ).ToArray()
            select new ActorAncestralInfo(
                context.Target.Actor.ToDisplayString(),
                ancestors
                    .Where(x =>
                        x.Target.Entity.Equals(context.Target.Entity, SymbolEqualityComparer.Default) ||
                        Hierarchy.Implements(context.Target.Entity, x.Target.Entity)
                    )
                    .Select(x => x.Target.Actor.ToDisplayString())
                    .ToImmutableEquatableArray(),
                ancestors.Select(x => x.Target.Actor.ToDisplayString()).ToImmutableEquatableArray()
            )
        );
    }

    public StatefulGeneration<BuildState> Build(BuildState state, CancellationToken token)
    {
        var type = TypeSpec.From(state.ActorInfo.Actor) with
        {
            Modifiers = new(["partial"]),
        };

        return new StatefulGeneration<BuildState>(
            state,
            type
        );
    }
}