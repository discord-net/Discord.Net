using System.Collections.Immutable;
using System.Diagnostics;
using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5;

public class LinksV5 : GenerationTask
{
    private readonly Logger _logger;

    public LinksV5(
        IncrementalGeneratorInitializationContext context,
        Logger logger
    ) : base(context, logger)
    {
        _logger = logger;

        var actorTask = GetTask<LinkActorTargets>(context);
        var schematicTask = GetTask<LinkSchematics>(context);

        var provider = schematicTask.Schematics
            .Combine(actorTask.Actors.Collect())
            .SelectMany((x, _) => x.Right.Select(y => new NodeContext(x.Left, y)));

        context
            .RegisterSourceOutput(
                Node.Create(
                    new NodeProviders(
                        schematicTask.Schematics,
                        actorTask.Actors,
                        provider
                    )
                ),
                Generate
            );
    }

    private void Generate(
        SourceProductionContext context,
        Node.StatefulGeneration<ActorNode.IntrospectedBuildState> result)
    {
        var outLogger = _logger
            .GetSubLogger(result.State.ActorInfo.Assembly.ToString())
            .GetSubLogger(result.State.ActorInfo.Actor.MetadataName)
            .WithCleanLogFile();

        //Debugger.Launch();

        try
        {
            var type = result.Spec.ToString();
            outLogger.Log($"Writing {result.State.ActorInfo.Actor}...\n{type}");


            context.AddSource(
                $"LinksV5/{result.State.ActorInfo.Actor.MetadataName}",
                $$"""

                  namespace {{result.State.ActorInfo.Actor.Namespace}};

                  {{type}}
                  """
            );
        }
        catch (Exception e)
        {
            outLogger.Log(LogLevel.Error, $"Failed: {e}");
        }
        finally
        {
            outLogger.Flush();
        }
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