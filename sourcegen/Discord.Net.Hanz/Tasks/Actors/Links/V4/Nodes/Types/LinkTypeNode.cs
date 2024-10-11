using System.Diagnostics.CodeAnalysis;
using Discord.Net.Hanz.Tasks.Actors.V3;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;

public abstract class LinkTypeNode : LinkNode, ITypeProducerNode
{
    public LinkSchematics.Entry Entry { get; }

    public IEnumerable<LinkTypeNode> ParentLinks
        => Parents.OfType<LinkTypeNode>();

    protected string? ImplementationLinkType => Target.Assembly switch
    {
        LinkActorTargets.AssemblyTarget.Rest => FormattedRestLinkType,
        _ => null
    };

    protected LinkTypeNode(
        LinkTarget target,
        LinkSchematics.Entry entry
    ) : base(target)
    {
        Entry = entry;

        // always has a backlink
        AddChild(new BackLinkNode(Target));

        // if theres extensions
        LinkExtensionNode.AddTo(Target, this);
        LinkHierarchyNode.AddTo(Target, this);
    }

    protected abstract void AddMembers(List<string> members, NodeContext context, Logger logger);
    protected abstract string CreateImplementation(NodeContext context, Logger logger);

    private protected override void Visit(NodeContext context, Logger logger)
    {
    }

    public override string Build(NodeContext context, Logger logger)
    {
        var bases = new HashSet<string>();
        var members = new List<string>();

        var path = FormatRelativeTypePath(x => x is LinkTypeNode);
        var ancestors = GetEntityAssignableAncestors(context);

        foreach (var parentLinks in LinkTypesProduct)
        {
            bases.Add($"{Target.Actor}.{string.Join(".", parentLinks.Select(x => x.GetTypeName()))}");
            logger.Log($"{path}.{GetTypeName()}: {string.Join(".", parentLinks.Select(x => x.GetTypeName()))}");
        }

        if (Parent is LinkTypeNode)
        {
            bases.Add($"{Target.Actor}.{GetTypeName()}");
        }

        if (IsCore)
        {
            bases.Add($"{FormattedLinkType}{path}.{GetTypeName()}");

            AddMembers(members, context, logger);
        }
        else
        {
            if (ImplementationLinkType is null) return string.Empty;

            // bases.AddRange([
            //     $"{Target.GetCoreActor()}{path}",
            //     ImplementationLinkType
            // ]);

            members.Add(CreateImplementation(context, logger));
        }

        if (ancestors.Length > 0)
        {
            // redefine get actor
            members.AddRange([
                $"new {Target.Actor} GetActor({Target.Id} id);",
                $"{Target.Actor} Discord.IActorProvider<{Target.Actor}, {Target.Id}>.GetActor({Target.Id} id) => GetActor(id);"
            ]);
            
            foreach (var ancestor in ancestors)
            {
                var ancestorBase = $"{ancestor.Target.Actor}{path}.{GetTypeName()}";
                
                bases.Add(ancestorBase);

                var overrideType = ancestor.GetEntityAssignableAncestors(context).Length > 0
                    ? ancestorBase
                    : $"Discord.IActorProvider<{ancestor.Target.Actor}, {ancestor.Target.Id}>";
                
                members.AddRange([
                    $"{ancestor.Target.Actor} {overrideType}.GetActor({ancestor.Target.Id} id) => GetActor(id);"
                ]);
            }
        }
        
        return
            $$"""
              public interface {{GetTypeName()}} : 
                  {{string.Join($",{Environment.NewLine}", bases.OrderBy(x => x)).WithNewlinePadding(4)}}{{(
                      Entry.Syntax.ConstraintClauses.Count > 0
                          ? $"{Environment.NewLine}{string.Join(Environment.NewLine, Entry.Syntax.ConstraintClauses)}"
                              .PrefixNewLine()
                              .WithNewlinePadding(4)
                          : string.Empty
                  )}}
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
                  {{BuildChildren(context, logger).WithNewlinePadding(4)}}
              }
              """;
    }

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
    public static bool TryGetNode(LinkTarget target, LinkSchematics.Entry entry, out LinkTypeNode node)
    {
        node = entry.Symbol.Name switch
        {
            "Indexable" => new IndexableNode(target, entry),
            "Paged" => new PagedNode(target, entry),
            "Enumerable" => new EnumerableNode(target, entry),
            "Defined" => new DefinedNode(target, entry),
            _ => null!
        };

        if (node is null) return false;

        foreach (var child in entry.Children)
        {
            if (TryGetNode(target, child, out var childNode))
                node.AddChild(childNode);
        }

        return true;
    }

    public string GetTypeName()
        => LinksV4.FormatTypeName(Entry.Symbol);
}