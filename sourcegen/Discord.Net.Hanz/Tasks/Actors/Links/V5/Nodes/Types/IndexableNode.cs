using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Types;

public class IndexableNode : 
    Node,
    ILinkImplmenter
{
    private readonly record struct State(
        ActorInfo ActorInfo,
        bool RedefinesLinkMembers,
        ImmutableEquatableArray<(string Actor, string OverrideTarget)> AncestorOverrides
    )
    {
        public static State Create(LinkNode.State link, Grouping<string, ActorInfo> ancestorGrouping)
        {
            var ancestors = ancestorGrouping.GetGroupOrEmpty(link.ActorInfo.Actor.DisplayString);
            
            return new State(
                link.ActorInfo,
                ancestors.Count > 0 || !link.ActorInfo.IsCore,
                new(
                    ancestors
                        .Select(x =>
                            (
                                x.Actor.FullyQualifiedName,
                                ancestorGrouping.GetGroupOrEmpty(x.Actor.DisplayString).Count > 0
                                    ? $"{x.Actor}.{link.Path.FormatRelative()}"
                                    : $"{x.FormattedLinkType}.Indexable"
                            )
                        )
                )
            );
        }
    }
    
    private readonly IncrementalValueProvider<Grouping<string, ActorInfo>> _ancestors;
    
    public IndexableNode(NodeProviders providers, Logger logger) : base(providers, logger)
    {
        _ancestors = providers.ActorAncestors;
    }
    
    public IncrementalValuesProvider<Branch<ILinkImplmenter.LinkImplementation>> Branch(
        IncrementalValuesProvider<Branch<LinkNode.State>> provider)
    {
        return provider
            .Where(x => x.Value is {IsTemplate: true, Entry.Type.Name: "Indexable"})
            .Combine(_ancestors)
            .Select((tuple, _) => tuple.Left
                .Mutate(State.Create(tuple.Left.Value, tuple.Right))
            )
            .Select((x, token) => x.Mutate(Build(x.Value, token)));
    }

    private ILinkImplmenter.LinkImplementation Build(State state, CancellationToken token)
    {
        using var logger = Logger
            .GetSubLogger(state.ActorInfo.Assembly.ToString())
            .GetSubLogger(nameof(Build))
            .GetSubLogger(state.ActorInfo.Actor.MetadataName);
        
        logger.Log("Building indexable link");
        logger.Log($" - {state.ActorInfo.Actor.FullyQualifiedName}");
        
        return new ILinkImplmenter.LinkImplementation(
            CreateInterfaceSpec(state, token),
            CreateImplementationSpec(state, token)
        );
    }

    private static ILinkImplmenter.LinkSpec CreateInterfaceSpec(State state, CancellationToken token)
    {
        var spec = new ILinkImplmenter.LinkSpec(
            Indexers: new([
                new IndexerSpec(
                    Type: state.ActorInfo.Actor.FullyQualifiedName,
                    Modifiers: new(state.RedefinesLinkMembers ? ["new"] : []),
                    Accessibility: Accessibility.Internal,
                    Parameters: new([
                        (state.ActorInfo.FormattedIdentifiable, "identity")
                    ]),
                    Expression: "identity.Actor ?? GetActor(identity.Id)"
                )
            ])
        );

        if (!state.ActorInfo.IsCore)
        {
            spec = spec with
            {
                Indexers = spec.Indexers.AddRange(
                    new IndexerSpec(
                        Type: state.ActorInfo.CoreActor.FullyQualifiedName,
                        Parameters: new([
                            (state.ActorInfo.FormattedIdentifiable, "identity")
                        ]),
                        Expression: "identity.Actor ?? GetActor(identity.Id)",
                        ExplicitInterfaceImplementation: $"{state.ActorInfo.CoreActor}.Indexable"
                    ),
                    new IndexerSpec(
                        Type: state.ActorInfo.CoreActor.FullyQualifiedName,
                        Parameters: new([
                            (state.ActorInfo.Id.FullyQualifiedName, "id")
                        ]),
                        Expression: "this[id]",
                        ExplicitInterfaceImplementation: $"{state.ActorInfo.FormattedCoreLinkType}.Indexable"
                    )
                ),
                Methods = spec.Methods.AddRange(
                    new MethodSpec(
                        Name: "Specifically",
                        ReturnType: state.ActorInfo.Actor.FullyQualifiedName,
                        ExplicitInterfaceImplementation: $"{state.ActorInfo.FormattedCoreLinkType}.Indexable",
                        Parameters: new([
                            (state.ActorInfo.Id.FullyQualifiedName, "id")
                        ]),
                        Expression: "Specifically(id)"
                    )
                )
            };
        }

        if (!state.RedefinesLinkMembers)
            return spec;

        return spec with
        {
            Indexers = spec.Indexers.AddRange([
                new IndexerSpec(
                    Type: state.ActorInfo.Actor.FullyQualifiedName,
                    Modifiers: new(["new"]),
                    Parameters: new([
                        (state.ActorInfo.Id.FullyQualifiedName, "id")
                    ]),
                    Expression: $"(this as {state.ActorInfo.FormattedActorProvider}).GetActor(id)"
                ),
                ..state.AncestorOverrides.Select(x =>
                    new IndexerSpec(
                        Type: x.Actor,
                        Parameters: new([
                            (state.ActorInfo.Id.FullyQualifiedName, "id")
                        ]),
                        ExplicitInterfaceImplementation: x.OverrideTarget,
                        Expression: "this[id]"
                    )
                )
            ]),
            Methods = spec.Methods.AddRange([
                new MethodSpec(
                    Name: "Specifically",
                    ReturnType: state.ActorInfo.Actor.FullyQualifiedName,
                    Modifiers: new(["new"]),
                    Parameters: new([
                        (state.ActorInfo.Id.FullyQualifiedName, "id")
                    ]),
                    Expression: $"(this as {state.ActorInfo.FormattedActorProvider}).GetActor(id)"
                ),
                ..state.AncestorOverrides.Select(x =>
                    new MethodSpec(
                        Name: "Specifically",
                        ReturnType: x.Actor,
                        Parameters: new([
                            (state.ActorInfo.Id.FullyQualifiedName, "id")
                        ]),
                        ExplicitInterfaceImplementation: x.OverrideTarget,
                        Expression: "Specifically(id)"
                    )
                )
            ])
        };
    }

    private static ILinkImplmenter.LinkSpec CreateImplementationSpec(State state, CancellationToken token)
    {
        return ILinkImplmenter.LinkSpec.Empty;
    }
}