using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Types;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;

public class ActorNode : Node
{
    public readonly record struct State(
        string? UserSpecifiedRelationshipName,
        ActorInfo ActorInfo,
        ImmutableEquatableArray<string> InheritedCanonicalRelationships
    ) : IHasActorInfo
    {
        public static State Create(LinksV5.NodeContext context, CancellationToken token)
        {
            return new State(
                UserSpecifiedRelationshipName: context.Target.Actor
                    .GetAttributes()
                    .FirstOrDefault(x => x.AttributeClass?.Name == "RelationshipNameAttribute")
                    ?.ConstructorArguments[0].Value as string,
                ActorInfo: ActorInfo.Create(context),
                InheritedCanonicalRelationships: new(
                    context.Target.GetCoreActor()
                        .Interfaces
                        .Where(x => x.ToString().EndsWith("CanonicalRelationship"))
                        .Select(x => x.Name.Replace(".CanonicalRelationship", string.Empty))
                )
            );
        }
    }

    public readonly record struct BuildState(
        State State,
        ActorAncestralInfo AncestralInfo
    ) : IHasActorInfo
    {
        public ActorInfo ActorInfo => State.ActorInfo;

        public TypePath Path { get; } = new(new([(typeof(ActorNode), State.ActorInfo.Actor.FullyQualifiedName)]));

        public bool RedefinesRootInterfaceMemebrs =>
            !State.ActorInfo.IsCore || AncestralInfo.EntityAssignableAncestors.Count > 0;
    }

    public readonly record struct IntrospectedBuildState(
        BuildState BuildState,
        ImmutableEquatableArray<IntrospectedBuildState> Ancestors,
        ImmutableEquatableArray<IntrospectedBuildState> EntityAssignableAncestors
    ) : IHasActorInfo
    {
        public ActorInfo ActorInfo => BuildState.ActorInfo;

        public bool RedefinesRootInterfaceMemebrs =>
            !BuildState.ActorInfo.IsCore || EntityAssignableAncestors.Count > 0;

        public bool CanonicalRelationshipIsRedefined
        {
            get
            {
                var ours = RelationshipName;
                return Ancestors.Any(x => x.RelationshipName == ours);
            }
        }

        public string RelationshipName =>
            BuildState.State.UserSpecifiedRelationshipName
            ??
            Ancestors
                .Select(x => x.BuildState.State.UserSpecifiedRelationshipName)
                .FirstOrDefault(x => x is not null)
            ??
            GetFriendlyName(BuildState.ActorInfo.Actor);
    }

    public readonly record struct ActorAncestralInfo(
        string Actor,
        ImmutableEquatableArray<string> EntityAssignableAncestors,
        ImmutableEquatableArray<string> Ancestors
    );

    public IncrementalValuesProvider<StatefulGeneration<IntrospectedBuildState>> TypeProvider { get; }

    public IncrementalValueProvider<Grouping<string, ActorAncestralInfo>> AncestralProvider { get; }

    public ActorNode(
        NodeProviders providers,
        Logger logger
    ) : base(providers, logger)
    {
        logger.Log("Initialized");

        AncestralProvider = providers
            .Context
            .Collect()
            .Select(MapAncestralInfo)
            .GroupBy(x => x.Actor);

        var buildProvider = providers
            .Context
            .Select(State.Create)
            .Combine(
                AncestralProvider,
                x => x.ActorInfo.Actor.DisplayString,
                (state, ancestors, token) => new BuildState(
                    state,
                    ancestors[0]
                )
            )
            .Select(CreatePartialContainer);

        buildProvider = AddNestedTypes(
            buildProvider,
            (build, _) => new(build.State.ActorInfo, build.Path),
            GetInstance<HierarchyNode>(),
            GetInstance<BackLinkNode>(),
            GetInstance<ExtensionNode>(),
            GetInstance<LinkNode>()
        );
        
        TypeProvider = buildProvider
            .Collect()
            .SelectMany(Introspect)
            .Select(CreateLinkInterface)
            .Select(CreateRelationshipsTypes);

        logger.Flush();
    }

    private static IEnumerable<StatefulGeneration<IntrospectedBuildState>> Introspect(
        ImmutableArray<StatefulGeneration<BuildState>> states,
        CancellationToken token
    )
    {
        var table = new Dictionary<string, IntrospectedBuildState>();

        foreach (var build in states)
        {
            yield return new StatefulGeneration<IntrospectedBuildState>(
                CreateIntrospected(build.State),
                build.Spec
            );

            token.ThrowIfCancellationRequested();
        }

        yield break;

        IntrospectedBuildState Find(string name)
        {
            if (table.TryGetValue(name, out var state))
                return state;

            return table[name] = CreateIntrospected(
                states
                    .First(x => x.State.ActorInfo.Actor.DisplayString == name)
                    .State
            );
        }

        IntrospectedBuildState CreateIntrospected(
            BuildState context)
        {
            var ancestors = context.AncestralInfo.Ancestors.Select(Find).ToArray();

            return new IntrospectedBuildState(
                BuildState: context,
                Ancestors: new ImmutableEquatableArray<IntrospectedBuildState>(
                    ancestors
                ),
                EntityAssignableAncestors: new ImmutableEquatableArray<IntrospectedBuildState>(
                    ancestors.Where(x => context
                        .AncestralInfo
                        .EntityAssignableAncestors
                        .Contains(x.ActorInfo.Actor.DisplayString)
                    )
                )
            );
        }
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

    public StatefulGeneration<BuildState> CreatePartialContainer(BuildState state, CancellationToken token)
    {
        var logger = Logger
            .GetSubLogger(state.ActorInfo.Assembly.ToString())
            .GetSubLogger(state.ActorInfo.Actor.MetadataName)
            .GetSubLogger(nameof(CreatePartialContainer))
            .WithCleanLogFile();

        logger.Log($"Generating: {state}");

        try
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
        finally
        {
            logger.Flush();
        }
    }

    public StatefulGeneration<IntrospectedBuildState> CreateRelationshipsTypes(
        StatefulGeneration<IntrospectedBuildState> context,
        CancellationToken token)
    {
        if (!context.State.ActorInfo.IsCore) return context;

        return context with
        {
            Spec = context.Spec
                .AddNestedType(CreateRelationshipType(context.State))
                .AddNestedType(CreateCanonicalRelationshipType(context.State))
        };
    }

    private TypeSpec CreateCanonicalRelationshipType(IntrospectedBuildState state)
    {
        var type =
            new TypeSpec(
                    "CanonicalRelationship",
                    TypeKind.Interface,
                    Bases: new([
                        "Relationship",
                        state.ActorInfo.FormattedCanonicalRelationship
                    ])
                )
                .AddBases(state.Ancestors.Select(x => $"{x.ActorInfo.Actor}.CanonicalRelationship"));

        foreach (var ancestor in state.Ancestors)
        {
            type = type
                .AddInterfacePropertyOverload(
                    ancestor.ActorInfo.Actor.FullyQualifiedName,
                    $"{ancestor.ActorInfo.Actor}.Relationship",
                    ancestor.RelationshipName,
                    state.RelationshipName
                );
        }

        // conflicts
        foreach
        (
            var group
            in state.Ancestors
                .Prepend(state)
                .GroupBy(
                    x => x.ActorInfo.Entity,
                    (key, x) => (Entity: key, Nodes: x.ToArray())
                )
                .Where(x => x.Nodes.Length > 1)
        )
        {
            var node = group.Nodes[0];
            type = type
                .AddInterfacePropertyOverload(
                    node.ActorInfo.Id.FullyQualifiedName,
                    node.ActorInfo.FormattedRelation,
                    "RelationshipId",
                    $"{node.RelationshipName}.Id"
                );
        }

        if (state.CanonicalRelationshipIsRedefined)
        {
            type = type
                .AddProperties(
                    new PropertySpec(
                        state.ActorInfo.Actor.FullyQualifiedName,
                        state.RelationshipName,
                        Accessibility.Internal,
                        new(["new"])
                    )
                )
                .AddInterfacePropertyOverload(
                    state.ActorInfo.Actor.FullyQualifiedName,
                    $"{state.ActorInfo.Actor}.Relationship",
                    state.RelationshipName,
                    state.RelationshipName
                );
        }

        return type
            .AddBases(state
                .Ancestors
                .Select(x => $"{x.ActorInfo.Actor}.CanonicalRelationship")
            );
    }

    private TypeSpec CreateRelationshipType(IntrospectedBuildState state)
    {
        return
            new TypeSpec(
                    "Relationship",
                    TypeKind.Interface,
                    Bases: new([state.ActorInfo.FormattedRelationship])
                )
                .AddInterfacePropertyOverload(
                    state.ActorInfo.Actor.FullyQualifiedName,
                    state.ActorInfo.FormattedRelationship,
                    "RelationshipActor",
                    state.RelationshipName
                )
                .AddProperties(new PropertySpec(
                    state.ActorInfo.Actor.FullyQualifiedName,
                    state.RelationshipName,
                    Accessibility.Internal
                ));
    }

    public StatefulGeneration<IntrospectedBuildState> CreateLinkInterface(
        StatefulGeneration<IntrospectedBuildState> context,
        CancellationToken token)
    {
        var linkType = new TypeSpec(
            "Link",
            TypeKind.Interface,
            Bases: new([
                context.State.ActorInfo.FormattedLink
            ])
        );

        if (!context.State.ActorInfo.IsCore)
            linkType = linkType.AddBases(context.State.ActorInfo.FormattedCoreLink);

        if (context.State.RedefinesRootInterfaceMemebrs)
        {
            linkType = linkType
                .AddInterfaceMethodOverload(
                    context.State.ActorInfo.Actor.FullyQualifiedName,
                    context.State.ActorInfo.FormattedActorProvider,
                    "GetActor",
                    [
                        new ParameterSpec(
                            context.State.ActorInfo.Id.FullyQualifiedName,
                            "id"
                        )
                    ],
                    expression: "GetActor(id)"
                )
                .AddInterfaceMethodOverload(
                    context.State.ActorInfo.Entity.FullyQualifiedName,
                    context.State.ActorInfo.FormattedEntityProvider,
                    "CreateEntity",
                    [
                        new ParameterSpec(
                            context.State.ActorInfo.Model.FullyQualifiedName,
                            "model"
                        )
                    ],
                    expression: "CreateEntity(model)"
                );

            if (!context.State.ActorInfo.IsCore)
            {
                linkType = linkType
                    .AddInterfaceMethodOverload(
                        context.State.ActorInfo.CoreActor.FullyQualifiedName,
                        context.State.ActorInfo.FormattedCoreActorProvider,
                        "GetActor",
                        [
                            new ParameterSpec(
                                context.State.ActorInfo.Id.FullyQualifiedName,
                                "id"
                            )
                        ],
                        expression: "GetActor(id)"
                    )
                    .AddInterfaceMethodOverload(
                        context.State.ActorInfo.CoreEntity.FullyQualifiedName,
                        context.State.ActorInfo.FormattedCoreEntityProvider,
                        "CreateEntity",
                        [
                            new ParameterSpec(
                                context.State.ActorInfo.Model.FullyQualifiedName,
                                "model"
                            )
                        ],
                        expression: "CreateEntity(model)"
                    );
            }
        }

        foreach (var ancestor in context.State.Ancestors)
        {
            var ancestorActorProviderTarget = ancestor.RedefinesRootInterfaceMemebrs
                ? $"{ancestor.ActorInfo.Actor}.Link"
                : ancestor.ActorInfo.FormattedActorProvider;

            var ancestorEntityProviderTarget = ancestor.RedefinesRootInterfaceMemebrs
                ? $"{ancestor.ActorInfo.Actor}.Link"
                : ancestor.ActorInfo.FormattedEntityProvider;

            linkType = linkType
                .AddInterfaceMethodOverload(
                    ancestor.ActorInfo.Actor.FullyQualifiedName,
                    ancestorActorProviderTarget,
                    "GetActor",
                    [(ancestor.ActorInfo.Id.FullyQualifiedName, "id")],
                    expression: "GetActor(id)"
                )
                .AddInterfaceMethodOverload(
                    ancestor.ActorInfo.Entity.FullyQualifiedName,
                    ancestorEntityProviderTarget,
                    "CreateEntity",
                    [(ancestor.ActorInfo.Model.FullyQualifiedName, "model")],
                    expression: "CreateEntity(model)"
                );
        }

        return context with
        {
            Spec = context.Spec.AddNestedType(linkType)
        };
    }
}