using System.Reflection.Metadata;
using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Types;

public class EnumerableLink :
    Node,
    ILinkImplmenter
{
    public readonly record struct StateWithParameterInfo(
        Branch<State, LinkNode.State> State,
        ExtraParameters? ExtraParameters,
        ImmutableEquatableArray<ExtraParameters> AncestorExtraParameters
    )
    {
        public bool RedefinesLinkMembers
            => State.Value.IsTemplate &&
               (
                   State.Value.AncestorOverrides.Count > 0
                   ||
                   !State.Value.ActorInfo.IsCore
                   || 
                   ExtraParameters is {Parameters.Count: > 0}
               );
    }

    public readonly record struct State(
        ActorInfo ActorInfo,
        ImmutableEquatableArray<(string Actor, string Entity, string OverrideTarget)> AncestorOverrides,
        bool RedefinesLinkMembers,
        bool IsTemplate
    );

    public readonly record struct ExtraParameters(
        string Actor,
        ImmutableEquatableArray<ParameterSpec> Parameters
    )
    {
        public static ExtraParameters Create(
            LinkActorTargets.GenerationTarget target,
            CancellationToken token)
        {
            if (target.Assembly is not LinkActorTargets.AssemblyTarget.Core)
            {
                var fetchableOfManyMethod = target.GetCoreEntity()
                    .GetMembers("FetchManyRoute")
                    .OfType<IMethodSymbol>()
                    .FirstOrDefault();

                if (fetchableOfManyMethod is null || fetchableOfManyMethod.Parameters.Length == 1)
                    goto returnEmpty;

                return new ExtraParameters(
                    target.Actor.ToDisplayString(),
                    new ImmutableEquatableArray<ParameterSpec>(
                        fetchableOfManyMethod
                            .Parameters
                            .Skip(1)
                            .Where(x => x.HasExplicitDefaultValue)
                            .Select(ParameterSpec.From)
                    )
                );
            }

            var fetchableOfManyAttribute = target.GetCoreEntity()
                .GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == "FetchableOfManyAttribute");

            if (fetchableOfManyAttribute is null)
                goto returnEmpty;

            if (EntityTraits.GetNameOfArgument(fetchableOfManyAttribute) is not MemberAccessExpressionSyntax
                routeMemberAccess)
                goto returnEmpty;

            var route = EntityTraits.GetRouteSymbol(
                routeMemberAccess,
                target.SemanticModel.Compilation.GetSemanticModel(routeMemberAccess.SyntaxTree)
            );

            return new ExtraParameters(
                target.Actor.ToDisplayString(),
                route is IMethodSymbol method && ParseExtraArgs(method) is { } extra
                    ? new(extra.Select(ParameterSpec.From))
                    : ImmutableEquatableArray<ParameterSpec>.Empty
            );

            returnEmpty:
            return new(target.Actor.ToDisplayString(), ImmutableEquatableArray<ParameterSpec>.Empty);

            static List<IParameterSymbol> ParseExtraArgs(IMethodSymbol symbol)
            {
                var args = new List<IParameterSymbol>();

                foreach (var parameter in symbol.Parameters)
                {
                    var heuristic = parameter.GetAttributes()
                        .FirstOrDefault(x => x.AttributeClass?.Name == "IdHeuristicAttribute");

                    if (heuristic is not null)
                    {
                        continue;
                    }

                    if (parameter.Name is "id") continue;

                    if (!parameter.HasExplicitDefaultValue) continue;

                    args.Add(parameter);
                }

                return args;
            }
        }
    }


    private readonly IncrementalValueProvider<Grouping<string, ExtraParameters>> _extraParametersProvider;

    public EnumerableLink(
        NodeProviders providers,
        Logger logger
    ) : base(providers, logger)
    {
        _extraParametersProvider = providers
            .Actors
            .Select(ExtraParameters.Create)
            .Where(x => x.Parameters.Count > 0)
            .GroupBy(x => x.Actor);
    }

    public IncrementalValuesProvider<Branch<ILinkImplmenter.LinkImplementation, LinkNode.State>> Branch(
        IncrementalValuesProvider<Branch<LinkNode.State, LinkNode.State>> provider)
    {
        return provider
            .Where(x => x.Value.Entry.Type.Name == "Enumerable")
            .Select((branch, token) => branch
                .Mutate(
                    new State(
                        branch.Value.ActorInfo,
                        new(
                            branch.Value.Actor.State.Ancestors
                                .Select(x =>
                                    (
                                        x.ActorInfo.Actor.DisplayString,
                                        x.ActorInfo.Entity.FullyQualifiedName,
                                        x.Ancestors.Count > 0
                                            ? $"{x.ActorInfo.Actor}.{string.Join(".", branch.Value.Parts)}"
                                            : $"{x.ActorInfo.FormattedLinkType}.Enumerable"
                                    )
                                )
                        ),
                        branch.Value.IsTemplate || branch.Value.Actor.State.Ancestors.Count > 0,
                        branch.Value.IsTemplate
                    )
                )
            )
            .Combine(
                _extraParametersProvider,
                branch => branch.Value.ActorInfo.Actor.DisplayString,
                (branch, extraParameters, graph) => new StateWithParameterInfo(
                    branch,
                    extraParameters.Count > 0 ? extraParameters[0] : null,
                    new(
                        branch.Value
                            .AncestorOverrides
                            .SelectMany(x =>
                                graph.GetGroupOrEmpty(x.Actor)
                            )
                    )
                )
            )
            .Where(x => x.RedefinesLinkMembers)
            .Select((context, token) => context
                .State
                .Mutate(
                    CreateImplementation(context, token)
                )
            );
    }

    private ILinkImplmenter.LinkImplementation CreateImplementation(
        StateWithParameterInfo context,
        CancellationToken token
    ) => new(
        CreateInterfaceSpec(context, token),
        CreateImplementationSpec(context, token)
    );

    private ILinkImplmenter.LinkSpec CreateInterfaceSpec(
        StateWithParameterInfo context,
        CancellationToken token)
    {
        using var logger = Logger
            .GetSubLogger(context.State.Value.ActorInfo.Assembly.ToString())
            .GetSubLogger(nameof(CreateInterfaceSpec))
            .GetSubLogger(context.State.Value.ActorInfo.Actor.MetadataName);

        logger.Log($"{context.State.Value.ActorInfo.Actor}");
        logger.Log($" - {context.State.Value.RedefinesLinkMembers}");
        logger.Log($" - {context.ExtraParameters}");

        var parameters = new ImmutableEquatableArray<ParameterSpec>([
            ("RequestOptions?", "options", "null"),
            ("CancellationToken", "token", "default")
        ]);

        var parametersWithExtra = parameters;

        if (context.ExtraParameters.HasValue)
        {
            parametersWithExtra = new([
                ..context.ExtraParameters.Value.Parameters,
                ..parameters
            ]);
        }

        var spec = new ILinkImplmenter.LinkSpec(
            Methods: new ImmutableEquatableArray<MethodSpec>([
                new MethodSpec(
                    Name: "AllAsync",
                    ReturnType: $"ITask<IReadOnlyCollection<{context.State.Value.ActorInfo.Entity}>>",
                    Parameters: parametersWithExtra,
                    Modifiers: new(["new"])
                ),
                new MethodSpec(
                    Name: "AllAsync",
                    ReturnType: $"ITask<IReadOnlyCollection<{context.State.Value.ActorInfo.Entity}>>",
                    Parameters: parameters,
                    ExplicitInterfaceImplementation: $"{context.State.Value.ActorInfo.FormattedLinkType}.Enumerable",
                    Expression: "AllAsync(options: options, token: token)"
                )
            ])
        );

        foreach (var ancestor in context.State.Value.AncestorOverrides)
        {
            var overrideParameters = parameters;

            if (
                context.ExtraParameters.HasValue &&
                context.AncestorExtraParameters
                        .FirstOrDefault(x => x.Actor == ancestor.Actor)
                    is { } ancestorExtra &&
                ancestorExtra.Parameters.Equals(context.ExtraParameters.Value.Parameters)
            )
            {
                overrideParameters = parametersWithExtra;
            }

            spec = spec with
            {
                Methods = spec.Methods.AddRange(
                    new MethodSpec(
                        Name: "AllAsync",
                        ReturnType: $"ITask<IReadOnlyCollection<{ancestor.Entity}>>",
                        Parameters: overrideParameters,
                        ExplicitInterfaceImplementation: ancestor.OverrideTarget,
                        Expression:
                        $"AllAsync({string.Join(", ", overrideParameters.Select(x => $"{x.Name}: {x.Name}"))})"
                    )
                )
            };
        }

        return spec;
    }

    private ILinkImplmenter.LinkSpec CreateImplementationSpec(
        StateWithParameterInfo context,
        CancellationToken token)
    {
        return ILinkImplmenter.LinkSpec.Empty;
    }
}