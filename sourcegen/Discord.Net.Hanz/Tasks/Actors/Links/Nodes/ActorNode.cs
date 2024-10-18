using System.Text;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

public class ActorNode :
    LinkNode,
    ITypeImplementerNode
{
    public bool WillGenerateImplementation => !IsCore;

    public string ImplementationClassName => $"__LinkBase";

    public Constructor? Constructor { get; private set; }

    public List<Property> Properties { get; } = [];

    public LinkSchematics.Schematic Schematic { get; }

    public List<string> AdditionalTypes { get; } = [];

    public bool ModelHasId => Target.Model.AllInterfaces.Any(x => x.Name == "IEntityModel");

    public ActorNode? BaseActorNode { get; private set; }

    public bool RedefinesRootInterfaceMembers { get; private set; }

    public string? UserSpecifiedRelationshipName { get; private set; }

    public string RelationshipName => UserSpecifiedRelationshipName ?? LinksV4.GetFriendlyName(Target.Actor);

    public Dictionary<string, List<ActorNode>> AdditionalCanonicalRelationships { get; } = [];

    public bool CanonicalRelationshipIsRedefined { get; private set; }

    public ActorNode(LinkTarget target, LinkSchematics.Schematic schematic) : base(target)
    {
        Schematic = schematic;

        AddChild(new BackLinkNode(target));

        LinkExtensionNode.AddTo(target, this);
        LinkHierarchyNode.AddTo(target, this);

        foreach (var entry in Schematic.Root.Children)
        {
            if (LinkTypeNode.TryGetNode(Target, entry, out var node))
            {
                AddChild(node);
            }
        }
    }

    private protected override void Visit(NodeContext context, Logger logger)
    {
        base.Visit(context, logger);

        UserSpecifiedRelationshipName =
            Target.Actor.GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == "RelationshipNameAttribute")
                ?.ConstructorArguments[0].Value as string
            ?? Ancestors
                .Select(x => x.UserSpecifiedRelationshipName)
                .FirstOrDefault(x => x is not null);

        if (!IsCore)
        {
            Properties.Clear();

            if (context.TryGetBaseTarget(Target, out var baseActorNode))
                BaseActorNode = baseActorNode;

            var isOverride = BaseActorNode is not null;
            var isVirtual = context.TryGetChildTarget(Target, out _);

            Properties.Add(
                new Property(
                    "Client",
                    $"Discord{Target.Assembly}Client",
                    accessibility: Accessibility.Public
                )
            );

            Properties.Add(
                new(
                    "ActorProvider",
                    FormattedActorProvider,
                    isOverride: isOverride,
                    isVirtual: isVirtual
                )
            );

            if (!ModelHasId)
            {
                Properties.Add(
                    new(
                        "EntityProvider",
                        FormattedEntityProvider,
                        isOverride: isOverride,
                        isVirtual: isVirtual
                    )
                );
            }

            Constructor = new(
                ImplementationClassName,
                Properties
                    .Select(x =>
                        new ConstructorParamter(
                            ToParameterName(x.Name),
                            x.Type,
                            null,
                            x
                        )
                    )
                    .ToList(),
                BaseActorNode?.Constructor
            );
        }

        RedefinesRootInterfaceMembers = !IsCore || Ancestors.Count > 0;

        AdditionalCanonicalRelationships.Clear();
        GetAdditionalRelationshipTypes(Target.GetCoreActor(), AdditionalCanonicalRelationships, context);

        CanonicalRelationshipIsRedefined =
            AdditionalCanonicalRelationships.ContainsKey(RelationshipName) ||
            Ancestors.Any(x => x.RelationshipName == RelationshipName);

        logger.Log($"{AdditionalCanonicalRelationships.Count} additional canonical relationships:");

        foreach (var entry in AdditionalCanonicalRelationships)
        {
            logger.Log($" - {entry.Key}: {entry.Value.Count}:");

            foreach (var item in entry.Value)
            {
                logger.Log($"    - {item.Target.Actor}");
            }
        }
    }

    private void GetAdditionalRelationshipTypes(
        INamedTypeSymbol symbol,
        Dictionary<string, List<ActorNode>> types,
        NodeContext context)
    {
        foreach (var iface in symbol.Interfaces.Where(x => x.ToString().EndsWith("CanonicalRelationship")))
        {
            var node = context.Graph.Nodes.FirstOrDefault(x =>
                x.Value.Target
                    .GetCoreActor()
                    .ToDisplayString()
                    .Equals(iface.ToDisplayString().Replace(".CanonicalRelationship", string.Empty))
            );

            if (node.Value is null) continue;

            if (types.TryGetValue(node.Value.RelationshipName, out var nodes))
            {
                if (
                    nodes.All(x =>
                        !x.Target.Actor.Equals(node.Value.Target.Actor, SymbolEqualityComparer.Default)
                    )
                ) nodes.Add(node.Value);

                continue;
            }

            types[node.Value.RelationshipName] = [node.Value];
            GetAdditionalRelationshipTypes(node.Value.Target.Actor, types, context);
        }
    }

    public override string Build(NodeContext context, Logger logger)
    {
        logger.Log($"Hash: {Target.GetHashCode()}");
        AdditionalTypes.Clear();

        var kind = Target.Actor.TypeKind.ToString().ToLower();

        var members = new List<string>();

        CreateBaseLinkType(members, context);
        CreateActorProviderFactory(members, context, logger);
        CreateRelationshipsTypes(members, context);

        members.Add(BuildChildren(context, logger));
        members.AddRange(AdditionalTypes);

        var result =
            $$"""
              public partial {{kind}} {{Target.Actor.Name}}{{(
                  Target.Actor.TypeParameters.Length > 0
                      ? $"<{string.Join(", ", Target.Actor.TypeParameters)}>"
                      : string.Empty
              )}}
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
              }
              """;

        var containing = Target.Actor.ContainingType;

        while (containing is not null)
        {
            result =
                $$"""
                  public partial {{containing.TypeKind.ToString().ToLower()}} {{containing.Name}}{{(
                      containing.TypeParameters.Length > 0
                          ? $"<{string.Join(", ", containing.TypeParameters)}>"
                          : string.Empty
                  )}}
                  {
                      {{result.WithNewlinePadding(4)}}
                  }
                  """;

            containing = containing.ContainingType;
        }

        return
            $$"""
              {{CreateView()}}
              {{result}}
              """;
    }

    private void CreateRelationshipsTypes(List<string> members, NodeContext context)
    {
        if (!IsCore) return;

        var relationshipMembers = new List<string>()
        {
            $"{Target.Actor} Discord.IRelationship<{Target.Actor}, {Target.Id}, {Target.Entity}>.RelationshipActor => {RelationshipName};"
        };

        var relationshipMemberIsNew = false;

        var canonicalRelationshipMembers = new List<string>();

        var relationshipBases = new List<string>()
        {
            $"Discord.IRelationship<{Target.Actor}, {Target.Id}, {Target.Entity}>"
        };
        var canonicalRelationshipBases = new List<string>()
        {
            $"Relationship",
            $"Discord.ICanonicalRelationship<{Target.Actor}, {Target.Id}, {Target.Entity}>"
        };

        var ancestors = GetAncestors(context);

        foreach (var ancestor in ancestors)
        {
            //relationshipBases.Add($"{ancestor.Target.Actor}.Relationship");
            canonicalRelationshipBases.Add($"{ancestor.Target.Actor}.CanonicalRelationship");

            // relationshipMembers.Add(
            //     $"{ancestor.Target.Actor} {ancestor.Target.Actor}.Relationship.{ancestor.RelationshipName} => {RelationshipName};"
            // );

            canonicalRelationshipMembers.Add(
                $"{ancestor.Target.Actor} {ancestor.Target.Actor}.Relationship.{ancestor.RelationshipName} => {RelationshipName};"
            );
        }

        foreach
        (
            var group
            in ancestors
                .Prepend(this)
                .GroupBy(
                    x => x.Target.Entity,
                    (key, x) => (Entity: key, Nodes: x.ToArray()),
                    SymbolEqualityComparer.Default
                )
                .Where(x => x.Nodes.Length > 1))
        {
            var node = group.Nodes[0];
            canonicalRelationshipMembers.Add(
                $"{node.Target.Id} Discord.IRelation<{node.Target.Id}, {group.Entity}>.RelationshipId => {node.RelationshipName}.Id;"
            );
        }

        if (CanonicalRelationshipIsRedefined)
        {
            canonicalRelationshipMembers.AddRange([
                $"internal new {Target.Actor} {RelationshipName} {{ get; }}",
                $"{Target.Actor} {Target.Actor}.Relationship.{RelationshipName} => {RelationshipName};"
            ]);
        }

        foreach (var relationship in AdditionalCanonicalRelationships)
        foreach (var node in relationship.Value)
        {
            canonicalRelationshipBases.Add($"{node.Target.Actor}.CanonicalRelationship");

            if (CanonicalRelationshipIsRedefined)
            {
                canonicalRelationshipMembers.Add(
                    $"{node.Target.Actor} {node.Target.Actor}.{(node.CanonicalRelationshipIsRedefined ? "Canonical" : string.Empty)}Relationship.{relationship.Key} => {RelationshipName}.{relationship.Key};");
            }
        }

        relationshipMembers.Add(
            $"internal{(relationshipMemberIsNew ? " new" : string.Empty)} {Target.Actor} {RelationshipName} {{ get; }}"
        );

        members.AddRange([
            $$"""
              public {{(Ancestors.Count > 0 ? "new " : string.Empty)}}interface Relationship : 
                  {{string.Join($",{Environment.NewLine}", relationshipBases).WithNewlinePadding(4)}}
              {
                  {{string.Join(Environment.NewLine, relationshipMembers).WithNewlinePadding(4)}}
              }
              """,
            $$"""
              public {{(Ancestors.Count > 0 ? "new " : string.Empty)}}interface CanonicalRelationship : 
                  {{string.Join($",{Environment.NewLine}", canonicalRelationshipBases).WithNewlinePadding(4)}}
              {
                  {{string.Join(Environment.NewLine, canonicalRelationshipMembers.Distinct()).WithNewlinePadding(4)}}
              }
              """
        ]);
    }

    private void CreateActorProviderFactory(List<string> members, NodeContext context, Logger logger)
    {
        if (IsCore) return;

        logger.Log($"Generating actor factories, {Target.Actor.Constructors} constructors");

        foreach
        (
            var constructor
            in Target.Actor.Constructors
        )
        {
            if (
                constructor.Parameters
                    .Count(x =>
                        x.Type.Name == $"Discord{Target.Assembly}Client" ||
                        x.Type.ToDisplayString() == FormattedIdentifiable ||
                        x.Type.ToDisplayString() == $"{LinksV4.GetFriendlyName(Target.Actor)}Identity"
                    )
                < 2
            ) continue;

            var parameters = new Dictionary<string, (string Type, string? Default)?>();
            var relationshipParameters = new HashSet<ActorNode>(
                AdditionalCanonicalRelationships
                    .SelectMany(x => x.Value)
            );

            var cacheKeyParams = new HashSet<string>();

            foreach (var parameter in constructor.Parameters)
            {
                logger.Log($"Parameter: {parameter.Type} {parameter.Name} ({parameter.Type.TypeKind})");

                if (parameter.Type.Name == "IIdentifiable")
                {
                    if (parameter.Type.ToDisplayString() == FormattedIdentifiable)
                    {
                        parameters.Add(parameter.Name, null);
                        continue;
                    }

                    if (
                        relationshipParameters.RemoveWhere(x =>
                            x.FormattedIdentifiable.Equals(parameter.Type.ToDisplayString())
                        ) > 0
                    ) cacheKeyParams.Add(parameter.Name);
                }

                parameters.Add(parameter.Name, (
                    parameter.Type.ToDisplayString(),
                    parameter.HasExplicitDefaultValue
                        ? SyntaxUtils.CreateLiteral(parameter.Type, parameter.ExplicitDefaultValue).ToString()
                        : null
                ));
            }

            foreach (var actor in relationshipParameters)
            {
                cacheKeyParams.Add(
                    ToParameterName(LinksV4.GetFriendlyName(actor.Target.Actor))
                );
                parameters.Add(
                    ToParameterName(LinksV4.GetFriendlyName(actor.Target.Actor)),
                    (actor.FormattedIdentifiable, null)
                );
            }

            var orderedParameters = parameters
                .Where(x => x.Value is not null)
                .Select((x, i) => (Parameter: x, Index: i))
                .OrderBy(x =>
                    x.Parameter.Value?.Default is null
                        ? x.Index
                        : int.MaxValue - x.Index
                )
                .Select(x => x.Parameter)
                .ToArray();

            var factory =
                $"""
                 (client, id) => new {Target.Actor}(
                     {string.Join(
                             $",{Environment.NewLine}",
                             parameters.Select(x =>
                                 x.Value is not null
                                     ? x.Key
                                     : $"{FormattedIdentifiable}.Of(id)"
                             )
                         ).WithNewlinePadding(4)
                     }
                 )
                 """;

            var invocation = AdditionalCanonicalRelationships.Count > 0
                ? $"""
                   CreateStateful<{Target.Actor}, {Target.Id}>(
                       client, 
                       HashCode.Combine({string.Join(", ", cacheKeyParams.Select(x => $"{x}.Id"))}),
                       {factory.WithNewlinePadding(4)}
                   )
                   """
                : $"""
                   CreateRoot<{Target.Actor}, {Target.Id}>(
                       client, 
                       {factory.WithNewlinePadding(4)}
                   )
                   """;

            members.AddRange([
                $"""
                 internal static readonly new Func<{string.Join(", ", orderedParameters.Select(x => x.Value?.Type ?? FormattedIdentifiable))}, {FormattedActorProvider}> DefaultActorProvider = GetProvider;
                 """,
                $"""
                 internal static new {FormattedActorProvider} GetProvider(
                     {
                         string.Join(
                             $",{Environment.NewLine}",
                             orderedParameters.Select(x =>
                                 $"{x.Value?.Type ?? FormattedIdentifiable} {x.Key}{(
                                     x.Value?.Default is not null
                                         ? $" = {x.Value.Value.Default}"
                                         : string.Empty
                                 )}"
                             )
                         ).WithNewlinePadding(4)
                     }
                 ) => RestActorProvider.{invocation};
                 """
            ]);
        }
    }

    private void CreateBaseLinkType(List<string> members, NodeContext context)
    {
        CreateBaseLinkInterface(members, context);

        if (WillGenerateImplementation)
        {
            switch (Target.Assembly)
            {
                case LinkActorTargets.AssemblyTarget.Rest:
                    CreateRestBaseLinkClass(members, context);
                    break;
            }
        }
    }

    private void CreateBaseLinkInterface(List<string> members, NodeContext context)
    {
        var bases = new List<string>()
        {
            FormattedLink
        };

        if (!IsCore)
            bases.Add(FormattedCoreLink);

        var linkMembers = new List<string>();

        switch (Target.Assembly)
        {
            case LinkActorTargets.AssemblyTarget.Rest:
                bases.Add(FormattedRestLinkType);
                bases.Add($"{Target.GetCoreActor()}.Link");
                linkMembers.AddRange([
                    $"internal new {Target.Actor} GetActor({Target.Id} id) => Provider.GetActor(id);",
                    $"internal new {Target.Entity} CreateEntity({Target.Model} model);",
                ]);
                break;
            case LinkActorTargets.AssemblyTarget.Core:
                if (RedefinesRootInterfaceMembers)
                {
                    linkMembers.AddRange([
                        $"internal new {Target.Actor} GetActor({Target.Id} id);",
                        $"internal new {Target.Entity} CreateEntity({Target.Model} model);",
                    ]);
                }

                break;
        }

        if (RedefinesRootInterfaceMembers || Ancestors.Count > 0)
        {
            linkMembers.AddRange([
                $"{Target.Actor} {FormattedActorProvider}.GetActor({Target.Id} id) => GetActor(id);",
                $"{Target.Entity} {FormattedEntityProvider}.CreateEntity({Target.Model} model) => CreateEntity(model);",
            ]);

            if (!IsCore)
                linkMembers.AddRange([
                    $"{Target.GetCoreActor()} {FormattedCoreActorProvider}.GetActor({Target.Id} id) => GetActor(id);",
                    $"{Target.GetCoreEntity()} {FormattedCoreEntityProvider}.CreateEntity({Target.Model} model) => CreateEntity(model);",
                ]);
        }

        foreach (var ancestor in Ancestors)
        {
            bases.Add($"{ancestor.Target.Actor}.Link");

            var ancestorActorProviderTarget = ancestor.RedefinesRootInterfaceMembers
                ? $"{ancestor.Target.Actor}.Link"
                : ancestor.FormattedActorProvider;

            var ancestorEntityProviderTarget = ancestor.RedefinesRootInterfaceMembers
                ? $"{ancestor.Target.Actor}.Link"
                : ancestor.FormattedEntityProvider;

            linkMembers.AddRange([
                $"{ancestor.Target.Actor} {ancestorActorProviderTarget}.GetActor({Target.Id} id) => GetActor(id);",
                $"{ancestor.Target.Entity} {ancestorEntityProviderTarget}.CreateEntity({ancestor.Target.Model} model) => CreateEntity(model);",
            ]);
        }

        members.Add(
            $$"""
              public {{(Ancestors.Count > 0 ? "new " : string.Empty)}}interface Link :
                  {{string.Join($",{Environment.NewLine}", bases).WithNewlinePadding(4)}}
              {
                  {{string.Join(Environment.NewLine, linkMembers.Distinct()).WithNewlinePadding(4)}}
              }  
              """
        );
    }

    private void CreateRestBaseLinkClass(List<string> members, NodeContext context)
    {
        var linkInterface = $"{Target.Actor}.Link";

        var bases = new List<string>()
        {
            linkInterface
        };

        var hasBase = context.TryGetBaseTarget(Target, out var baseActorNode);
        if (hasBase)
        {
            bases.Insert(0, $"{baseActorNode.Target.Actor}.{baseActorNode.ImplementationClassName}");
        }

        var modifier = hasBase ? "override" : "virtual";

        var classMembers = new List<string>();

        classMembers.AddRange(Properties.Select(x => x.Format()));

        classMembers.AddRange([
            $"internal {modifier} {Target.Actor} GetActor({Target.Id} id) => ActorProvider.GetActor(id);",
            $"{Target.Actor} {(RedefinesRootInterfaceMembers ? linkInterface : FormattedActorProvider)}.GetActor({Target.Id} id) => GetActor(id);"
        ]);

        var entityFactoryImpl = ModelHasId
            ? "GetActor(model.Id).CreateEntity(model);"
            : "EntityProvider.CreateEntity(model);";

        classMembers.AddRange([
            $"internal {modifier} {Target.Entity} CreateEntity({Target.Model} model) => {entityFactoryImpl}",
            $"{Target.Entity} {(RedefinesRootInterfaceMembers ? linkInterface : FormattedEntityProvider)}.CreateEntity({Target.Model} model) => CreateEntity(model);"
        ]);

        classMembers.AddRange([
            $"{FormattedActorProvider} {FormattedRestLinkType}.Provider => ActorProvider;"
        ]);

        if (Constructor is not null)
            classMembers.Add(Constructor.Format());

        members.Add(
            $$"""
              private protected {{(Ancestors.Count > 0 ? "new " : string.Empty)}}abstract class {{ImplementationClassName}} :
                  {{string.Join($",{Environment.NewLine}", bases).WithNewlinePadding(4)}}
              {
                  {{string.Join(Environment.NewLine, classMembers).WithNewlinePadding(4)}}
              }
              """
        );
    }

    private string CreateView()
    {
        var sb = new StringBuilder()
            .AppendLine($"// {Target.Actor}:")
            .AppendLine($"// - Model: {Target.Model}")
            .AppendLine($"// - Entity: {Target.Entity}")
            .AppendLine($"// - Id: {Target.Id}")
            .AppendLine($"// Interfaces: ");

        foreach (var iface in Target.Actor.Interfaces)
        {
            sb.AppendLine($"//  - {iface}");
        }


        sb.AppendLine($"// Nodes: ");

        FormatNode(this, 0);

        return sb.ToString();

        void FormatNode(LinkNode node, int depth)
        {
            sb.AppendLine(
                $"//{"".PadLeft(depth * 2)} - {
                    node.ToString()
                        .Replace(Environment.NewLine, $"{Environment.NewLine}> ")
                        .WithNewlinePadding(3 + depth * 2)
                        .Replace(Environment.NewLine, $"{Environment.NewLine}//")
                }"
            );

            if (node.Children.Count == 0) return;

            sb.AppendLine($"//{"".PadLeft(depth * 2)} > Children: ");

            foreach (var child in node.Children)
            {
                FormatNode(child, depth + 1);
            }
        }
    }

    public string GetTypeName()
        => Target.Actor.ToDisplayString();
}