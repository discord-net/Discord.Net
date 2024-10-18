using Discord.Net.Hanz.Tasks.Actors.V3;
using Microsoft.CodeAnalysis;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

public sealed class NodeContext(LinkGraph graph)
{
    public LinkGraph Graph { get; } = graph;

    public bool TryGetBaseTarget(LinkTarget target, out ActorNode node)
    {
        node = null!;

        if (target.Assembly is LinkActorTargets.AssemblyTarget.Core) return false;

        if (target.Actor.BaseType is null) return false;

        return Graph.Nodes.TryGetValue(target.Actor.BaseType, out node);
    }

    public bool TryGetChildTarget(LinkTarget target, out ActorNode node)
    {
        node = null!;

        if (target.Assembly is LinkActorTargets.AssemblyTarget.Core) return false;

        node = Graph.Nodes
            .FirstOrDefault(x =>
                x.Key.BaseType?.Equals(
                    target.Actor,
                    SymbolEqualityComparer.Default
                ) ?? false
            ).Value;

        return node is not null;
    }
}