using System.Collections.Concurrent;
using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

using LinkTarget = LinkActorTargets.GenerationTarget;

public abstract class LinkNode : IEquatable<LinkNode>
{
    public LinkNode? Parent { get; private set; }
    public List<LinkNode> Children { get; private set; }
    public LinkTarget Target { get; }

    public IEnumerable<LinkNode> Parents
    {
        get
        {
            var target = Parent;

            while (target is not null)
            {
                yield return target;
                target = target.Parent;
            }
        }
    }

    public ActorNode? RootActorNode => Parents.OfType<ActorNode>().FirstOrDefault();

    protected bool IsCore => Target.Assembly is LinkActorTargets.AssemblyTarget.Core;

    public string FormattedBackLinkType
        => $"Discord.IBackLink<TSource, {Target.Actor}, {Target.Id}, {Target.Entity}, {Target.Model}>";

    public string FormattedCoreBackLinkType
        =>
            $"Discord.IBackLink<TSource, {Target.GetCoreActor()}, {Target.Id}, {Target.GetCoreEntity()}, {Target.Model}>";

    public string FormattedLinkType
        => $"Discord.ILinkType<{Target.Actor}, {Target.Id}, {Target.Entity}, {Target.Model}>";

    public string FormattedCoreLinkType
        =>
            $"Discord.ILinkType<{Target.GetCoreActor()}, {Target.Id}, {Target.GetCoreEntity()}, {Target.Model}>";

    public string FormattedLink
        => $"Discord.ILink<{Target.Actor}, {Target.Id}, {Target.Entity}, {Target.Model}>";

    public string FormattedCoreLink
        => $"Discord.ILink<{Target.GetCoreActor()}, {Target.Id}, {Target.GetCoreEntity()}, {Target.Model}>";

    public string FormattedRestLinkType =>
        $"Discord.Rest.IRestLinkType<{Target.Actor}, {Target.Id}, {Target.Entity}, {Target.Model}>";

    public string FormattedActorProvider
        => $"Discord.IActorProvider<{Target.Actor}, {Target.Id}>";

    public string FormattedCoreActorProvider
        => $"Discord.IActorProvider<{Target.GetCoreActor()}, {Target.Id}>";

    public string FormattedRestActorProvider
        => $"Discord.Rest.RestActorProvider<{Target.Actor}, {Target.Id}>";

    public string FormattedEntityProvider
        => $"Discord.IEntityProvider<{Target.Entity}, {Target.Model}>";

    public string FormattedCoreEntityProvider
        => $"Discord.IEntityProvider<{Target.GetCoreEntity()}, {Target.Model}>";

    public HashSet<LinkNode> SemanticCompisition { get; } = [];
    public HashSet<LinkNode> ImplicitSemanticCompisition { get; } = [];

    public IEnumerable<IEnumerable<LinkTypeNode>> ParentLinkTypesProduct
        => GetProduct(Parents.OfType<LinkTypeNode>());

    public IEnumerable<IEnumerable<LinkTypeNode>> LinkTypesProduct
        => GetProduct(Parents.Prepend(this).OfType<LinkTypeNode>(), true)
            .Skip(1)
            .Select(x => x
                .Reverse()
            );

    public IEnumerable<LinkNode> ContainingNodes
    {
        get
        {
            if (RootActorNode is not { } actorNode) return [];

            return ParentsProduct
                .Select(x =>
                {
                    LinkNode current = actorNode;

                    foreach (var node in x.Reverse())
                    {
                        if (
                            current.Children
                                .FirstOrDefault(y =>
                                    y.GetType() == node.GetType() &&
                                    (
                                        (y is not LinkTypeNode a || node is not LinkTypeNode b ||
                                         a.Entry.Equals(b.Entry))
                                        &&
                                        (y is not ITypeProducerNode c || node is not ITypeProducerNode d ||
                                         c.GetTypeName().Equals(d.GetTypeName()))
                                    )
                                ) is not { } linkNode
                        ) return null;

                        current = linkNode;
                    }

                    return current;
                })
                .OfType<LinkNode>();
        }
    }

    protected IEnumerable<LinkNode> GetSemanticTypeNodeRelatives(Logger logger, bool excludeSelf = true)
        => GetSemanticTypeNodeCompisition(logger, excludeSelf)
            .Where(x => x.SemanticEquals(this) && (!excludeSelf || x != this))
            .OfType<LinkModifierNode>();

    protected HashSet<LinkNode> GetSemanticTypeNodeCompisition(
        Logger logger,
        bool excludeSelf = true,
        bool excludeParentCompisition = true)
    {
        if (RootActorNode is null) return [];

        // rules:
        // - Link Nodes are order-aware based on the schematic:
        //   - {A, B, C} ✅
        //   - {C, B, A} ❌
        //   - {A, B}    ✅
        //   - {B, C}    ✅
        //   - {A, C} is dependant on if A has a child of C.
        // - Hierarchy nodes never contain descendant nodes of other hierarchy nodes, they can contain other modifier
        //   nodes.
        // - Link Extensions can contain other extensions, extension A in B ensures that theres a variant of B in A
        //   on the closest non-extension parent. Generally, {{a,b} : a∈A ∧ b∈B} holds here.
        //   - {A, B} ✅
        //   - {B, A} ✅
        //   - {B}    ✅
        // - BackLinks are terminal, and don't have children.
        // - Actor nodes are terminal, and don't have parents.

        var product = new List<(Type SetType, List<List<LinkNode>> Nodes)>();

        var searchType = GetSearchType(this);
        var searchSet = new List<LinkNode>() {this};

        Type GetSearchType(LinkNode node)
        {
            if (node is LinkTypeNode)
                return typeof(LinkTypeNode);

            return node.GetType();
        }
        
        void CalculateSearchProduct()
        {
            searchSet.Reverse();

            product.Add(
                (
                    searchType,
                    searchSet.Last() switch
                    {
                        LinkTypeNode or LinkExtensionNode => GetProduct(searchSet)
                            .Select(x => x.ToList())
                            .ToList(),
                        _ => [searchSet.ToList()]
                    }
                )
            );
        }

        foreach (var parent in Parents.Where(x => x is ITypeProducerNode))
        {
            if (parent is ActorNode or BackLinkNode) continue;

            var parentType = GetSearchType(parent);
            
            if (searchType != parentType)
            {
                CalculateSearchProduct();
                searchType = parentType;
                searchSet.Clear();
            }

            searchSet.Add(parent);
        }

        if (searchSet.Count > 0)
            CalculateSearchProduct();

        logger.Log($"Products with semantics: {product.Count}");

        product.Reverse();

        foreach (var entry in product)
        {
            logger.Log($" - {entry.SetType}: {entry.Nodes.Count}");

            var pads = new List<int>();

            var formatted = entry.Nodes
                .Select(x => x
                    .Select((x, i) =>
                    {
                        var line = $"{x.GetType().Name}: {((ITypeProducerNode) x).GetTypeName()}";

                        if (pads.Count <= i)
                            pads.Insert(i, line.Length);
                        else if (pads[i] < line.Length)
                            pads[i] = line.Length;

                        return line;
                    })
                    .ToArray()
                ).ToArray();

            var maxElements = formatted.Select(x => x.Length).Max();
            
            foreach (var set in formatted)
            {
                logger.Log(
                    $" - {{ {string
                        .Join(
                            " | ",
                            set.Select((x, i) => x
                                .PadRight(
                                    pads[i] + pads.Take(i - 1).Sum()
                                )
                            )
                        )
                        .PadRight(pads.Sum() + 3 * (maxElements - set.Length))
                    } }}"
                );
            }

            // foreach (var nodes in entry.Nodes)
            // {
            //     logger.Log(
            //         $"    - {{ {string.Join(" | ", nodes.Select(x => $"{x.GetType().Name}: {((ITypeProducerNode) x).GetTypeName()}"))} }}");
            // }
        }

        var result = new HashSet<LinkNode>();

        var bounds = (1 << product.Count) - 1;
        var boundsDigitSize = Convert.ToString(bounds, 2).Length;

        for (var sample = 1; sample <= bounds; sample++)
        {
            var sampleName = Convert.ToString(sample, 2).PadLeft(boundsDigitSize, '0');
            var searchNodes = new HashSet<LinkNode>() {RootActorNode};

            for (var index = 0; index != product.Count; index++)
            {
                if (searchNodes.Count == 0) break;

                var identity = (1 << index);
                var identityName = Convert.ToString(identity, 2).PadLeft(boundsDigitSize, '0');

                if ((identity & sample) == 0)
                {
                    logger.Log($"Sample {sampleName} ({sample}/{bounds}): {identityName} -> skipped");
                    continue;
                }

                var nodes = product[index].Nodes;

                var bag = new ConcurrentBag<LinkNode>();

                var sampleLocal = sample;
                
                Parallel.ForEach(searchNodes, searchNode =>
                {
                    for (var setIndex = 0; setIndex < nodes.Count; setIndex++)
                    {
                        var setSearchNode = searchNode;
                        var set = nodes[setIndex];

                        for (var i = 0; i < set.Count; i++)
                        {
                            var node = set[i];
                            if (node == RootActorNode) break;

                            if (setSearchNode?.Children.FirstOrDefault(x => x.SemanticEquals(node)) is not { } next)
                            {
                                logger.Log(
                                    $"Sample {sampleName} ({sampleLocal}/{bounds}): {identityName} -> set " +
                                    $"{setIndex + 1}/{nodes.Count} terminates at element {i}"
                                );
                                break;
                            }

                            setSearchNode = next;

                            if (i == set.Count - 1)
                            {
                                // if (result.Contains(next) || searchNodes.Contains(next))
                                // {
                                //     logger.Log(
                                //         $"Sample {sampleName} ({sample}/{bounds}): {identityName} -> set " +
                                //         $"{setIndex + 1}/{nodes.Count} completes but it was already found."
                                //     );
                                //     break;
                                // }

                                if (
                                    excludeParentCompisition &&
                                    (Parent?.SemanticCompisition.Contains(setSearchNode) ?? false))
                                {
                                    logger.Log(
                                        $"Sample {sampleName} ({sampleLocal}/{bounds}): {identityName} -> set " +
                                        $"{setIndex + 1}/{nodes.Count} completes but is excluded as its contained in the parent."
                                    );
                                    break;
                                }

                                if (excludeSelf && setSearchNode == this)
                                {
                                    logger.Log(
                                        $"Sample {sampleName} ({sampleLocal}/{bounds}): {identityName} -> set " +
                                        $"{setIndex + 1}/{nodes.Count} completes but is excluded as its the current node."
                                    );
                                    break;
                                }

                                bag.Add(setSearchNode);
                                logger.Log(
                                    $"Sample {sampleName} ({sampleLocal}/{bounds}): {identityName} -> set " +
                                    $"{setIndex + 1}/{nodes.Count} completes"
                                );
                            }
                        }
                    }
                });

                if (bag.Count == 0)
                {
                    logger.Log($"Sample {sampleName} ({sample}/{bounds}): ending at {index + 1}/{product.Count}");
                    searchNodes.Clear();
                    break;
                }

                logger.Log(
                    $"Sample {sampleName} ({sample}/{bounds}): {identityName} -> {bag.Count} nodes from {searchNodes.Count}");

                searchNodes.Clear();
                searchNodes.UnionWith(bag);
            }

            logger.Log($"Sample: {sampleName} ({sample}/{bounds}): {searchNodes.Count} new nodes:");

            foreach (var node in searchNodes)
            {
                logger.Log($" - {node.FormatAsTypePath()} ({node.GetType().Name})");
            }

            result.UnionWith(searchNodes);
        }

        logger.Log("Result:");

        foreach (var node in result)
        {
            logger.Log($" - {node.FormatAsTypePath()} ({node.GetType().Name})");
        }

        return result;
    }

    public bool SemanticEquals(LinkNode node)
    {
        return
            GetType() == node.GetType()
            && (this, node) switch
            {
                (LinkTypeNode a, LinkTypeNode b) =>
                (
                    a.Entry.Symbol.ToDisplayString().Equals(b.Entry.Symbol.ToDisplayString())
                ),
                (ITypeImplementerNode a, ITypeImplementerNode b) =>
                (
                    a.WillGenerateImplementation == b.WillGenerateImplementation &&
                    a.GetTypeName().Equals(b.GetTypeName())
                ),
                (ITypeProducerNode a, ITypeProducerNode b) =>
                (
                    a.GetTypeName().Equals(b.GetTypeName())
                ),
                _ => true
            };
        // &&
        // (
        //     this is not LinkTypeNode a ||
        //     node is not LinkTypeNode b ||
        //     a.Entry.Symbol.ToDisplayString().Equals(b.Entry.Symbol.ToDisplayString())
        // )
        // &&
        // (
        //     this is not ITypeImplementerNode c ||
        //     node is not ITypeImplementerNode d ||
        //     (
        //         c.WillGenerateImplementation == d.WillGenerateImplementation &&
        //         c.GetTypeName().Equals(d.GetTypeName()) &&
        //         c.ImplementationClassName.Equals(d.ImplementationClassName)
        //     )
        // )
        // &&
        // (
        //     this is not ITypeProducerNode e ||
        //     node is not ITypeProducerNode f ||
        //     e.GetTypeName().Equals(f.GetTypeName())
        // );
    }

    public IEnumerable<IEnumerable<LinkNode>> ParentsProduct
        => GetProduct(Parents);

    internal static IEnumerable<IEnumerable<T>> GetProduct<T>(IEnumerable<T> source, bool removeLast = false)
    {
        var arr = source.ToArray();

        if (arr.Length == 0) return [];

        return Enumerable.Range(1, (1 << arr.Length) - (removeLast ? 2 : 1))
            .Select(index => arr
                .Where((_, i) => (index & (1 << i)) != 0)
            );
    }

    public void GetPathGenerics(out List<string> generics, out List<string> constraints)
    {
        generics = [];
        constraints = [];

        foreach (var parent in Parents.Prepend(this))
        {
            switch (parent)
            {
                case LinkTypeNode linkType:
                    if (!linkType.Entry.Symbol.IsGenericType) continue;

                    generics.AddRange(linkType.Entry.Symbol.TypeParameters.Select(x => x.Name));
                    constraints.AddRange(linkType.Entry.Syntax.ConstraintClauses.Select(x => x.ToString()));
                    break;
                case BackLinkNode backLink:
                    generics.Add("TSource");
                    constraints.Add("where TSource : class, IPathable");
                    break;
            }
        }
    }

    internal LinkNode AddChild(LinkNode node)
    {
        node.Parent = this;
        Children.Add(node);
        return this;
    }

    public string FormatTypePath(Func<LinkNode, bool>? predicate = null)
    {
        var parts = new List<string>();

        foreach (var parent in Parents)
        {
            if (predicate is not null && !predicate(parent)) continue;

            if (parent is ITypeProducerNode typeProducer)
                parts.Add(typeProducer.GetTypeName());
        }

        parts.Reverse();

        return parts.Count == 0 ? string.Empty : string.Join(".", parts);
    }

    public string FormatAsTypePath(Func<LinkNode, bool>? predicate = null)
    {
        if (this is not ITypeProducerNode typeProducer)
            return string.Empty;

        var path = FormatTypePath(predicate);

        if (path == string.Empty) return string.Empty;

        return $"{path}.{typeProducer.GetTypeName()}";
    }

    public string FormatRelativeTypePath(Func<LinkNode, bool>? predicate = null)
    {
        var result = FormatTypePath(x => x is not ActorNode && (predicate?.Invoke(x) ?? true));

        if (result == string.Empty) return string.Empty;

        return $".{result}";
    }

    public LinkNode? GetNodeWithEquivalentPathing(ActorNode target)
    {
        return Parents
            .Prepend(this)
            .Reverse()
            .Skip(1)
            .Aggregate(
                (LinkNode?) target,
                (node, part) => node?.Children.FirstOrDefault(x => x.SemanticEquals(part))
            );

        // LinkNode? node = target;
        // foreach (var parent in Parents.Prepend(this).Reverse())
        // {
        //     if (parent is ActorNode) continue;
        //
        //     node = node.Children.FirstOrDefault(x =>
        //         x.GetType() == parent.GetType()
        //         &&
        //         (
        //             x is not LinkTypeNode a ||
        //             parent is not LinkTypeNode b ||
        //             a.Entry.Symbol.Equals(b.Entry.Symbol, SymbolEqualityComparer.Default)
        //         )
        //         &&
        //         (
        //             x is not ITypeProducerNode c ||
        //             parent is not ITypeProducerNode d ||
        //             c.GetTypeName().Equals(d.GetTypeName())
        //         )
        //     );
        //
        //     if (node is null) return null;
        // }
        //
        // return node;
    }

    public bool HasTypePath(LinkNode node)
    {
        var path = FormatTypePath();

        if (path == string.Empty) return false;

        return AsPaths(node).Any(x => x.Equals(path));

        static IEnumerable<string> AsPaths(LinkNode node)
        {
            yield return node.FormatTypePath();

            foreach (var child in node.Children.SelectMany(AsPaths))
            {
                yield return child;
            }
        }
    }

    public IEnumerable<LinkNode> GetRelativeNodes()
    {
        foreach (var parent in Parents)
        {
            foreach (var relative in parent.Children.Where(x => x.GetType() == GetType() && x != this))
            {
                yield return relative;
            }
        }
    }

    public LinkNode[] GetAncestorNodes()
    {
        var nodes = new List<LinkNode>();

        var target = Parent;

        while (target is not null)
        {
            nodes.Add(target);
            target = target.Parent;
        }

        nodes.Reverse();
        return nodes.ToArray();
    }

    public void AddSibling(Func<LinkNode> factory)
    {
        Parent?.AddChild(factory());
    }

    public LinkNode(LinkTarget target)
    {
        Target = target;
        Children = [];
    }

    private protected virtual void Visit(NodeContext context, Logger logger)
    {
        ImplicitSemanticCompisition.Clear();
        ImplicitSemanticCompisition.UnionWith(GetSemanticTypeNodeCompisition(logger, excludeParentCompisition: false));

        SemanticCompisition.UnionWith(ImplicitSemanticCompisition);
        if (Parent is not null)
            SemanticCompisition.ExceptWith(Parent.ImplicitSemanticCompisition);
    }

    public abstract string Build(NodeContext context, Logger logger);

    private protected string BuildChildren(NodeContext context, Logger logger)
    {
        if (Children.Count == 0) return string.Empty;

        return string.Join($"{Environment.NewLine}{Environment.NewLine}",
            Children.Select(x => x.Build(context, logger)).Where(x => x != string.Empty));
    }

    public void VisitTree(NodeContext context, Logger logger)
    {
        Visit(context, logger);

        foreach (var child in Children)
        {
            child.VisitTree(context, logger);
        }
    }

    public virtual bool Equals(LinkNode other)
    {
        return ReferenceEquals(this, other);
        // return Target.Equals(other.Target)
        //        && (Parent is null ? other.Parent is null : (other.Parent is not null && Parent.Equals(other.Parent)))
        //        && Children.SequenceEqual(other.Children);
    }

    public ActorNode[] GetAncestors(NodeContext context)
    {
        return context.Graph.Nodes
            .Where(x => Hierarchy.Implements(Target.Actor, x.Key))
            .Select(x => x.Value)
            .ToArray();
    }

    public IEnumerable<LinkNode> GetParents()
    {
        var current = Parent;

        while (current is not null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    public ActorNode[] GetEntityAssignableAncestors(NodeContext context)
    {
        return GetAncestors(context)
            .Where(x =>
                x.Target.Entity.Equals(Target.Entity, SymbolEqualityComparer.Default) ||
                Hierarchy.Implements(Target.Entity, x.Target.Entity)
            )
            .ToArray();
    }

    public override string ToString()
    {
        return this switch
        {
            LinkTypeNode link => $"{link.GetTypeName()} Link ({GetType()})",
            ITypeProducerNode typeProducer => $"{typeProducer.GetTypeName()} ({GetType()})",
            _ => GetType().ToString()
        };
    }

    protected string ToParameterName(string name)
    {
        if (name == string.Empty) return name;

        if (name.StartsWith("_"))
            name = name.Substring(1);

        int lowerCount = 0;
        for (; lowerCount != name.Length; lowerCount++)
        {
            if (char.IsLower(name[lowerCount]))
                break;
        }

        if (lowerCount > 1)
            lowerCount--;

        var a = name.Substring(0, lowerCount);
        var b = name.Substring(lowerCount);

        return $"{a.ToLower()}{b}";
    }
}