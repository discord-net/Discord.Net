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
                    a.GetTypeName().Equals(b.GetTypeName()) &&
                    a.ImplementationClassName.Equals(b.ImplementationClassName)
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
        LinkNode? node = target;
        foreach (var parent in Parents.Prepend(this).Reverse())
        {
            if (parent is ActorNode) continue;

            node = node.Children.FirstOrDefault(x =>
                x.GetType() == parent.GetType()
                &&
                (
                    x is not LinkTypeNode a ||
                    parent is not LinkTypeNode b ||
                    a.Entry.Symbol.Equals(b.Entry.Symbol, SymbolEqualityComparer.Default)
                )
                &&
                (
                    x is not ITypeProducerNode c ||
                    parent is not ITypeProducerNode d ||
                    c.GetTypeName().Equals(d.GetTypeName())
                )
            );

            if (node is null) return null;
        }

        return node;
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

    private protected abstract void Visit(NodeContext context, Logger logger);

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
        return Target.Equals(other.Target)
               && (Parent is null ? other.Parent is null : other.Parent is not null && Parent.Equals(other.Parent))
               && Children.SequenceEqual(other.Children);
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