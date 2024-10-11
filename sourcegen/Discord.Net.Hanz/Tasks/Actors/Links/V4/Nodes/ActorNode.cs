using System.Text;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
using Discord.Net.Hanz.Tasks.Actors.V3;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

public class ActorNode :
    LinkNode,
    ITypeProducerNode
{
    public ActorNode(LinkTarget target, LinkSchematics.Schematic schematic) : base(target)
    {
        Schematic = schematic;

        AddChild(new BackLinkNode(target));
        
        LinkExtensionNode.AddTo(target, this);
        LinkHierarchyNode.AddTo(target, this);
    }

    public LinkSchematics.Schematic Schematic { get; }

    private protected override void Visit(NodeContext context, Logger logger)
    {
        foreach (var entry in Schematic.Root.Children)
        {
            if (LinkTypeNode.TryGetNode(Target, entry, out var node))
            {
                AddChild(node);
            }
        }
    }

    public override string Build(NodeContext context, Logger logger)
    {
        var kind = Target.Actor.TypeKind.ToString().ToLower();

        return
            $$"""
              {{CreateView()}}
              public partial {{kind}} {{Target.Actor.Name}}
              {
                  {{
                      BuildChildren(context, logger).WithNewlinePadding(4)
                  }}
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