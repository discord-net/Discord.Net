using System.Text;
using Discord.Net.Hanz.Nodes;
using Discord.Net.Hanz.Tasks.Actors.Common;
using Discord.Net.Hanz.Tasks.Actors.Nodes;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.TraitsV2.Nodes;

public readonly record struct ActorPathingInfo(
    ActorsTask.ActorHierarchy Hierarchy,
    ActorRelationships Relationships
)
{
    public string? ResolveRouteParameterUsingPathable(RouteParameter parameter)
        => ResolveRouteParameterUsingPathable(parameter, "path");

    public string? ResolveRouteParameterUsingPathable(RouteParameter parameter, string pathableName)
    {
        if (parameter.Heuristics.Count == 0) return null;

        return $"{pathableName}.Require<{parameter.Heuristics[0]}>()";
    }
}

public abstract record TraitImplementationTarget(AssemblyTarget Assembly)
{
    public abstract TypeRef Type { get; }
    public abstract TypeRef Id { get; }
    public abstract TypeRef Entity { get; }
    public abstract TypeRef Model { get; }

    public string FormattedBackLinkOfType(TypeRef type)
        => $"Discord.IBackLink<{type}, {Type}, {Id}, {Entity}, {Model}>";
}

public sealed record ActorTraitImplementationTarget(
    ActorInfo ActorInfo
) : TraitImplementationTarget(ActorInfo.Assembly)
{
    public override TypeRef Type => ActorInfo.Actor;
    public override TypeRef Id => ActorInfo.Id;
    public override TypeRef Entity => ActorInfo.Entity;
    public override TypeRef Model => ActorInfo.Model;
}

public sealed record CustomTraitImplementationTarget(
    ActorTraitInfo TraitInfo
) : TraitImplementationTarget(TraitInfo.Assembly)
{
    public override TypeRef Type => TraitInfo.Trait;
    public override TypeRef Id => TraitInfo.Id;
    public override TypeRef Entity => TraitInfo.Entity;
    public override TypeRef Model => TraitInfo.Model;
}

public sealed record TraitTargetAncestor(
    TraitImplementationTarget Target,
    bool? IsEntityAssignable
);

public abstract class TraitNode : Node
{
    protected IncrementalKeyValueProvider<ActorInfo, ActorPathingInfo> PathingInfoProvider { get; }

    protected IncrementalKeyValueProvider<string, TraitImplementationTarget> TargetsProvider { get; }

    protected IncrementalKeyValueProvider<TraitImplementationTarget, ImmutableEquatableArray<TraitTargetAncestor>>
        TargetAncestorsProvider { get; }

    protected TraitNode(IncrementalGeneratorInitializationContext context, ILogger logger) : base(context, logger)
    {
        PathingInfoProvider = GetTask<ActorsTask>()
            .ActorHierarchies
            .JoinByKey(
                GetNode<ActorNode>().Relationships,
                ActorPathingInfo? (info, hierarchy, relationships) =>
                {
                    if (hierarchy == default || relationships == default)
                        return null;

                    return new ActorPathingInfo(hierarchy, relationships);
                }
            )
            .Where((_, x) => x != null)
            .MapValues((_, x) => x!.Value);

        TargetsProvider = GetTask<ActorsTask>()
            .ActorInfos
            .ValuesProvider
            .Select(TraitImplementationTarget (info, _) => new ActorTraitImplementationTarget(info))
            .Concat(
                GetTask<TraitsTask>()
                    .TraitInfos
                    .ValuesProvider
                    .MaybeSelect(info =>
                        info is ActorTraitInfo actorTraitInfo
                            ? new CustomTraitImplementationTarget(actorTraitInfo)
                                .Some<TraitImplementationTarget>()
                            : default
                    )
            )
            .KeyedBy(x => x.Type.DisplayString);

        TargetAncestorsProvider = TargetsProvider
            .ValuesProvider
            .DependsOn(GetTask<ActorsTask>().ActorHierarchies)
            .DependsOn(GetTask<TraitsTask>().TraitAncestors)
            .KeyedBy(
                x => x,
                x => x switch
                {
                    ActorTraitImplementationTarget actor => GetTask<ActorsTask>()
                        .ActorHierarchies
                        .GetValueOrDefault(actor.ActorInfo)?
                        .ParentInfos
                        .Select(x =>
                            TargetsProvider.TryGetValue(x.ActorInfo.Actor.DisplayString, out var target)
                                ? new TraitTargetAncestor(target, x.IsEntityAssignable).Some()
                                : default
                        )
                        .Where(x => x.HasValue)
                        .Select(x => x.Value)
                        .ToImmutableEquatableArray(),
                    CustomTraitImplementationTarget custom => GetTask<TraitsTask>()
                        .TraitAncestors
                        .GetValueOrDefault(custom.TraitInfo, ImmutableEquatableArray<TraitAncestor>.Empty)!
                        .Select(x =>
                            TargetsProvider.TryGetValue(x.Info.Trait.DisplayString, out var target)
                                ? new TraitTargetAncestor(target, x.IsEntityAssignable).Some()
                                : default
                        )
                        .Where(x => x.HasValue)
                        .Select(x => x.Value)
                        .ToImmutableEquatableArray(),
                    _ => ImmutableEquatableArray<TraitTargetAncestor>.Empty
                }
            )!;
    }

    protected bool HasChildren(TraitImplementationTarget target)
        => TargetAncestorsProvider.Values.Any(x => x.Any(x => x.Target == target));

    protected bool HasAncestors(TraitImplementationTarget target)
        => TargetAncestorsProvider.TryGetValue(target, out var ancestors) && ancestors.Count > 0;
    
    protected StatefulGeneration<T> CreateContainer<T>(TraitImplementationTarget target, T state)
        => new(
            state, TypeSpec.From(target.Type) with
            {
                Modifiers = new(["partial"]),
            }
        );
}