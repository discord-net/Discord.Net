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
    LinkModifierNode,
    ITypeImplementerNode
{
    public bool IsTemplate => Parent is ActorNode;

    public bool WillGenerateImplementation
        => RootActorNode is not null && Target.Assembly is not LinkActorTargets.AssemblyTarget.Core;

    public List<ActorNode> HierarchyNodes { get; } = [];
    public HierarchyProperties HierarchyProperties { get; } = [];
    public Constructor? Constructor { get; private set; }
    public List<Property> Properties { get; } = [];

    //public HashSet<LinkHierarchyNode> ExplicitlyImplements { get; } = [];

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
        LinkExtensionNode.AddTo(Target, this);
    }

    private protected override void Visit(NodeContext context, Logger logger)
    {
        ExplicitlyImplements.Clear();
        HierarchyNodes.Clear();
        HierarchyProperties.Clear();
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

        base.Visit(context, logger);

        // ExplicitlyImplements.UnionWith(
        //     SemanticCompisition.OfType<LinkHierarchyNode>()
        // );
        //
        // ExplicitlyImplements.ExceptWith(
        //     SemanticCompisition.OfType<LinkHierarchyNode>().SelectMany(x => x.ExplicitlyImplements)
        // );
    }

    public override string Build(NodeContext context, Logger logger)
    {
        if (HierarchyNodes.Count == 0) return string.Empty;

        var bases = new List<string>();

        // if (!IsTemplate)
        //     bases.AddRange([
        //         FormatTypePath(),
        //         //$"{Target.Actor}.Hierarchy"
        //     ]);

        var members = new List<string>(
            HierarchyNodes.Select(FormatHierarchyNodeAsProperty)
        );

        if (!IsTemplate)
        {
            foreach (var baseNode in ExplicitlyImplements)
            {
                bases.Add($"{baseNode.FormatAsTypePath()}");
            
                if(baseNode is not LinkHierarchyNode relative)
                    continue;
            
                if (!IsCore)
                    bases.Add($"{Target.GetCoreActor()}{baseNode.FormatRelativeTypePath()}.Hierarchy");

                foreach (var node in relative.HierarchyNodes)
                {
                    var name = LinksV4.GetFriendlyName(node.Target.Actor);
                    var type = relative.IsTemplate
                        ? node.FormattedLink
                        : $"{node.Target.Actor}{relative.FormatRelativeTypePath()}";

                    members.Add($"{type} {relative.FormatAsTypePath()}.{name} => {name};");

                    if (!IsCore)
                    {
                        var coreType = relative.IsTemplate
                            ? node.FormattedCoreLink
                            : $"{node.Target.GetCoreActor()}{relative.FormatRelativeTypePath()}.Hierarchy";

                        members.Add(
                            $"{coreType} {relative.Target.GetCoreActor()}{relative.FormatRelativeTypePath()}.Hierarchy.{name} => {name};");
                    }
                }
            }
        }
        
        if (!IsCore)
        {
            // bases.Add($"{Target.GetCoreActor()}{FormatRelativeTypePath()}.Hierarchy");
            //
            // members.AddRange(
            //     HierarchyNodes.Select(x =>
            //     {
            //         var type = IsTemplate
            //             ? x.FormattedCoreLink
            //             : $"{x.Target.GetCoreActor()}{FormatRelativeTypePath()}";
            //
            //         var overrideTarget = $"{Target.GetCoreActor()}{FormatRelativeTypePath()}.Hierarchy";
            //
            //         var name = LinksV4.GetFriendlyName(x.Target.Actor);
            //
            //         return $"{type} {overrideTarget}.{name} => {name};";
            //     })
            // );

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

        var ctorParams = Constructor?.GetActualParameters() ?? [];

        members.Add(
            $$"""
              internal static new {{FormatAsTypePath()}} Create({{(
                  ctorParams.Count > 0
                      ? $"{Environment.NewLine}{string.Join(
                          $",{Environment.NewLine}",
                          ctorParams.Select(x =>
                              x.Format()
                          )
                      )}".WithNewlinePadding(4) + Environment.NewLine
                      : string.Empty
              )}}) => new {{Target.Actor}}.{{ImplementationClassName}}<{{string.Join(", ", generics)}}>({{(
              ctorParams.Count > 0
                  ? string.Join(", ", ctorParams.Select(x => x.Name))
                  : string.Empty
          )}});
              """
        );
    }

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