using System.Diagnostics.CodeAnalysis;
using Discord.Net.Hanz.Tasks.Actors.V3;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;
using LinkSpecificMember = (string Type, string Name, string? Default);
using LinkSpecificMembers = System.Collections.Generic.List<(string Type, string Name, string? Default)>;


namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;

public abstract class LinkTypeNode :
    LinkNode,
    ITypeImplementerNode
{
    public LinkSchematics.Entry Entry { get; }

    public bool RedefinesLinkMembers { get; protected set; }

    public string ImplementationClassName
        => $"__{Target.Assembly}Link{
            string.Join(
                string.Empty,
                Parents
                    .Prepend(this)
                    .OfType<LinkTypeNode>()
                    .Select(x => $"{x.Entry.Symbol.Name}{(x.Entry.Symbol.TypeArguments.Length > 0 ? x.Entry.Symbol.TypeArguments.Length : string.Empty)}")
                    .Reverse()
            )
        }";

    public IEnumerable<LinkTypeNode> ParentLinks
        => Parents.OfType<LinkTypeNode>();

    public LinkSpecificMembers InstanceImplementationMembers { get; } = [];
    public Dictionary<LinkTypeNode, HashSet<LinkSpecificMember>> AllImplmenetationMembers { get; } = [];

    protected string? ImplementationLinkType => Target.Assembly switch
    {
        LinkActorTargets.AssemblyTarget.Rest => FormattedRestLinkType,
        _ => null
    };

    public LinkTypeNode? ImplementationBase { get; private set; }
    public LinkTypeNode? ImplementationChild { get; private set; }


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

    protected abstract void CreateImplementation(
        List<string> members,
        List<string> bases,
        NodeContext context,
        Logger logger
    );

    private void CreateImplementationForNode(
        List<string> members,
        HashSet<string> bases,
        NodeContext context,
        Logger logger)
    {
        if (RootActorNode is null) return;

        var classBases = new List<string>()
        {
            FormatAsTypePath()
        };

        var classMembers = new List<string>();

        if (ImplementationBase is not null)
        {
            ImplementationBase.GetPathGenerics(out var baseGenerics, out _);

            classBases.Insert(0,
                $"{ImplementationBase.Target.Actor}.{ImplementationClassName}{(
                    baseGenerics.Count > 0
                        ? $"<{string.Join(", ", baseGenerics)}>"
                        : string.Empty
                )}"
            );
        }

        var memberModifier = ImplementationBase is not null
            ? "override "
            : ImplementationChild is not null
                ? "virtual "
                : string.Empty;

        classMembers.AddRange([
            $"{FormattedActorProvider} {FormattedRestLinkType}.Provider => ActorProvider;"
        ]);

        CreateImplementation(classMembers, classBases, context, logger);

        foreach (var parentLinkNode in Parents.OfType<LinkTypeNode>())
        {
            parentLinkNode.CreateImplementation(classMembers, classBases, context, logger);
        }

        var allLinkMembers = AllImplmenetationMembers.Values
            .SelectMany(x => x)
            .Union(InstanceImplementationMembers)
            .GroupBy(x => x.Name)
            .Select(x => x.First())
            .ToArray();

        classMembers.AddRange([
            $"internal {memberModifier}{Target.Actor} GetActor({Target.Id} id) => ActorProvider.GetActor(id);",
            $"{Target.Actor} {FormattedActorProvider}.GetActor({Target.Id} id) => GetActor(id);",
            $"{Target.GetCoreActor()} {FormattedCoreActorProvider}.GetActor({Target.Id} id) => GetActor(id);",
        ]);

        var entityFactoryImpl = Target.Model
            .AllInterfaces
            .Any(x => x.Name == "IEntityModel")
            ? "GetActor(model.Id).CreateEntity(model);"
            : "EntityProvider.CreateEntity(model);";

        classMembers.AddRange([
            $"internal {memberModifier}{Target.Entity} CreateEntity({Target.Model} model) => {entityFactoryImpl}",
            $"{Target.Entity} {FormattedEntityProvider}.CreateEntity({Target.Model} model) => CreateEntity(model);",
            $"{Target.GetCoreEntity()} {FormattedCoreEntityProvider}.CreateEntity({Target.Model} model) => CreateEntity(model);",
        ]);

        foreach (var instanceImplementationMember in InstanceImplementationMembers)
        {
            var isField = instanceImplementationMember.Name.StartsWith("_");

            var modifier = instanceImplementationMember.Name is "Client"
                ? "public"
                : isField
                    ? "private readonly"
                    : "internal";

            var tail = isField ? ";" : " { get; }";

            classMembers.Add(
                $"{modifier} {memberModifier}{instanceImplementationMember.Type} {instanceImplementationMember.Name}{tail}");
        }

        classMembers.Add(
            $$"""
              internal {{ImplementationClassName}}(
                  {{(
                      allLinkMembers.Length > 0
                          ? string.Join(
                              $",{Environment.NewLine}",
                              allLinkMembers.Select(x =>
                                  $"{x.Type} {ToParameterName(x.Name)}{(x.Default is not null ? $" = {x.Default}" : string.Empty)}"
                              )
                          ).WithNewlinePadding(4)
                          : string.Empty
                  )}}
              ){{(
                  ImplementationBase is not null ? $" : base({(
                      allLinkMembers.Length > 0
                          ? string.Join(
                              ", ",
                              allLinkMembers.Select(x =>
                                  ToParameterName(x.Name)
                              )
                          )
                          : string.Empty
                  )})" : string.Empty
              )}}
              {
                  {{(
                      InstanceImplementationMembers.Count > 0
                          ? string.Join(
                              Environment.NewLine,
                              InstanceImplementationMembers
                                  .GroupBy(x => x.Name)
                                  .Select(x => x.First())
                                  .Select(x =>
                                      $"{x.Name} = {ToParameterName(x.Name)};"
                                  )
                          ).WithNewlinePadding(4)
                          : string.Empty
                  )}}     
              }
              """
        );

        GetPathGenerics(out var generics, out var constraints);

        var formattedGenerics = generics.Count > 0 ? $"<{string.Join(", ", generics)}>" : string.Empty;
        var formattedConstraints = string.Join(Environment.NewLine, constraints);

        RootActorNode.AdditionalTypes.Add(
            $$"""
              private protected class {{ImplementationClassName}}{{formattedGenerics}} : 
                  {{string.Join($",{Environment.NewLine}", classBases.Distinct()).WithNewlinePadding(4)}}{{(
                      constraints.Count > 0
                          ? $"{Environment.NewLine}{formattedConstraints}".WithNewlinePadding(4)
                          : string.Empty
                  )}}
              {
                  {{string.Join(Environment.NewLine, classMembers.Distinct()).WithNewlinePadding(4)}}
              }
              """
        );

        members.Add(
            $$"""
              internal static {{FormatAsTypePath()}} Create(
                  {{(
                      allLinkMembers.Length > 0
                          ? string.Join(
                              $",{Environment.NewLine}",
                              allLinkMembers.Select(x =>
                                  $"{x.Type} {ToParameterName(x.Name)}{(x.Default is not null ? $" = {x.Default}" : string.Empty)}"
                              )
                          ).WithNewlinePadding(4)
                          : string.Empty
                  )}}
              ) => new {{Target.Actor}}.{{ImplementationClassName}}{{formattedGenerics}}({{(
                  allLinkMembers.Length > 0
                      ? string.Join(
                          $", ",
                          allLinkMembers.Select(x =>
                              ToParameterName(x.Name)
                          )
                      )
                      : string.Empty
              )}});
              """
        );
    }

    internal virtual LinkSpecificMembers ImplementationMembers { get; } = [];

    private protected override void Visit(NodeContext context, Logger logger)
    {
        if (Target.Assembly is not LinkActorTargets.AssemblyTarget.Core)
        {
            InstanceImplementationMembers.Clear();
            AllImplmenetationMembers.Clear();

            var hasBase = context.TryGetBaseTarget(Target, out var baseTarget);
            var hasChild = context.TryGetChildTarget(Target, out var childTarget);

            if (hasBase && GetNodeWithEquivalentPathing(baseTarget) is LinkTypeNode baseNode)
                ImplementationBase = baseNode;
            else ImplementationBase = null;

            if (hasChild && GetNodeWithEquivalentPathing(childTarget) is LinkTypeNode childNode)
                ImplementationChild = childNode;
            else ImplementationChild = null;

            if (!hasBase)
            {
                InstanceImplementationMembers.Add(($"Discord{Target.Assembly}Client", "Client", null));
            }

            InstanceImplementationMembers.Add((FormattedActorProvider, "ActorProvider", null));

            InstanceImplementationMembers.AddRange(ImplementationMembers);

            foreach (var parentLink in Parents.OfType<LinkTypeNode>())
            {
                InstanceImplementationMembers.AddRange(parentLink.ImplementationMembers);
            }

            if (hasBase)
            {
                var current = baseTarget;

                do
                {
                    if (GetNodeWithEquivalentPathing(current) is LinkTypeNode node)
                    {
                        if (!AllImplmenetationMembers.TryGetValue(node, out var linkMembers))
                            AllImplmenetationMembers[node] = linkMembers = new();

                        linkMembers.UnionWith(node.ImplementationMembers);
                    }
                } while (context.TryGetBaseTarget(current.Target, out current));
            }

            if (!Target.Model.AllInterfaces.Any(x => x.Name == "IEntityModel"))
            {
                InstanceImplementationMembers.Add((FormattedEntityProvider, "EntityProvider", null));
            }
        }
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

        AddMembers(members, context, logger);

        if (IsCore)
        {
            bases.Add($"{FormattedLinkType}{path}.{GetTypeName()}");
        }
        else
        {
            if (ImplementationLinkType is null) return string.Empty;

            bases.UnionWith([
                $"{Target.GetCoreActor()}{path}.{GetTypeName()}",
                $"{FormattedLinkType}{path}.{GetTypeName()}",
                $"{ImplementationLinkType}"
            ]);

            CreateImplementationForNode(members, bases, context, logger);

            foreach (var parentlinkType in Parents.OfType<LinkTypeNode>())
            {
                parentlinkType.AddMembers(members, context, logger);
            }
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

    LinkSpecificMembers ITypeImplementerNode.RequiredMembers
        => InstanceImplementationMembers;

    LinkSpecificMembers ITypeImplementerNode.ConstructorMembers
        => Parent is ITypeImplementerNode parentImplementer
            ? [..parentImplementer.ConstructorMembers, ..InstanceImplementationMembers]
            : InstanceImplementationMembers;
}