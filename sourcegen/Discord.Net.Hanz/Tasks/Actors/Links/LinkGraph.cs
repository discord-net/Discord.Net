// using System.Collections.Immutable;
// using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;
// using Discord.Net.Hanz.Tasks.Actors.V3;
// using Discord.Net.Hanz.Utils;
// using Microsoft.CodeAnalysis;
//
// namespace Discord.Net.Hanz.Tasks.Actors.Links.V4;
//
// public sealed class LinkGraph : IEquatable<LinkGraph>
// {
//     public Dictionary<INamedTypeSymbol, ActorNode> Nodes { get; }
//
//     public NodeContext Context { get; }
//
//     public Compilation Compilation { get; }
//     public LinkSchematics.Schematic Schematic { get; }
//
//     public LinkGraph(
//         Dictionary<INamedTypeSymbol, ActorNode> nodes, 
//         Compilation compilation,
//         LinkSchematics.Schematic schematic)
//     {
//         Nodes = nodes;
//         Compilation = compilation;
//         Schematic = schematic;
//         Context = new(this);
//     }
//
//     internal void Log(Logger logger)
//     {
//         logger.Log("Graph:");
//
//         foreach (var node in Nodes)
//         {
//             logger.Log($" - {node.Key}:");
//             LogNode(node.Value, 1);
//         }
//
//         void LogNode(LinkNode node, int depth)
//         {
//             logger.Log($"{"".PadLeft(depth * 2)} - {node}");
//             foreach (var child in node.Children)
//             {
//                 LogNode(child, depth + 1);
//             }
//         }
//     }
//
//     public void Visit(Logger logger)
//     {
//         foreach (var node in Hierarchy.OrderByHierarchy(Nodes.Values.ToImmutableArray()!, x => x.Target.Actor, out _, out _))
//         {
//             var nodeLogger = logger.GetSubLogger(node.Target.Actor.ToFullMetadataName()).WithCleanLogFile();
//             
//             try
//             {
//                 var start = DateTimeOffset.UtcNow;
//                 
//                 node.VisitTree(Context, nodeLogger);
//
//                 var dt = DateTimeOffset.UtcNow - start;
//                 logger.Log($"{node.Target.Actor}: visited in {dt.TotalMilliseconds}ms ");
//             }
//             catch (Exception x)
//             {
//                 nodeLogger.Log(LogLevel.Error, $"Failed visiting node {node.Target.Actor}: {x}");
//                 logger.Log(LogLevel.Error, $"Failed visiting node {node.Target.Actor}: {x}");
//                 throw;
//             }
//             finally
//             {
//                 nodeLogger.Flush();
//                 logger.Flush();
//             }
//         }
//     }
//
//     public bool Equals(LinkGraph? other)
//     {
//         if (other is null) return false;
//         if (ReferenceEquals(this, other)) return true;
//         return Nodes.Values.SequenceEqual(other.Nodes.Values);
//     }
//
//     public override bool Equals(object? obj)
//     {
//         return ReferenceEquals(this, obj) || obj is LinkGraph other && Equals(other);
//     }
//
//     public override int GetHashCode()
//     {
//         return Nodes.GetHashCode();
//     }
//
//     public static bool operator ==(LinkGraph? left, LinkGraph? right)
//     {
//         return Equals(left, right);
//     }
//
//     public static bool operator !=(LinkGraph? left, LinkGraph? right)
//     {
//         return !Equals(left, right);
//     }
// }