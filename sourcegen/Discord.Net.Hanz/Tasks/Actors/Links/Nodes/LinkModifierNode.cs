// using System.Collections.Concurrent;
// using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
// using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;
//
// namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;
//
// public abstract class LinkModifierNode(LinkTarget target) : LinkNode(target)
// {
//     public List<LinkModifierNode> CartesianLinkTypeNodes { get; } = [];
//     public HashSet<LinkNode> ExplicitlyImplements { get; } = [];
//
//     private protected override void Visit(NodeContext context, Logger logger)
//     {
//         CartesianLinkTypeNodes.Clear();
//         ExplicitlyImplements.Clear();
//         
//         CartesianLinkTypeNodes.AddRange(
//             GetCartesianProduct(
//                 ParentLinkTypesProduct.Select(x => x.Reverse()).Prepend([]),
//                 logger
//             )
//         );
//
//         base.Visit(context, logger);
//         
//         ExplicitlyImplements.UnionWith(
//             SemanticComposition
//         );
//
//         ExplicitlyImplements.ExceptWith(
//             SemanticComposition.OfType<LinkModifierNode>().SelectMany(x => x.ExplicitlyImplements)
//         );
//     }
//
//     protected IEnumerable<LinkModifierNode> GetCartesianProduct(IEnumerable<IEnumerable<LinkNode>> nodes, Logger logger)
//     {
//         foreach
//         (
//             var product
//             in nodes
//         )
//         {
//             var productNodes = product as LinkNode[] ?? product.ToArray();
//
//             LinkNode? relativeSearchNode = RootActorNode;
//
//             foreach (var entry in productNodes)
//             {
//                 var next = relativeSearchNode
//                     ?.Children
//                     .FirstOrDefault(x => x.SemanticEquals(entry));
//
//                 if (next is null)
//                 {
//                     logger.Warn(
//                         $"{FormatAsTypePath()}: relative search break on {relativeSearchNode?.GetType()} -> {entry.FormatAsTypePath()} ({entry.GetType()})"
//                     );
//                     break;
//                 }
//
//                 relativeSearchNode = next;
//             }
//
//             if (
//                 relativeSearchNode?
//                     .Children
//                     .OfType<LinkModifierNode>()
//                     .FirstOrDefault(x => x.SemanticEquals(this))
//                 is not { } relative)
//             {
//                 logger.Warn(
//                     $"{FormatAsTypePath()}: relative search fail: {relativeSearchNode?.GetType()} -> {GetType()}"
//                 );
//                 continue;
//             }
//
//             if (relative == this) continue;
//
//             yield return relative;
//         }
//     }
// }