using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

public abstract class LinkModifierNode(LinkTarget target) : LinkNode(target)
{
    public List<LinkModifierNode> CartesianLinkTypeNodes { get; } = [];

    private protected override void Visit(NodeContext context, Logger logger)
    {
        CartesianLinkTypeNodes.Clear();
        
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

            if (
                relativeSearchNode?
                    .Children
                    .OfType<LinkModifierNode>()
                    .FirstOrDefault(x => x.GetType() == GetType())
                is not { } relative)
            {
                logger.Warn(
                    $"{FormatAsTypePath()}: relative search fail: {relativeSearchNode?.GetType()} -> {GetType()}");
                continue;
            }

            if (relative == this) continue;

            logger.Log($"{FormatAsTypePath()}: += cartesain {relative.FormatAsTypePath()}");
            CartesianLinkTypeNodes.Add(relative);
        }
    }
}