using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

public sealed class NodeContext(LinkGraph graph)
{
    public LinkGraph Graph { get; } = graph;
}