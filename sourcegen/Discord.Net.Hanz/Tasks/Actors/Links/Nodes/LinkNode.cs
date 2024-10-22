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
    public LinkTarget Target { get; private set; }
    public HashSet<ActorNode> Ancestors { get; } = [];

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

    public bool ShouldDeclareNewType => Ancestors.Any(x => GetNodeWithEquivalentPathing(x) is ITypeProducerNode);

    public virtual bool SupportsAncestralPathing => true;

    public bool PathSupportsAncestralPathing =>
        SupportsAncestralPathing && Parents.All(v => v.SupportsAncestralPathing);

    public ActorNode? RootActorNode => Parents.OfType<ActorNode>().FirstOrDefault();

    protected bool IsCore => Target.Assembly is LinkActorTargets.AssemblyTarget.Core;

    public string FormattedBackLinkSourceContainer { get; } = "Discord.IBackLinkSource<TSource>";

    public string FormattedIdentifiable
        => $"Discord.IIdentifiable<{Target.Id}, {Target.Entity}, {Target.Actor}, {Target.Model}>";

    public string FormattedCoreIdentifiable
        => $"Discord.IIdentifiable<{Target.Id}, {Target.GetCoreEntity()}, {Target.GetCoreActor()}, {Target.Model}>";

    public string FormattedBackLinkType
        => $"Discord.IBackLink<TSource, {Target.Actor}, {Target.Id}, {Target.Entity}, {Target.Model}>";

    public string FormattedCoreBackLinkType
        =>
            $"Discord.IBackLink<TSource, {Target.GetCoreActor()}, {Target.Id}, {Target.GetCoreEntity()}, {Target.Model}>";

    public string FormattedLinkType
        => $"Discord.ILinkType<{Target.Actor}, {Target.Id}, {Target.Entity}, {Target.Model}>";

    public string FormattedCoreLinkType
        => $"Discord.ILinkType<{Target.GetCoreActor()}, {Target.Id}, {Target.GetCoreEntity()}, {Target.Model}>";

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

    public HashSet<LinkNode> SemanticComposition { get; } = [];
    public HashSet<LinkNode> ImplicitSemanticComposition { get; } = [];

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
        => GetSemanticTypeNodeComposition(logger, excludeSelf)
            .Where(x => x.SemanticEquals(this) && (!excludeSelf || x != this))
            .OfType<LinkModifierNode>();

    protected HashSet<LinkNode> GetSemanticTypeNodeComposition(
        Logger logger,
        bool excludeSelf = true,
        bool excludeParentComposition = true)
    {
        if (RootActorNode is null) return [];

        // we want to get the semantic composition of this node, we define what the result to be as:
        // - all ancestor nodes.
        // - descending node(s) of each ancestor that are semantically equal to an ancestor to this node or are a 
        //   cartesian product of a subset of ancestors of this node.
        // Rules:
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
        //
        //                                         Input space:                                        
        //
        //               │                Link Types                │          Link Modifiers          │
        //               ├─────────┬────────────┬───────────┬───────┼───────────┬───────────┬──────────┤
        // - Actor:      │ Defined │ Enumerable │ Indexable │ Paged │ Hierarchy │ Extension │ BackLink │
        // - Defined:    │    ∅    │ Enumerable │ Indexable │   ∅   │ Hierarchy │ Extension │ BackLink │
        // - Enumerable: │    ∅    │     ∅      │ Indexable │   ∅   │ Hierarchy │ Extension │ BackLink │
        // - Indexable:  │    ∅    │     ∅      │     ∅     │   ∅   │ Hierarchy │ Extension │ BackLink │ 
        // - Paged:      │    ∅    │     ∅      │ Indexable │   ∅   │ Hierarchy │ Extension │ BackLink │
        // - Hierarchy:  │    ∅    │     ∅      │     ∅     │   ∅   │     ∅     │ Extension │ BackLink │
        // - Extension:  │    ∅    │     ∅      │     ∅     │   ∅   │     ∅     │ Extension │ BackLink │ 
        // - BackLink:   │    ∅    │     ∅      │     ∅     │   ∅   │     ∅     │     ∅     │     ∅    │
        // 
        // Given the starting node (this), and the parent nodes (.Parents), we can traverse the tree upwards, and group
        // each sequence of nodes by link types and by each unique modifier type, creating a product set.
        //
        // We also need to correct the order the product set, since we're working from the bottom up, we want to
        // reverse the final product set and subsets within that set.
        //
        // For example, the given tree:
        //
        //            Actor
        //              |
        //          Enumerable 
        //              |
        //          Indexable 
        //              |
        //          Extension
        //
        // will become the following set:
        //
        // { Extension, { Indexable, Enumerable } }
        //
        // And then reversed:
        //
        // { { Enumerable, Indexable }, Extension }
        //
        // After reversing the set, we can create a cartesian variation that expands this product set to all valid 
        // combinations of the set and its subsets that comply to the input space, this is the semantic composition
        // of the power set.
        // This acts similar to the cartesian product, except order here is determined by the input space, rather than
        // order of apperance.
        //
        // For the above example, this turns into:
        //
        // { Extension, Enumerable.Extension, Indexable.Extension, Enumerable, Indexable }
        //
        // Finally, we remove the current instance from the result:
        //
        // { Enumerable.Extension, Indexable.Extension, Enumerable, Indexable }

        // The product set that we'll use to group the ancestor nodes.
        var product = new List<(Type SetType, List<List<LinkNode>> Nodes)>();

        // The current search type, initialized to the current type of 'this'.
        var searchType = GetSearchType(this);

        // The current subset that our search accumulates into.
        var searchSet = new List<LinkNode>() {this};

        // Iterate over all ancestor nodes that produce a type ancestor nodes
        foreach (var parent in Parents.Where(x => x is ITypeProducerNode))
        {
            // If the node is a backlink or the root actor node, we've finished the search.
            if (parent is ActorNode or BackLinkNode) break;

            var parentType = GetSearchType(parent);

            // If the current search type differs from the parent, we can add the current
            // search set to the product and update the search type to the 'parent' node.
            if (searchType != parentType)
            {
                CalculateSearchProduct();
                searchType = parentType;
                searchSet.Clear();
            }

            // Add the parent node to the search set.
            searchSet.Add(parent);
        }

        // If we have any remaining nodes in the search set, add them to the product.
        if (searchSet.Count > 0)
            CalculateSearchProduct();

        logger.Log($"Products with semantics: {product.Count}");

        // Reverse the product set to allow for top-to-bottom traversal.
        product.Reverse();

        LogProduct();

        // Create the result set, this will contain the semantic composition nodes.
        var result = new HashSet<LinkNode>();

        // The implementation for the composition uses bitsets to calculate the powerset of the product set.
        // put simply, we create a bitmask of what product sets to chose, if we have N product sets,
        // we create a N bit number and iterate over it, AND-ing the number and the index of the product set
        // to determine whether to include it in the search.
        //
        // lets say we have 3 product sets, we define the following:
        //
        // bounds = 0b111 : 3 bits, set to all ones 
        // sample = 0b001 : start at 1
        // result = {}
        //
        // the loop simply iterates like the following:
        // 
        // while sample is less than bounds:
        //   searchSpace = { Actor } : the search always starts at the top of the tree
        //   
        //   for each set in product
        //     if (the index of set in product BITWISE_AND sample) is 0
        //     then continue
        //         
        //     setSearchResult = {}
        //
        //     for each node in set:
        //        for each search in searchSpace:
        //           if node in search
        //           then add node to setSearchResult
        //        end
        //     end
        //     
        //     if setSearchResult is empty
        //     then break
        //     
        //     searchSpace = setSearchResult
        //   end
        //   
        //   if searchSpace is not empty
        //   then add searchSpace to result
        // end

        // Our N size bitset, with 'product.Count' bits
        var bounds = (1 << product.Count) - 1;
        var boundsDigitSize = Convert.ToString(bounds, 2).Length;

        // Iterate over every possible number in the N size bitset
        for (var sample = 1; sample <= bounds; sample++)
        {
            var sampleName = Convert.ToString(sample, 2).PadLeft(boundsDigitSize, '0');

            // We'll store the result of this sample here, we initialize it to 
            // contain the root of the tree, which is the actor node, since
            // all nodes we're searching for must descend from this node.
            var searchNodes = new HashSet<LinkNode>() {RootActorNode};

            // Iterate over each set in the product set, from farthest to closest
            // to the current node instance.
            for (var index = 0; index != product.Count; index++)
            {
                // If there are no nodes to search against, we can exit the loop.
                if (searchNodes.Count == 0) break;

                // 'identity' is the index into the bitset for the set we're iterating,
                // we're going to use this to perform our bitwise AND.
                var identity = (1 << index);

                var identityName = Convert.ToString(identity, 2).PadLeft(boundsDigitSize, '0');

                // If the sample mask doesn't have the current sets' index set, we can
                // continue the iteration, excluding it from the search.
                if ((identity & sample) == 0)
                {
                    logger.Log($"Sample {sampleName} ({sample}/{bounds}): {identityName} -> skipped");
                    continue;
                }

                // This is the group in the product set, containing each possible
                // configuration of the group based on the input space.
                var nodes = product[index].Nodes;

                // We'll store the results of 'searchNodes' -> 'nodes' in this 'bag'.
                // We also use a concurrent bag since we'll be running this in parallel.
                var bag = new ConcurrentBag<LinkNode>();

                var sampleLocal = sample;

                // Iterate over each search node we currently have.
                Parallel.ForEach(searchNodes, searchNode =>
                {
                    // Iterate over each combination of nodes from our group.
                    for (var setIndex = 0; setIndex < nodes.Count; setIndex++)
                    {
                        // Store a copy of 'searchNode', since we'll be mutating it.
                        var setSearchNode = searchNode;

                        // This is the combination of nodes in our group we'll be testing against.
                        var set = nodes[setIndex];

                        // We'll iterate over each node in that set, in order.
                        for (var i = 0; i < set.Count; i++)
                        {
                            var node = set[i];

                            // If the node is the root of the tree, we can exit early.
                            if (node == RootActorNode) break;

                            // If our copy of the search node doesn't contain a child thats semantically
                            // equal to the current node in the combination, it isn't valid, so we can
                            // exit the search.
                            if (
                                setSearchNode?.Children
                                    .FirstOrDefault(x => x
                                        .SemanticEquals(node)
                                    ) is not { } next
                            )
                            {
                                logger.Log(
                                    $"Sample {sampleName} ({sampleLocal}/{bounds}): {identityName} -> set " +
                                    $"{setIndex + 1}/{nodes.Count} terminates at element {i}"
                                );
                                break;
                            }

                            // We update the search node to be the semantic child we've just found.
                            setSearchNode = next;

                            // If we're at the end of combination, we can determine if we want to
                            // include the node we found in the result.
                            if (i == set.Count - 1)
                            {
                                // If the callee wants to exclude nodes that the current instances'
                                // parent node also contains in its composition, we can exit the search.
                                if (
                                    excludeParentComposition &&
                                    (Parent?.SemanticComposition.Contains(setSearchNode) ?? false))
                                {
                                    logger.Log(
                                        $"Sample {sampleName} ({sampleLocal}/{bounds}): {identityName} -> set " +
                                        $"{setIndex + 1}/{nodes.Count} completes but is excluded as its contained in the parent."
                                    );
                                    break;
                                }

                                // If the callee wants to not include the current instance, and the
                                // node we found is the current instance, we can exit the search.
                                if (excludeSelf && setSearchNode == this)
                                {
                                    logger.Log(
                                        $"Sample {sampleName} ({sampleLocal}/{bounds}): {identityName} -> set " +
                                        $"{setIndex + 1}/{nodes.Count} completes but is excluded as its the current node."
                                    );
                                    break;
                                }

                                // Add the node we found to the bag.
                                bag.Add(setSearchNode);

                                logger.Log(
                                    $"Sample {sampleName} ({sampleLocal}/{bounds}): {identityName} -> set " +
                                    $"{setIndex + 1}/{nodes.Count} completes"
                                );
                            }
                        }
                    }
                });

                // If this set found no valid compositions, we can end the 
                // search for this sample.
                if (bag.Count == 0)
                {
                    logger.Log($"Sample {sampleName} ({sample}/{bounds}): ending at {index + 1}/{product.Count}");
                    searchNodes.Clear();
                    break;
                }

                logger.Log(
                    $"Sample {sampleName} ({sample}/{bounds}): {identityName} -> {bag.Count} nodes from {searchNodes.Count}");

                // Update 'searchNodes' to only contain what this set found during its search. 
                searchNodes.Clear();
                searchNodes.UnionWith(bag);
            }

            // Add the nodes we found for the sample into the final result set.
            result.UnionWith(searchNodes);
            continue;
        }

        return result;

        void LogProduct()
        {
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
            }
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

        Type GetSearchType(LinkNode node) => node is LinkTypeNode ? typeof(LinkTypeNode) : node.GetType();
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
        Ancestors.Clear();
        Ancestors.UnionWith(GetEntityAssignableAncestors(context));

        SemanticComposition.Clear();
        ImplicitSemanticComposition.Clear();
        ImplicitSemanticComposition.UnionWith(GetSemanticTypeNodeComposition(logger, excludeParentComposition: false));

        SemanticComposition.UnionWith(ImplicitSemanticComposition);
        if (Parent is not null)
            SemanticComposition.ExceptWith(Parent.ImplicitSemanticComposition);
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

    private ActorNode[] GetEntityAssignableAncestors(NodeContext context)
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

        if (lowerCount == name.Length)
            return name.ToLower();

        if (lowerCount > 1)
            lowerCount--;

        var a = name.Substring(0, lowerCount);
        var b = name.Substring(lowerCount);

        return $"{a.ToLower()}{b}";
    }
}