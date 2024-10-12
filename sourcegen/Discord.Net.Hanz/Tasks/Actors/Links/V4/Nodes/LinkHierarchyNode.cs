using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
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

    public List<ActorNode> HierarchyNodes { get; } = [];
    public HierarchyProperties Properties { get; } = [];

    public string ImplementationClassName
        => $"__{Target.Assembly}Hierarchy{
            string.Join(
                string.Empty,
                Parents
                    .Prepend(this)
                    .OfType<LinkTypeNode>()
                    .Select(x => $"{x.Entry.Symbol.Name}{(x.Entry.Symbol.TypeArguments.Length > 0 ? x.Entry.Symbol.TypeArguments.Length : string.Empty)}")
                    .Reverse()
            )
        }";

    public List<(string Type, string Name, string? Default)> RequiredMembers { get; } = [];
    public List<(string Type, string Name, string? Default)> ConstructorMembers { get; } = [];

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
        RequiredMembers.Clear();
        ConstructorMembers.Clear();

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

        if (!IsTemplate && !IsCore)
        {
            RequiredMembers.AddRange(
                HierarchyNodes.Select((string Type, string Name, string? Default) (x) =>
                    ($"{x.Target.Actor}{FormatRelativeTypePath()}", LinksV4.GetFriendlyName(x.Target.Actor), null)
                )
            );
            
            if(Parent is ITypeImplementerNode parentImplementer)
                ConstructorMembers.AddRange(parentImplementer.ConstructorMembers);
            
            ConstructorMembers.AddRange(RequiredMembers);
        }
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

        var members = new List<string>()
        {
            FormatMembers(),
            BuildChildren(context, logger)
        };

        if (!IsCore)
        {
            bases.Add($"{Target.GetCoreActor()}{FormatRelativeTypePath()}.Hierarchy");

            CreateImplementation(members, bases);
        }

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

    private void CreateImplementation(
        List<string> members,
        List<string> bases)
    {
        if (IsTemplate) return;

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

        var ctorParameters = ConstructorMembers
            .GroupBy(x => x.Name)
            .Select(x => x.First())
            .Select(x => $"{x.Type} {ToParameterName(x.Name)}")
            .ToList();

        List<(string Type, string Name, string? Default)> baseCtorParameters = [];

        if (Parent is ITypeImplementerNode implementer)
        {
            ctorParameters.InsertRange(
                0,
                implementer.RequiredMembers.Select(x => $"{x.Type} {ToParameterName(x.Name)}")
            );

            baseCtorParameters = implementer.ConstructorMembers;
            Parent.GetPathGenerics(out var parentGenerics, out _);

            hierarchyBases.Insert(0, $"{Target.Actor}.{implementer.ImplementationClassName}{(
                parentGenerics.Count > 0
                    ? $"<{string.Join(", ", parentGenerics)}>"
                    : string.Empty
            )}");
        }

        var baseArgs = baseCtorParameters
            .GroupBy(x => x.Name)
            .Select(x => x.First())
            .Select(x => ToParameterName(x.Name))
            .ToArray();

        var modifier = Children.OfType<BackLinkNode>().Any() ? "virtual " : string.Empty;
        
        hierarchyMembers.AddRange(
            RequiredMembers.Select(x =>
                $$"""
                  internal {{modifier}}{{x.Type}} {{x.Name}} { get; }
                  {{x.Type}} {{FormatAsTypePath()}}.{{x.Name}} => {{x.Name}};
                  """
            )
        );

        hierarchyMembers.Add(
            $$"""
              public {{ImplementationClassName}}(
                  {{
                      string.Join(
                          $",{Environment.NewLine}",
                          ctorParameters.Distinct()
                      ).WithNewlinePadding(4)
                  }}
              ){{(
                  baseCtorParameters.Count > 0
                      ? $" : base({string.Join(", ", baseArgs)})"
                      : string.Empty
              )}}
              {
                  {{
                      string.Join(
                          Environment.NewLine,
                          RequiredMembers.Select(x => $"{x.Name} = {ToParameterName(x.Name)};")
                      ).WithNewlinePadding(4)
                  }}
              }
              """
        );

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

    public string FormatMembers(bool backlink = false)
    {
        return string
            .Join(
                Environment.NewLine,
                GetFormattedProperties(backlink)
            );
    }

    public IEnumerable<string> GetFormattedProperties(bool backlink = false)
        => HierarchyNodes.SelectMany(x =>
        {
            var result = new List<string>();

            var name = LinksV4.GetFriendlyName(x.Target.Actor);

            var rootType = backlink ? x.FormattedBackLinkType : x.FormattedCoreLink;

            if (IsTemplate)
            {
                if (backlink)
                {
                    result.AddRange([
                        $"new {x.FormattedBackLinkType} {name} {{ get; }}",
                        $"{x.FormattedCoreLink} {Target.Actor}.Hierarchy.{name} => {name};"
                    ]);
                }
                else
                {
                    result.Add($"{rootType} {name} {{ get; }}");
                }
            }
            else
            {
                var type = $"{x.Target.Actor}{FormatRelativeTypePath()}";
                var overrideTarget = $"{Target.Actor}.Hierarchy";

                if (backlink)
                {
                    type = $"{type}.BackLink<TSource>";
                    overrideTarget = $"{overrideTarget}.BackLink<TSource>";
                }

                result.AddRange([
                    $"new {type} {name} {{ get; }}",
                    $"{rootType} {overrideTarget}.{name} => {name};"
                ]);
            }

            if (!IsCore)
            {
                var coreOverloadType = IsTemplate
                    ? backlink ? x.FormattedCoreBackLinkType : x.FormattedCoreLink
                    : $"{x.Target.GetCoreActor()}{FormatRelativeTypePath()}{(backlink ? ".BackLink<TSource>" : string.Empty)}";

                result.Add(
                    $"{coreOverloadType} {Target.GetCoreActor()}{FormatRelativeTypePath()}.Hierarchy{(backlink ? ".BackLink<TSource>" : string.Empty)}.{name} => {name};"
                );
            }

            return result;
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