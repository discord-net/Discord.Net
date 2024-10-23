using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;

public class BackLinkNode : Node, INestedNode
{
    public readonly record struct State<TParentState>(
        TParentState Parent
    ) :
        ITypePath,
        IHasActorInfo
        where TParentState : IHasActorInfo
    {
        public bool IsRoot => Parent is ActorNode.IntrospectedBuildState;

        public bool IsClass => IsRoot && Parent.ActorInfo.Actor.TypeKind is TypeKind.Class;

        public bool IsCovariant => !IsClass;

        public string TypeName
            => $"BackLink<{(IsCovariant ? "out " : string.Empty)}TSource>";

        public ImmutableEquatableArray<string> Parts => [..Parent is ITypePath path ? path.Parts : [], TypeName];

        public ActorInfo ActorInfo => Parent.ActorInfo;
    }

    public BackLinkNode(NodeProviders providers, Logger logger) : base(providers, logger)
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
            parent.AddNestedType(CreateBackLinkInterface(state))
        );

        // if (state.Parent.ActorInfo.IsCore)
        // {
        //     return new StatefulGeneration<TParent>(
        //         state.Parent,
        //         parent.AddNestedType(CreateBackLinkInterface())
        //     );
        // }
    }

    private static TypeSpec CreateBackLinkInterface<TParent>(
        State<TParent> state
    ) where TParent : IHasActorInfo
    {
        var spec =
            new TypeSpec(
                Name: "BackLink",
                Kind: TypeKind.Interface,
                Generics: new(["TSource"]),
                GenericConstraints: new([
                    ("TSource", ["class", "IPathable"])
                ]),
                Bases: new([state.ActorInfo.FormattedBackLinkType])
            );

        if (state.Parent is ITypePath path)
        {
            spec = spec.AddBases(
                $"{state.ActorInfo.Actor}.{string.Join(".", path.Parts)}"
            );
        }
        
        if (!state.ActorInfo.IsCore)
        {
            spec = spec.AddBases(
                $"{state.ActorInfo.CoreActor}.{string.Join(".", state.Parts)}"
            );
        }

        return spec;
    }
}