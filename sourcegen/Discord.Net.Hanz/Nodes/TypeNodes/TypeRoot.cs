using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Nodes.TypeNodes;

public readonly record struct TypeRootProviders<TSource>(
    IncrementalGroupingProvider<TSource, TypeSpec> SpecProvider,
    IncrementalValueProvider<IIntrospectionGraph> IntrospectionGraphProvider,
    IncrementalKeyValueProvider<TSource, IntrospectionSourceTree<TSource>> IntrospectionTreeProvider
);

public class NestedTypeRoot<TSource> : ProviderNesting<TSource>
{
    private readonly IncrementalValuesProvider<(TSource Source, TypePath Path)> _provider;

    public NestedTypeRoot(IncrementalValuesProvider<(TSource, TypePath Path)> provider)
    {
        _provider = provider;
    }

    public TypeRootProviders<TSource> Build(ILogger logger)
    {
        var nodes = Children
            .Select(x => x(_provider))
            .ToArray();

        var introspectionTreesProvider = nodes
            .Select(x => x.GetStateTreeProvider())
            .Aggregate((a, b) => a.Combine(b))
            .Select((source, trees) => new IntrospectionSourceTree<TSource>(source, trees));


        var introspectionProvider = introspectionTreesProvider
            .Collect()
            .Select(IIntrospectionGraph (x, _) =>
            {
                var graph = new IntrospectionGraph<TSource>(x);
                LogGraph(graph, logger);
                return graph;
            });

        return new(
            nodes
                .Select(x => x.GetTypeSpecProvider(introspectionProvider))
                .Aggregate(
                    (a, b) => a.Combine(b)
                )
                .MapValues((_, x) => x.Spec),
            introspectionProvider,
            introspectionTreesProvider.KeyedBy(x => x.Source)
        );
    }

    private static void LogGraph(IntrospectionGraph<TSource> graph, ILogger logger)
    {
        if(graph.Trees.Length > 0)
            logger.Clean();
        
        try
        {
            logger.Log($"{graph.Trees.Length} trees:");

            for (var i = 0; i < graph.Trees.Length; i++)
            {
                var tree = graph.Trees[i];
                logger.Log($" - tree[{i}] ({tree.Trees.Length} subtrees): {tree.Source}");

                foreach (var stateTree in tree.Trees)
                {
                    LogStateTree(stateTree, 1);
                }
            }

            void LogStateTree(StateTree tree, int depth)
            {
                logger.Log($" - {tree.Node.GetType()}:".Prefix(depth * 2));
                logger.Log($"   - Path: {tree.Generation.Path}:".Prefix(depth * 2));
                logger.Log($"   - State: {tree.Generation.GetState()}:".Prefix(depth * 2));
                logger.Log($"   - Children: {tree.Children.Count}:".Prefix(depth * 2));

                foreach (var child in tree.Children)
                {
                    LogStateTree(child, depth + 2);
                }
            }
        }
        finally
        {
            logger.Flush();
        }
    }
}