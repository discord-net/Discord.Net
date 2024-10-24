using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;

public class HierarchyNode :
    Node,
    INestedTypeProducerNode
{
    public readonly record struct HierarchyContext(
        string Actor,
        ImmutableEquatableArray<ActorInfo> Hierarchy
    )
    {
        public static HierarchyContext? Create(
            LinkActorTargets.GenerationTarget context,
            NodeProviders.Hierarchy hierarchyInfo)
        {
            var attribute = context.GetCoreActor()
                .GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == "LinkHierarchicalRootAttribute");

            if (attribute is null) return null;

            var types = attribute.NamedArguments
                .FirstOrDefault(x => x.Key == "Types")
                .Value;

            var hierarchy = types.Kind is not TypedConstantKind.Error
                ? types.Kind switch
                {
                    TypedConstantKind.Array => types
                        .Values
                        .Select(x =>
                            hierarchyInfo.Children.FirstOrDefault(y =>
                                y.Actor.DisplayString == ((INamedTypeSymbol) x.Value!).ToDisplayString())
                        )
                        .Where(x => x != default)
                        .ToImmutableEquatableArray(),
                    _ => []
                }
                : hierarchyInfo.Children.ToImmutableEquatableArray();

            if (hierarchy.Count == 0) return null;

            return new HierarchyContext(
                context.Actor.ToDisplayString(),
                hierarchy
            );
        }
    }

    public readonly record struct BuildContext(
        ActorInfo ActorInfo,
        TypePath Path,
        bool IsTemplate,
        ImmutableEquatableArray<ActorInfo> Properties,
        ImmutableEquatableArray<string> Bases,
        ImmutableEquatableArray<string?> Overloads)
    {
        public TypePath Path { get; } = Path.Add<HierarchyNode>("Hierarchy");
        
        public IEnumerable<PropertySpec> GetPropertySpecs()
        {
            var actor = ActorInfo;
            var isTemplate = IsTemplate;
            var relativePath = Path.FormatRelative();
            var overloads = Overloads;

            return Properties.SelectMany(IEnumerable<PropertySpec> (x) =>
            [
                new(
                    Type: isTemplate
                        ? x.FormattedLink
                        : $"{x.Actor}.{relativePath}",
                    Name: GetFriendlyName(x.Actor)
                ),
                ..overloads.Select(path =>
                    new PropertySpec(
                        Type: path is null
                            ? x.FormattedLink
                            : $"{x.Actor}.{path}",
                        Name: GetFriendlyName(x.Actor),
                        ExplicitInterfaceImplementation: path is null
                            ? $"{actor}.Hierarchy"
                            : $"{actor}.{path}.Hierarchy",
                        Expression: GetFriendlyName(x.Actor)
                    )
                )
            ]);
        }

        public static BuildContext Create(NestedTypeProducerContext parameters, HierarchyContext context)
        {
            var bases = new List<string>();
            var overloads = new List<string?>();

            var isTemplate = parameters.Path.Last?.Type == typeof(ActorNode);
            
            if (isTemplate)
            {
                bases.Add($"{parameters.Path}");
                bases.Add($"{parameters.ActorInfo.Actor}.Hierarchy");
                overloads.Add(null);

                for (int i = 0; i < parameters.Path.Count - 1; i++)
                {
                    var path = parameters.Path.Slice(0, i);
                    bases.Add($"{path}.Hierarchy");
                    overloads.Add(path.ToString());
                }

            }

            return new(
                parameters.ActorInfo,
                parameters.Path,
                isTemplate,
                context.Hierarchy,
                bases.ToImmutableEquatableArray(),
                overloads.ToImmutableEquatableArray()
            );
        }
    }

    private readonly IncrementalValueProvider<Grouping<string, HierarchyContext>> _hierarchyProvider;

    public HierarchyNode(NodeProviders providers, Logger logger) : base(providers, logger)
    {
        _hierarchyProvider = providers
            .Actors
            .Pair(
                providers
                    .ActorHierarchy
                    .GroupBy(x => x.Actor),
                x => x.Actor.ToDisplayString(),
                HierarchyContext.Create
            )
            .WhereNonNull()
            .GroupBy(x => x.Actor);
    }

    public IncrementalValuesProvider<Branch<TypeSpec>> Create<TParent>(
        IncrementalValuesProvider<Branch<(NestedTypeProducerContext Parameters, TParent Source)>>
            provider)
    {
        var hierarchyProvider = provider
            .Select((x, _) => x.Parameters)
            .Pair(
                _hierarchyProvider,
                x => x.Value.ActorInfo.Actor.DisplayString,
                (branch, context) => branch.Mutate(BuildContext.Create(branch.Value, context))
            )
            .Select(Build);
        
        return AddNestedTypes(
                GetInstance<BackLinkNode>(),
                hierarchyProvider,
                (context, _) =>
                    new NestedTypeProducerContext(
                        context.State.ActorInfo,
                        context.State.Path
                    ),
                (context, specs, _) =>
                    context with
                    {
                        Spec = context.Spec.AddNestedTypes(specs)
                    },
                context => context.State
            )
            .Select((x, _) => x.Spec);
    }

    private StatefulGeneration<BuildContext> Build(BuildContext context, CancellationToken token)
    {
        var spec = new TypeSpec(
            Name: "Hierarchy",
            Kind: TypeKind.Interface,
            Properties: new(context.GetPropertySpecs()),
            Bases: context.Bases
        );

        return new StatefulGeneration<BuildContext>(context, spec);
    }
}