using Discord.Net.Hanz.Tasks.Actors.V3;
using Microsoft.CodeAnalysis;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

using HierarchyProperties = List<(string Type, LinkTarget Target, ActorNode Node)>;
using HierarchyProperty = (string Type, LinkTarget Target, ActorNode Node);

public class LinkHierarchyNode :
    LinkNode,
    ITypeProducerNode
{
    public bool IsTemplate => Parent is ActorNode;

    public List<ActorNode> HierarchyNodes { get; } = [];
    public HierarchyProperties Properties { get; } = [];

    private readonly AttributeData _attribute;

    public LinkHierarchyNode(
        LinkTarget target,
        AttributeData attribute
    ) : base(target)
    {
        _attribute = attribute;
        AddChild(new BackLinkNode(target));
    }

    private protected override void Visit(NodeContext context, Logger logger)
    {
        HierarchyNodes.Clear();
        Properties.Clear();

        var types = _attribute.NamedArguments
            .FirstOrDefault(x => x.Key == "Types")
            .Value;

        var children = types.Kind is not TypedConstantKind.Error
            ? (
                types.Kind switch
                {
                    TypedConstantKind.Array => types.Values.Select(x => (INamedTypeSymbol) x.Value!),
                    _ => (INamedTypeSymbol[]) types.Value!
                }
            )
            .Select(x => context.Graph.Nodes.Values
                .FirstOrDefault(y =>
                    y.Target.GetCoreActor().Equals(x, SymbolEqualityComparer.Default)
                )
            )
            .ToArray()
            : context.Graph.Nodes.Values
                .Where(x =>
                    Hierarchy.Implements(x.Target.GetCoreActor(), Target.GetCoreActor()))
                .ToArray();

        logger.Log($"{Target.Actor}: {children.Length} hierarchical link targets");

        HierarchyNodes.AddRange(children);

        Properties.AddRange(
            HierarchyNodes.Select(x =>
                (
                    Type: IsTemplate
                        ? x.FormattedCoreLink
                        : FormatTypePath(),
                    x.Target,
                    Node: x
                )
            )
        );
    }

    public override string Build(NodeContext context, Logger logger)
    {
        if (HierarchyNodes.Count == 0) return string.Empty;

        var bases = new List<string>();

        if (!IsTemplate)
            bases.AddRange([
                FormatTypePath(),
                $"{Target.Actor}.Hierarchy"
            ]);

        return
            $$"""
              public interface Hierarchy{{(
                  bases.Count > 0
                      ? $" :{Environment.NewLine}{string.Join($",{Environment.NewLine}", bases)}".WithNewlinePadding(4)
                      : string.Empty
              )}}
              {
                  {{
                      FormatMembers().WithNewlinePadding(4)
                  }}
                  {{
                      BuildChildren(context, logger).WithNewlinePadding(4)
                  }}
              }
              """;
    }

    public string FormatMembers(bool backlink = false)
    {
        return string
            .Join(
                Environment.NewLine,
                GetFormattedProperties(backlink)
            );
    }

    public IEnumerable<string> GetFormattedProperties(bool backlink = false)
        => HierarchyNodes.Select(x =>
        {
            var name = LinksV3.GetFriendlyName(x.Target.Actor);

            var rootType = backlink ? x.FormattedBackLinkType : x.FormattedCoreLink;

            if (IsTemplate)
            {
                if (backlink)
                {
                    return
                        $$"""
                          new {{x.FormattedBackLinkType}} {{name}} { get; }
                          {{x.FormattedCoreLink}} {{Target.Actor}}.Hierarchy.{{name}} => {{name}};
                          """;
                }
                
                return $"{rootType} {name} {{ get; }}";
            }

            var type = $"{x.Target.Actor}{FormatRelativeTypePath()}";
            var overrideTarget = $"{Target.Actor}.Hierarchy";

            if (backlink)
            {
                type = $"{type}.BackLink<TSource>";
                overrideTarget = $"{overrideTarget}.BackLink<TSource>";
            }

            return
                $$"""
                  new {{type}} {{name}} { get; }
                  {{rootType}} {{overrideTarget}}.{{name}} => {{name}};
                  """;
        });

    public string GetTypeName()
        => "Hierarchy";

    public static void AddTo(LinkTarget target, LinkNode node)
    {
        var hierarchyAttribute =
            target.GetCoreActor()
                .GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == "LinkHierarchicalRootAttribute");

        if (hierarchyAttribute is null) return;

        node.AddChild(new LinkHierarchyNode(target, hierarchyAttribute));
    }
    
    public override string ToString()
    {
        return
            $"""
             {base.ToString()}
             Types: {Properties.Count}{(
                 Properties.Count > 0
                     ? $"{Environment.NewLine}{string.Join(Environment.NewLine, Properties.Select(x => $"- {x.Type} ({x.Target.Actor})"))}"
                     : string.Empty
             )}
             Is Template: {IsTemplate}
             """;
    }
}