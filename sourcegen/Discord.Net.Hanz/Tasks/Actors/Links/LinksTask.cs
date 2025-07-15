using System.Collections.Immutable;
using System.Diagnostics;
using Discord.Net.Hanz.Tasks.Actors.Common;
using Discord.Net.Hanz.Tasks.Actors.TraitsV2;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links;

public record LinkTargetAncestor(
    ActorOrTraitInfo Info,
    bool? IsEntityAssignable
);

public class LinksTask : GenerationTask
{
    public IncrementalValuesProvider<NodeContext> NodeContexts { get; }

    public IncrementalKeyValueProvider<string, ActorOrTraitInfo> TargetsProvider { get; }

    public IncrementalKeyValueProvider<ActorOrTraitInfo, ImmutableEquatableArray<LinkTargetAncestor>>
        TargetAncestorsProvider { get; }


    private readonly ILogger _logger;

    public LinksTask(
        IncrementalGeneratorInitializationContext context,
        ILogger logger
    ) : base(context, logger)
    {
        _logger = logger;

        var actorTask = GetTask<ActorsTask>(context);
        var schematicTask = GetTask<LinkSchematics>(context);
        var traitsTask = GetTask<TraitsTask>(context);

        NodeContexts = schematicTask.Schematics
            .Combine(actorTask.Actors.Collect())
            .SelectMany((x, _) => x.Right.Select(y => new NodeContext(x.Left, y)));

        TargetsProvider = actorTask
            .ActorInfos
            .ValuesProvider
            .Select(ActorOrTraitInfo (x, _) => x)
            .Concat(
                traitsTask
                    .TraitInfos
                    .ValuesProvider
                    .MaybeSelect(info =>
                        info is ActorTraitInfo actorTrait ? actorTrait.Some<ActorOrTraitInfo>() : default
                    )
            )
            .KeyedBy(x => x.Type.DisplayString);

        TargetAncestorsProvider = TargetsProvider
            .ValuesProvider
            .DependsOn(actorTask.ActorAncestors)
            .DependsOn(traitsTask.TraitAncestors)
            .KeyedBy(
                x => x,
                x => x switch
                {
                    ActorTraitInfo actorTrait => traitsTask
                        .TraitAncestors
                        .GetValueOrDefault(actorTrait, ImmutableEquatableArray<TraitAncestor>.Empty)!
                        .Where(x => x.Info is ActorTraitInfo)
                        .Select(x =>
                            new LinkTargetAncestor(
                                (ActorTraitInfo) x.Info,
                                x.IsEntityAssignable
                            )
                        )
                        .ToImmutableEquatableArray(),
                    ActorInfo actorInfo => actorTask
                        .ActorAncestors
                        .GetValueOrDefault(actorInfo)!
                        .Select(x =>
                            new LinkTargetAncestor(x.ActorInfo, x.IsEntityAssignable)
                        )
                        .ToImmutableEquatableArray(),
                    _ => ImmutableEquatableArray<LinkTargetAncestor>.Empty
                }
            );
    }

    public readonly struct NodeContext : IEquatable<NodeContext>
    {
        public readonly LinkSchematics.Schematic Schematic;
        public readonly ActorsTask.ActorSymbols Target;

        public NodeContext(LinkSchematics.Schematic schematic, ActorsTask.ActorSymbols target)
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