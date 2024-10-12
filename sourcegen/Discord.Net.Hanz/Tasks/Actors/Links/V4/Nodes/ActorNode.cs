using System.Text;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;
using Discord.Net.Hanz.Tasks.Actors.V3;
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
        if (IsCore) return;

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
                .ToList()
        );
    }

    public override string Build(NodeContext context, Logger logger)
    {
        AdditionalTypes.Clear();

        var kind = Target.Actor.TypeKind.ToString().ToLower();

        var members = new List<string>();

        if (WillGenerateImplementation && CreateBaseLinkType(context) is { } baseLinkType)
            members.Add(baseLinkType);

        members.Add(BuildChildren(context, logger));
        members.AddRange(AdditionalTypes);

        return
            $$"""
              {{CreateView()}}
              public partial {{kind}} {{Target.Actor.Name}}
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
              }
              """;
    }

    private string? CreateBaseLinkType(NodeContext context)
    {
        switch (Target.Assembly)
        {
            case LinkActorTargets.AssemblyTarget.Rest:
                return CreateRestBaseLinkType(context);
                break;
        }

        return null;
    }

    private string CreateRestBaseLinkType(NodeContext context)
    {
        var bases = new List<string>()
        {
            FormattedLink,
            FormattedCoreLink,
            FormattedRestLinkType
        };

        var hasBase = context.TryGetBaseTarget(Target, out var baseActorNode);
        if (hasBase)
        {
            bases.Insert(0, $"{baseActorNode.Target.Actor}.{baseActorNode.ImplementationClassName}");
        }

        var modifier = hasBase ? "override" : "virtual";

        var members = new List<string>();

        members.AddRange(Properties.Select(x => x.Format()));

        members.AddRange([
            $"internal {modifier} {Target.Actor} GetActor({Target.Id} id) => ActorProvider.GetActor(id);",
            $"{Target.Actor} {FormattedActorProvider}.GetActor({Target.Id} id) => GetActor(id);",
            $"{Target.GetCoreActor()} {FormattedCoreActorProvider}.GetActor({Target.Id} id) => GetActor(id);",
        ]);

        var entityFactoryImpl = ModelHasId
            ? "GetActor(model.Id).CreateEntity(model);"
            : "EntityProvider.CreateEntity(model);";

        members.AddRange([
            $"internal {modifier} {Target.Entity} CreateEntity({Target.Model} model) => {entityFactoryImpl}",
            $"{Target.Entity} {FormattedEntityProvider}.CreateEntity({Target.Model} model) => CreateEntity(model);",
            $"{Target.GetCoreEntity()} {FormattedCoreEntityProvider}.CreateEntity({Target.Model} model) => CreateEntity(model);",
        ]);
        
        members.AddRange([
            $"{FormattedActorProvider} {FormattedRestLinkType}.Provider => ActorProvider;"
        ]);

        if (Constructor is not null)
            members.Add(Constructor.Format());

        return
            $$"""
              private protected abstract class {{ImplementationClassName}} :
                  {{string.Join($",{Environment.NewLine}", bases).WithNewlinePadding(4)}}
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
              }
              """;
    }

    private string CreateView()
    {
        var sb = new StringBuilder()
            .AppendLine($"// {Target.Actor}:")
            .AppendLine($"// - Model: {Target.Model}")
            .AppendLine($"// - Entity: {Target.Entity}")
            .AppendLine($"// - Id: {Target.Id}")
            .AppendLine($"// Nodes: ");

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