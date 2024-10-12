using Discord.Net.Hanz.Tasks.Actors.V3;
using Microsoft.CodeAnalysis;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;

public class PagedNode(LinkTarget target, LinkSchematics.Entry entry) : LinkTypeNode(target, entry)
{
    public bool PagesEntity => Entry.Symbol.TypeArguments.Length == 1;

    public ITypeSymbol PagedEntityType
        => Entry.Symbol.TypeArguments.Length == 2
            ? Entry.Symbol.TypeParameters[0]
            : Target.Entity;

    public string PagingProviderType
        => $"Discord.IPagedLinkProvider<{PagedEntityType}, TParams>";

    private protected override void Visit(NodeContext context, Logger logger)
    {
        RedefinesLinkMembers = GetEntityAssignableAncestors(context).Length > 0;

        ImplementationMembers.Clear();

        ImplementationMembers.Add(
            (PagingProviderType, "PagingProvider", null)
        );

        base.Visit(context, logger);
    }

    protected override void AddMembers(List<string> members, NodeContext context, Logger logger)
    {
        if (!RedefinesLinkMembers) return;

        var ancestors = GetEntityAssignableAncestors(context);

        members.AddRange([
            $"new IAsyncPaged<{PagedEntityType}> PagedAsync(TParams? args = default, RequestOptions? options = null);",
            $"IAsyncPaged<{PagedEntityType}> {FormattedLinkType}.{LinksV3.FormatTypeName(Entry.Symbol)}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);",
        ]);

        if (ParentLinks.Any())
        {
            members.AddRange([
                $"IAsyncPaged<{PagedEntityType}> {Target.Actor}.{LinksV3.FormatTypeName(Entry.Symbol)}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);"
            ]);
        }

        foreach (var ancestor in ancestors)
        {
            var overrideType =
                $"{(
                    ancestor.GetEntityAssignableAncestors(context).Length > 0
                        ? $"{ancestor.Target.Actor}{FormatRelativeTypePath()}"
                        : ancestor.FormattedLinkType
                )}.{LinksV3.FormatTypeName(Entry.Symbol)}";

            var ancestorPagedType = PagesEntity ? ancestor.Target.Entity.ToDisplayString() : "TPaged";

            members.AddRange([
                $"IAsyncPaged<{ancestorPagedType}> {overrideType}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);"
            ]);
        }
    }

    protected override void CreateImplementation(
        List<string> members,
        List<string> bases,
        NodeContext context,
        Logger logger)
    {
        var memberModifier = ImplementationBase is not null
            ? "override "
            : ImplementationChild is not null
                ? "virtual "
                : string.Empty;

        var overrideType = RedefinesLinkMembers
            ? FormatAsTypePath()
            : $"{FormattedCoreLinkType}.{GetTypeName()}";

        members.Add(
            $"""
             public {memberModifier}IAsyncPaged<{PagedEntityType}> PagedAsync(TParams? args = default, RequestOptions? options = null)
                 => PagingProvider.PagedAsync(args, options);
             """
        );

        if (RedefinesLinkMembers)
        {
            members.Add(
                $"""
                 IAsyncPaged<{PagedEntityType}> {FormatAsTypePath()}.PagedAsync(TParams? args, RequestOptions? options)
                     => PagedAsync(args, options);
                 """
            );
        }
        else
        {
            var corePagedEntityType = PagesEntity ? Target.GetCoreEntity() : PagedEntityType;
            
            members.AddRange([
                $"""
                 IAsyncPaged<{PagedEntityType}> {FormattedLinkType}.{GetTypeName()}.PagedAsync(TParams? args, RequestOptions? options)
                     => PagedAsync(args, options);
                 """,
                $"""
                 IAsyncPaged<{corePagedEntityType}> {FormattedCoreLinkType}.{GetTypeName()}.PagedAsync(TParams? args, RequestOptions? options)
                     => PagedAsync(args, options);
                 """
            ]);
        }
    }
}