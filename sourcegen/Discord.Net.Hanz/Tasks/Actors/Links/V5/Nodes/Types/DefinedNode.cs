using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Types;

public class DefinedNode :
    Node,
    ILinkImplmenter
{
    public readonly record struct State(
        ActorInfo Actor,
        ImmutableEquatableArray<(string Id, string OverrideTarget)> AncestorOverrides)
    {
        public static State Create(
            LinkNode.State link,
            Grouping<string, ActorInfo> ancestors)
        {
            return new State(
                link.ActorInfo,
                ancestors
                    .GetGroupOrEmpty(link.ActorInfo.Actor.DisplayString)
                    .Select(x =>
                        (
                            x.Id.FullyQualifiedName,
                            ancestors.GetGroupOrEmpty(x.Actor.DisplayString).Count > 0
                                ? $"{x.Actor}.{link.Path.FormatRelative()}"
                                : $"{x.FormattedLinkType}.Defined"
                        )
                    )
                    .ToImmutableEquatableArray()
            );
        }
    }

    private readonly IncrementalValueProvider<Grouping<string, ActorInfo>> _ancestors;

    public DefinedNode(NodeProviders providers, Logger logger) : base(providers, logger)
    {
        _ancestors = providers.ActorAncestors;
    }

    public IncrementalValuesProvider<Branch<ILinkImplmenter.LinkImplementation>> Branch(
        IncrementalValuesProvider<Branch<LinkNode.State>> provider)
    {
        return provider
            .Where(x => x.Entry.Type.Name == "Defined")
            .Combine(_ancestors)
            .Select((tuple, _) => tuple.Left.Mutate(State.Create(tuple.Left.Value, tuple.Right)))
            .Select(CreateImplmentation);
    }

    private ILinkImplmenter.LinkImplementation CreateImplmentation(State state, CancellationToken token)
    {
        return new ILinkImplmenter.LinkImplementation(
            CreateInterfaceSpec(state, token),
            CreateImplementationSpec(state, token)
        );
    }

    private ILinkImplmenter.LinkSpec CreateInterfaceSpec(State state, CancellationToken token)
    {
        return new ILinkImplmenter.LinkSpec(
            Properties: new([
                new PropertySpec(
                    Type: $"IReadOnlyCollection<{state.Actor.Id}>",
                    Name: "Ids",
                    Modifiers: new(["new"])
                ),
                new PropertySpec(
                    Type: $"IReadOnlyCollection<{state.Actor.Id}>",
                    Name: "Ids",
                    ExplicitInterfaceImplementation: $"{state.Actor.FormattedLinkType}.Defined",
                    Expression: "Ids"
                ),
                ..state.AncestorOverrides.Select(x =>
                    new PropertySpec(
                        Type: $"IReadOnlyCollection<{x.Id}>",
                        Name: "Ids",
                        ExplicitInterfaceImplementation: x.OverrideTarget,
                        Expression: "Ids"
                    )
                )
            ])
        );
    }

    private ILinkImplmenter.LinkSpec CreateImplementationSpec(State state, CancellationToken token)
    {
        return ILinkImplmenter.LinkSpec.Empty;
    }
}