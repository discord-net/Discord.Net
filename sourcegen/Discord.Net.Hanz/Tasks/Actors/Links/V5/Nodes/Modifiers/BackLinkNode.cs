using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;

public class BackLinkNode :
    Node,
    INestedTypeProducerNode
{
    public readonly record struct State<TParent>(
        ActorInfo ActorInfo,
        TypePath Path,
        TParent Parent,
        bool IsCovariant,
        string TypeName
    )
    {
        public TypePath Path { get; } = Path.Add<BackLinkNode>(TypeName);
        public bool IsRoot { get; } = Path.Last?.Type == typeof(ActorNode);

        public static State<TParent> Create(NestedTypeProducerContext context, TParent parent)
        {
            var isCovariant = context.Path.Last?.Type != typeof(ActorNode) || context.ActorInfo.IsCore;

            return new State<TParent>(
                ActorInfo: context.ActorInfo,
                Path: context.Path,
                Parent: parent,
                IsCovariant: isCovariant,
                $"BackLink<{(isCovariant ? "out " : string.Empty)}TSource>"
            );
        }
    }

    public BackLinkNode(NodeProviders providers, Logger logger) : base(providers, logger)
    {
    }

    public IncrementalValuesProvider<Branch<TypeSpec>> Create<TSource>(
        IncrementalValuesProvider<Branch<(NestedTypeProducerContext Parameters, TSource Source)>> provider)
    {
        return provider
            .Select((context, token) => State<TSource>.Create(context.Parameters, context.Source))
            .Select(Build);
    }

    private TypeSpec Build<TParent>(State<TParent> state, CancellationToken token)
    {
        using var logger = Logger
            .GetSubLogger(state.ActorInfo.Assembly.ToString())
            .GetSubLogger(nameof(Build))
            .GetSubLogger(state.ActorInfo.Actor.MetadataName);
        
        logger.Log($"Building {typeof(TParent).Name} -> {state.Path}");
        
        var spec =
            new TypeSpec(
                Name: "BackLink",
                Kind: TypeKind.Interface,
                Generics: new([
                    ("TSource", state.IsCovariant ? VarianceKind.Out : VarianceKind.None)
                ]),
                GenericConstraints: new([
                    ("TSource", ["class", "IPathable"])
                ]),
                Bases: new([state.ActorInfo.FormattedBackLinkType])
            );

        if (!state.IsRoot)
        {
            spec = spec.AddBases(state.Path.FormatParent());

            for (var i = 2; i < state.Path.Count - 1; i++)
            {
                spec = spec.AddBases(
                    $"{state.Path.Format(from: 1, to: i)}.BackLink<TSource>"
                );
            }
        }

        switch (state.Parent)
        {
            case HierarchyNode.BuildContext hierarchy:
                BuildOntoHierarchy(ref spec, hierarchy, state);
                break;
            case ExtensionNode.ExtensionContext extension:
                BuildOntoExtension(ref spec, extension, state, logger);
                break;
        }

        return spec;
    }

    private void BuildOntoExtension<TParent>(
        ref TypeSpec spec,
        ExtensionNode.ExtensionContext context,
        State<TParent> state,
        Logger logger
    )
    {
        logger.Log($"Extension properties for {state.Path}:");

        foreach (var property in context.Extension.Properties)
        {
            logger.Log($" - {property.Name} -> {property.IsDefinedOnPath(state.Path)}");
        }
        
        spec = spec.AddProperties(
            context.Extension.Properties.SelectMany(property =>
                ExtensionNode.BuildExtensionProperty(
                    state.Path,
                    property,
                    context.Extension
                )
            )
        );
    }

    private void BuildOntoHierarchy<TParent>(
        ref TypeSpec spec,
        HierarchyNode.BuildContext hierarchy,
        State<TParent> state)
    {
        spec = spec.AddProperties(
            hierarchy.Properties.SelectMany(IEnumerable<PropertySpec> (x) =>
            [
                new(
                    Type: hierarchy.IsTemplate
                        ? x.FormattedBackLinkType
                        : $"{x.Actor}.{hierarchy.Path.FormatRelative()}.BackLink<TSource>",
                    Name: GetFriendlyName(x.Actor),
                    Modifiers: new(["new"])
                ),
                new(
                    Type: hierarchy.IsTemplate
                        ? x.FormattedLink
                        : $"{x.Actor}.{hierarchy.Path.FormatRelative()}",
                    Name: GetFriendlyName(x.Actor),
                    ExplicitInterfaceImplementation: hierarchy.IsTemplate
                        ? $"{state.ActorInfo.Actor}.Hierarchy"
                        : $"{hierarchy.Path}.Hierarchy",
                    Expression: GetFriendlyName(x.Actor)
                ),
                ..hierarchy.Overloads.Select(path =>
                    new PropertySpec(
                        Type: path is null
                            ? x.FormattedBackLinkType
                            : $"{x.Actor}.{path}.Hierarchy.BackLink<TSource>",
                        Name: GetFriendlyName(x.Actor),
                        ExplicitInterfaceImplementation:
                        $"{hierarchy.ActorInfo}.{path}.Hierarchy.BackLink<TSource>",
                        Expression: GetFriendlyName(x.Actor)
                    )
                )
            ])
        );
    }

    // public readonly record struct State<TParentState>(
    //     TParentState Parent
    // ) :
    //     ITypePath,
    //     IHasActorInfo
    //     where TParentState : IHasActorInfo
    // {
    //     public bool IsRoot => Parent is ActorNode.IntrospectedBuildState;
    //
    //     public bool IsClass => IsRoot && Parent.ActorInfo.Actor.TypeKind is TypeKind.Class;
    //
    //     public bool IsCovariant => !IsClass;
    //
    //     public string TypeName
    //         => $"BackLink<{(IsCovariant ? "out " : string.Empty)}TSource>";
    //
    //     public ImmutableEquatableArray<string> Parts => [..Parent is ITypePath path ? path.Parts : [], TypeName];
    //
    //     public ActorInfo ActorInfo => Parent.ActorInfo;
    // }
    //
    // public BackLinkNode(NodeProviders providers, Logger logger) : base(providers, logger)
    // {
    // }
    //
    // public IncrementalValuesProvider<StatefulGeneration<TState>> From<TState>(
    //     IncrementalValuesProvider<StatefulGeneration<TState>> provider
    // ) where TState : IHasActorInfo
    // {
    //     return provider
    //         .Select(CreateState)
    //         .Select((context, token) =>
    //             Build(context.State, context.Spec, token)
    //         );
    // }
    //
    // private static StatefulGeneration<State<TParentState>> CreateState<TParentState>(
    //     StatefulGeneration<TParentState> parent,
    //     CancellationToken token)
    //     where TParentState : IHasActorInfo
    // {
    //     return new StatefulGeneration<State<TParentState>>(new State<TParentState>(parent.State), parent.Spec);
    // }
    //
    // private static StatefulGeneration<TParent> Build<TParent>(
    //     State<TParent> state,
    //     TypeSpec parent,
    //     CancellationToken token)
    //     where TParent : IHasActorInfo
    // {
    //     return new StatefulGeneration<TParent>(
    //         state.Parent,
    //         parent.AddNestedType(CreateBackLinkInterface(state))
    //     );
    //
    //     // if (state.Parent.ActorInfo.IsCore)
    //     // {
    //     //     return new StatefulGeneration<TParent>(
    //     //         state.Parent,
    //     //         parent.AddNestedType(CreateBackLinkInterface())
    //     //     );
    //     // }
    // }
    //
    // private static TypeSpec CreateBackLinkInterface<TParent>(
    //     State<TParent> state
    // ) where TParent : IHasActorInfo
    // {
    //     var spec =
    //         new TypeSpec(
    //             Name: "BackLink",
    //             Kind: TypeKind.Interface,
    //             Generics: new(["TSource"]),
    //             GenericConstraints: new([
    //                 ("TSource", ["class", "IPathable"])
    //             ]),
    //             Bases: new([state.ActorInfo.FormattedBackLinkType])
    //         );
    //
    //     if (state.Parent is ITypePath path)
    //     {
    //         spec = spec.AddBases(
    //             $"{state.ActorInfo.Actor}.{string.Join(".", path.Parts)}"
    //         );
    //     }
    //     
    //     if (!state.ActorInfo.IsCore)
    //     {
    //         spec = spec.AddBases(
    //             $"{state.ActorInfo.CoreActor}.{string.Join(".", state.Parts)}"
    //         );
    //     }
    //
    //     return spec;
    // }
}