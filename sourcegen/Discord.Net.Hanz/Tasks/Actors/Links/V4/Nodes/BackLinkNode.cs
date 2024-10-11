using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
using Discord.Net.Hanz.Tasks.Actors.V3;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

public class BackLinkNode(LinkTarget target) :
    LinkNode(target),
    ITypeProducerNode
{
    public bool IsTemplate
        => Parent is ActorNode;

    public bool HasImplementation { get; private set; }
    public bool RedefinesSource { get; private set; }

    public List<BackLinkNode> InheritedBackLinks { get; } = [];

    private protected override void Visit(NodeContext context, Logger logger)
    {
        InheritedBackLinks.Clear();

        HasImplementation = Target.Assembly is not LinkActorTargets.AssemblyTarget.Core;
        RedefinesSource = HasImplementation || GetEntityAssignableAncestors(context).Length > 0;
        
        foreach (var relative in ContainingNodes.Select(x => x.Children.OfType<BackLinkNode>().FirstOrDefault()))
        {
            if(relative is null || relative == this) continue;
            
            logger.Log($"{FormatTypePath()} BackLink += {relative.FormatTypePath()}");
            
            InheritedBackLinks.Add(relative);
            RedefinesSource |= relative.RedefinesSource;
        }
    }

    public override string Build(NodeContext context, Logger logger)
    {
        var members = new HashSet<string>();

        var bases = new HashSet<string>()
        {
            FormatTypePath(),
            FormattedBackLinkType
        };

        var ancestors = GetEntityAssignableAncestors(context);

        if (!IsCore)
        {
            bases.UnionWith([
                FormattedCoreBackLinkType,
                $"{Target.GetCoreActor()}{FormatRelativeTypePath()}.BackLink<TSource>"
            ]);
        }
        
        if (RedefinesSource)
        {
            members.UnionWith([
                "new TSource Source { get; }",
                $"TSource {FormattedCoreBackLinkType}.Source => Source;"
            ]);

            if (!IsCore)
                members.Add($"TSource {FormattedBackLinkType}.Source => Source;");
        }
        
        if (ancestors.Length > 0)
        {
            foreach (var ancestor in ancestors)
            {
                if (GetNodeWithEquivalentPathing(ancestor) is not BackLinkNode ancestorBackLink) continue;

                var ancestorPath = ancestorBackLink.FormatTypePath();

                bases.Add($"{ancestorPath}.BackLink<TSource>");

                var overrideType = ancestorBackLink.RedefinesSource
                    ? $"{ancestorPath}.BackLink<TSource>"
                    : ancestor.FormattedBackLinkType;
                
                members.Add($"TSource {overrideType}.Source => Source;");
            }
        }

        if (!IsCore)
            members.Add(CreateImplementation());

        switch (Parent)
        {
            case LinkHierarchyNode hierarchy:
                members.UnionWith(hierarchy.GetFormattedProperties(true));

                if (!hierarchy.IsTemplate)
                {
                    var hierarchyBackLink = $"{Target.Actor}.Hierarchy.BackLink<TSource>";

                    if (hierarchy.Children.OfType<BackLinkNode>().FirstOrDefault() is {RedefinesSource: true})
                        members.Add($"TSource {hierarchyBackLink}.Source => Source;");
                }

                break;
            case LinkExtensionNode extension:
                members.UnionWith(
                    extension.Properties
                        .SelectMany(x => extension.FormatProperty(x, true))
                );

                if (!extension.IsTemplate)
                {
                    var templateExtensionBackLink = $"{Target.Actor}.{extension.GetTypeName()}.BackLink<TSource>";
                    
                    if (extension.Children.OfType<BackLinkNode>().FirstOrDefault() is {RedefinesSource: true})
                        members.Add($"TSource {templateExtensionBackLink}.Source => Source;");
                }
                break;
        }

        foreach (var inheritedBackLink in InheritedBackLinks)
        {
            bases.Add($"{inheritedBackLink.FormatTypePath()}.BackLink<TSource>");

            if (!inheritedBackLink.RedefinesSource) continue;

            members.Add($"TSource {inheritedBackLink.FormatTypePath()}.BackLink<TSource>.Source => Source;");
        }
        
        return
            $$"""
              public interface {{GetTypeName()}} :
                  {{string.Join($",{Environment.NewLine}", bases.OrderBy(x => x)).WithNewlinePadding(4)}}
                  where TSource : class, IPathable
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
              }
              """;
    }

    private string CreateImplementation()
    {
        return string.Empty;
    }

    public string GetTypeName()
        => "BackLink<out TSource>";

    public override string ToString()
    {
        return
            $"""
              {base.ToString()}
              Is Template?: {IsTemplate}
              Has Implementation?: {HasImplementation}
              Redefines Source?: {RedefinesSource}
              Inherited BackLinks: {InheritedBackLinks.Count}
              """;
        
//        {(
//                  InheritedBackLinks.Count > 0
//                      ? $"{Environment.NewLine}{string.Join(Environment.NewLine, InheritedBackLinks.Select(x => $"- {x}".Replace(Environment.NewLine, $"{Environment.NewLine}  > ")))}"
//                      : string.Empty
//              )}
    }
}