using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Types;

public class PagedNode :
    Node,
    ILinkImplmenter
{
    public readonly record struct State(
        bool PagesEntity,
        ActorInfo ActorInfo,
        string PagedType,
        string PagingProviderType,
        string ReferenceName,
        ImmutableEquatableArray<(string AsyncPagedType, string OverrideTarget)> Ancestors)
    {
        public string AsyncPagedType => $"IAsyncPaged<{PagedType}>";

        public static State Create(LinkNode.State link, Grouping<string, ActorInfo> ancestorGrouping)
        {
            var pagesEntity = link.Entry.Type.Generics.Length == 1;

            var pagedType = pagesEntity
                ? link.ActorInfo.Entity.FullyQualifiedName
                : link.Entry.Type.Generics[0];

            var ancestors = ancestorGrouping.GetGroupOrEmpty(link.ActorInfo.Actor.DisplayString);

            return new State(
                pagesEntity,
                link.ActorInfo,
                pagedType,
                $"Func<{link.ActorInfo}.{link.Entry.Type.ReferenceName}, TParams?, RequestOptions?, IAsyncPaged<{pagedType}>>",
                link.Entry.Type.ReferenceName,
                new(
                    ancestors.Select(x =>
                        (
                            $"IAsyncPaged<{(pagesEntity ? x.Entity.FullyQualifiedName : pagedType)}>",
                            ancestorGrouping.GetGroupOrEmpty(x.Actor.DisplayString).Count > 0
                                ? $"{x.Actor}.{link.Path.FormatRelative()}"
                                : $"{x.FormattedLinkType}.{link.Entry.Type.ReferenceName}"
                        )
                    )
                )
            );
        }
    }
    
    private readonly IncrementalValueProvider<Grouping<string, ActorInfo>> _ancestors;

    public PagedNode(NodeProviders providers, Logger logger) : base(providers, logger)
    {
        _ancestors = providers.ActorAncestors;
    }

    private static bool WillGenerate(LinkNode.State state)
        => state is {IsTemplate: true, Entry.Type.Name: "Paged"};

    public IncrementalValuesProvider<Branch<ILinkImplmenter.LinkImplementation>> Branch(
        IncrementalValuesProvider<Branch<LinkNode.State>> provider)
    {
        return provider
            .Where(WillGenerate)
            .Combine(_ancestors)
            .Select((tuple, _) => tuple.Left
                .Mutate(State.Create(tuple.Left.Value, tuple.Right))
            )
            .Select((branch, token) => branch.Mutate(CreateImplmentation(branch.Value, token)));
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
            Methods: new([
                new MethodSpec(
                    Name: "PagedAsync",
                    ReturnType: state.AsyncPagedType,
                    Modifiers: new(["new"]),
                    Parameters: new([
                        ("TParams?", "args", "default"),
                        ("RequestOptions?", "options", "null"),
                    ])
                ),
                new MethodSpec(
                    Name: "PagedAsync",
                    ReturnType: state.AsyncPagedType,
                    ExplicitInterfaceImplementation: $"{state.ActorInfo.FormattedLinkType}.{state.ReferenceName}",
                    Parameters: new([
                        ("TParams?", "args", "default"),
                        ("RequestOptions?", "options", "null"),
                    ]),
                    Expression: "PagedAsync(args, options)"
                ),
                ..state.Ancestors.Select(x => 
                    new MethodSpec(
                        Name: "PagedAsync",
                        ReturnType: x.AsyncPagedType,
                        ExplicitInterfaceImplementation: x.OverrideTarget,
                        Parameters: new([
                            ("TParams?", "args", "default"),
                            ("RequestOptions?", "options", "null"),
                        ]),
                        Expression: "PagedAsync(args, options)"
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