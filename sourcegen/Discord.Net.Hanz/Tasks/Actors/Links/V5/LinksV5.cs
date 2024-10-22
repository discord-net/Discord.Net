using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5;

public class LinksV5 : GenerationTask
{
    public LinksV5(
        Context context,
        Logger logger
    ) : base(context, logger)
    {
        var actorTask = context.GetTask<LinkActorTargets>();
        var schematicTask = context.GetTask<LinkSchematics>();

        var provider = schematicTask.Schematics
            .Combine(actorTask.Actors.Collect())
            .SelectMany((x, _) => x.Right.Select(y => new NodeContext(x.Left, y)));

        Node.GetInstance<ActorNode>(provider);
    }
    
    
    public readonly struct NodeContext : IEquatable<NodeContext>
    {
        public readonly LinkSchematics.Schematic Schematic;
        public readonly LinkActorTargets.GenerationTarget Target;

        public NodeContext(LinkSchematics.Schematic schematic, LinkActorTargets.GenerationTarget target)
        {
            Schematic = schematic;
            Target = target;
        }

        public override int GetHashCode()
            => HashCode.Of(Schematic).And(Target);

        public bool Equals(NodeContext other)
            => Schematic.Equals(other.Schematic) && Target.Equals(other.Target);
    }
}