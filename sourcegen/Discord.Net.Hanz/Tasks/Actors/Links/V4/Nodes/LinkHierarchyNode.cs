using System.Text;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Microsoft.CodeAnalysis;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

using HierarchyProperties = List<(string Type, LinkTarget Target, ActorNode Node)>;
using HierarchyProperty = (string Type, LinkTarget Target, ActorNode Node);

public class LinkHierarchyNode :
    LinkNode,
    ITypeImplementerNode
{
    public bool IsTemplate => Parent is ActorNode;

    public bool WillGenerateImplementation
        => RootActorNode is not null && Target.Assembly is not LinkActorTargets.AssemblyTarget.Core;

    public List<ActorNode> HierarchyNodes { get; } = [];
    public HierarchyProperties HierarchyProperties { get; } = [];

    public Constructor? Constructor { get; private set; }

    public List<Property> Properties { get; } = [];

    public List<LinkHierarchyNode> CartesianHierarchyNodes { get; } = [];

    public string ImplementationClassName
        => $"__{Target.Assembly}Hierarchy{
            string.Join(
                string.Empty,
                Parents
                    .Select(x =>
                        x switch {
                            LinkTypeNode linkType => $"{linkType.Entry.Symbol.Name}{(linkType.Entry.Symbol.TypeParameters.Length > 0 ? linkType.Entry.Symbol.TypeParameters.Length : string.Empty)}",
                            LinkExtensionNode extension => extension.GetTypeName(),
                            _ => null
                        }
                    )
                    .OfType<string>()
                    .Reverse()
            )
        }";

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
        HierarchyProperties.Clear();
        Properties.Clear();
        CartesianHierarchyNodes.Clear();

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

        foreach
        (
            var product
            in ParentLinkTypesProduct.Select(x => x.Reverse()).Prepend([])
        )
        {
            var productNodes = product as LinkTypeNode[] ?? product.ToArray();

            LinkNode? relativeSearchNode = RootActorNode;

            foreach (var entry in productNodes)
            {
                var next = relativeSearchNode
                    ?.Children
                    .FirstOrDefault(x => x.SemanticEquals(entry));

                if (next is null)
                {
                    logger.Warn(
                        $"{FormatAsTypePath()}: relative search break on {relativeSearchNode?.GetType()} -> {entry.FormatAsTypePath()}");
                    break;
                }

                relativeSearchNode = next;
            }

            if (relativeSearchNode?.Children.OfType<LinkHierarchyNode>().FirstOrDefault() is not { } relative)
            {
                logger.Warn(
                    $"Couldn't find relative nodes for link hierarchy '{string.Join(" | ", productNodes.Select(x => x.GetTypeName()))}'");
                continue;
            }

            if (relative == this) continue;

            CartesianHierarchyNodes.Add(relative);
        }

        HierarchyNodes.AddRange(children);

        HierarchyProperties.AddRange(
            HierarchyNodes.Select(x =>
                (
                    Type: IsTemplate
                        ? x.FormattedLink
                        : FormatTypePath(),
                    x.Target,
                    Node: x
                )
            )
        );

        if (!IsCore)
        {
            Properties.AddRange(
                HierarchyNodes.Select(x =>
                    new Property(
                        LinksV4.GetFriendlyName(x.Target.Actor),
                        IsTemplate ? x.FormattedLink : $"{x.Target.Actor}{FormatRelativeTypePath()}",
                        isVirtual: IsTemplate || Children.OfType<BackLinkNode>().Any()
                    )
                )
            );

            Constructor = new(
                ImplementationClassName,
                Properties
                    .Select(x =>
                        new ConstructorParamter(
                            ToParameterName(x.Name),
                            x.Type,
                            initializes: x
                        )
                    )
                    .ToList(),
                (Parent as ITypeImplementerNode)?.Constructor
            );
        }
    }

    public override string Build(NodeContext context, Logger logger)
    {
        if (HierarchyNodes.Count == 0) return string.Empty;

        var bases = new List<string>();

        if (!IsTemplate)
            bases.AddRange([
                FormatTypePath(),
                //$"{Target.Actor}.Hierarchy"
            ]);

        var members = new List<string>(
            HierarchyNodes.Select(FormatHierarchyNodeAsProperty)
        );

        foreach (var relative in CartesianHierarchyNodes)
        {
            bases.Add($"{relative.FormatAsTypePath()}");

            var coreOverrideTarget = $"{Target.GetCoreActor()}{relative.FormatRelativeTypePath()}.Hierarchy";

            if (!IsCore)
                bases.Add(coreOverrideTarget);

            foreach (var hierarchyNode in HierarchyNodes)
            {
                var type = relative.IsTemplate
                    ? hierarchyNode.FormattedLink
                    : $"{hierarchyNode.Target.Actor}{relative.FormatRelativeTypePath()}";

                var name = LinksV4.GetFriendlyName(hierarchyNode.Target.Actor);

                members.Add($"{type} {relative.FormatAsTypePath()}.{name} => {name};");

                if (!IsCore)
                {
                    var coreType = relative.IsTemplate
                        ? hierarchyNode.FormattedCoreLink
                        : $"{hierarchyNode.Target.GetCoreActor()}{relative.FormatRelativeTypePath()}";

                    members.Add($"{coreType} {coreOverrideTarget}.{name} => {name};");
                }
            }
        }

        if (!IsCore)
        {
            bases.Add($"{Target.GetCoreActor()}{FormatRelativeTypePath()}.Hierarchy");

            members.AddRange(
                HierarchyNodes.Select(x =>
                {
                    var type = IsTemplate
                        ? x.FormattedCoreLink
                        : $"{x.Target.GetCoreActor()}{FormatRelativeTypePath()}";

                    var overrideTarget = $"{Target.GetCoreActor()}{FormatRelativeTypePath()}.Hierarchy";

                    var name = LinksV4.GetFriendlyName(x.Target.Actor);

                    return $"{type} {overrideTarget}.{name} => {name};";
                })
            );

            CreateImplementation(members, bases);
        }

        members.Add(BuildChildren(context, logger));

        return
            $$"""
              public interface Hierarchy{{(
                  bases.Count > 0
                      ? $" :{Environment.NewLine}{string.Join($",{Environment.NewLine}", bases)}".WithNewlinePadding(4)
                      : string.Empty
              )}}
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
              }
              """;
    }

    public string FormatHierarchyNodeAsProperty(ActorNode node)
    {
        var type = IsTemplate ? node.FormattedLink : $"{node.Target.Actor}{FormatRelativeTypePath()}";
        var name = LinksV4.GetFriendlyName(node.Target.Actor);

        var result = new StringBuilder();

        if (!IsCore || !IsTemplate)
            result.Append("new ");

        return result
            .Append(type)
            .Append(' ')
            .Append(name)
            .Append(" { get; }")
            .ToString();
    }

    private void CreateImplementation(
        List<string> members,
        List<string> bases)
    {
        switch (Target.Assembly)
        {
            case LinkActorTargets.AssemblyTarget.Rest:
                CreateRestImplementation(members, bases);
                break;
        }
    }

    private void CreateRestImplementation(
        List<string> members,
        List<string> bases)
    {
        if (RootActorNode is null) return;

        var hierarchyBases = new List<string>() {FormatAsTypePath()};
        var hierarchyMembers = new List<string>();

        GetPathGenerics(out var generics, out var constraints);

        if (Parent is ITypeImplementerNode {WillGenerateImplementation: true} implementer)
        {
            Parent.GetPathGenerics(out var parentGenerics, out _);

            hierarchyBases.Insert(0, $"{Target.Actor}.{implementer.ImplementationClassName}{(
                parentGenerics.Count > 0
                    ? $"<{string.Join(", ", parentGenerics)}>"
                    : string.Empty
            )}");
        }

        hierarchyMembers.AddRange(
            Properties.SelectMany(IEnumerable<string> (x) =>
            [
                x.Format(),
                $"{x.Type} {FormatAsTypePath()}.{x.Name} => {x.Name};"
            ])
        );

        if (Constructor is not null)
            hierarchyMembers.Add(Constructor.Format());

        var typeName =
            $"{ImplementationClassName}{(generics.Count > 0 ? $"<{string.Join(", ", generics)}>" : string.Empty)}";

        RootActorNode.AdditionalTypes.Add(
            $$"""
              private protected class {{typeName}} : 
                  {{string.Join($",{Environment.NewLine}", hierarchyBases.Distinct()).WithNewlinePadding(4)}}
                  {{string.Join(Environment.NewLine, constraints).WithNewlinePadding(4)}}
              {
                  {{string.Join(Environment.NewLine, hierarchyMembers).WithNewlinePadding(4)}}
              }
              """
        );
    }

    // public string FormatMembers(bool backlink = false)
    // {
    //     return string
    //         .Join(
    //             Environment.NewLine,
    //             GetFormattedProperties(backlink)
    //         );
    // }
    //
    // public IEnumerable<string> GetFormattedProperties(bool backlink = false)
    //     => HierarchyNodes.SelectMany(x =>
    //     {
    //         var result = new List<string>();
    //
    //         var name = LinksV4.GetFriendlyName(x.Target.Actor);
    //
    //         var rootType = backlink ? x.FormattedBackLinkType : x.FormattedLink;
    //
    //         if (IsTemplate)
    //         {
    //             if (backlink)
    //             {
    //                 result.AddRange([
    //                     $"new {x.FormattedBackLinkType} {name} {{ get; }}",
    //                     $"{x.FormattedLink} {Target.Actor}.Hierarchy.{name} => {name};"
    //                 ]);
    //             }
    //             else
    //             {
    //                 result.Add($"{(!IsCore ? "new ": string.Empty)}{rootType} {name} {{ get; }}");
    //             }
    //         }
    //         else
    //         {
    //             var type = $"{x.Target.Actor}{FormatRelativeTypePath()}";
    //             var overrideTarget = $"{Target.Actor}.Hierarchy";
    //
    //             if (backlink)
    //             {
    //                 type = $"{type}.BackLink<TSource>";
    //                 overrideTarget = $"{overrideTarget}.BackLink<TSource>";
    //             }
    //
    //             result.AddRange([
    //                 $"new {type} {name} {{ get; }}",
    //                 $"{rootType} {overrideTarget}.{name} => {name};"
    //             ]);
    //         }
    //
    //         if (!IsCore)
    //         {
    //             var coreOverloadType = IsTemplate
    //                 ? backlink ? x.FormattedCoreBackLinkType : x.FormattedCoreLink
    //                 : $"{x.Target.GetCoreActor()}{FormatRelativeTypePath()}{(backlink ? ".BackLink<TSource>" : string.Empty)}";
    //
    //             result.Add(
    //                 $"{coreOverloadType} {Target.GetCoreActor()}{FormatRelativeTypePath()}.Hierarchy{(backlink ? ".BackLink<TSource>" : string.Empty)}.{name} => {name};"
    //             );
    //
    //             if (!IsTemplate)
    //             {
    //                 result.Add(
    //                     $"{(backlink ? x.FormattedCoreBackLinkType : x.FormattedCoreLink)} {Target.GetCoreActor()}.Hierarchy{(backlink ? ".BackLink<TSource>" : string.Empty)}.{name} => {name};"
    //                 );
    //             }
    //             
    //             if (backlink)
    //             {
    //                 result.Add(
    //                     $"{x.FormattedCoreLink} {Target.GetCoreActor()}.Hierarchy.{name} => {name};"
    //                 );
    //             }
    //         }
    //
    //         return result;
    //     });

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
             Types: {HierarchyProperties.Count}{(
                 HierarchyProperties.Count > 0
                     ? $"{Environment.NewLine}{string.Join(Environment.NewLine, HierarchyProperties.Select(x => $"- {x.Type} ({x.Target.Actor})"))}"
                     : string.Empty
             )}
             Is Template: {IsTemplate}
             """;
    }
}