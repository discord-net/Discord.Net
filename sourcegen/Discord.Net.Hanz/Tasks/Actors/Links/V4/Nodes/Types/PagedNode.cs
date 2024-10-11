using Discord.Net.Hanz.Tasks.Actors.V3;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;

public class PagedNode(LinkTarget target, LinkSchematics.Entry entry) : LinkTypeNode(target, entry)
{
    protected override void AddMembers(List<string> members, NodeContext context, Logger logger)
    {
        var ancestors = GetEntityAssignableAncestors(context);
        
        if (ancestors.Length == 0) return;
        
        var pagedType = Entry.Symbol.TypeArguments.Length == 1
            ? Target.Entity.ToDisplayString()
            : "TPaged";
        
        members.AddRange([
            $"new IAsyncPaged<{pagedType}> PagedAsync(TParams? args = default, RequestOptions? options = null);",
            $"IAsyncPaged<{pagedType}> {FormattedLinkType}.{LinksV3.FormatTypeName(Entry.Symbol)}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);",
        ]);
        
        if (ParentLinks.Any())
        {
            members.AddRange([
                $"IAsyncPaged<{pagedType}> {Target.Actor}.{LinksV3.FormatTypeName(Entry.Symbol)}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);"
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
        
            var ancestorPagedType = pagedType == "TPaged" ? "TPaged" : ancestor.Target.Entity.ToDisplayString();
        
            members.AddRange([
                $"IAsyncPaged<{ancestorPagedType}> {overrideType}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);"
            ]);
        }
    }

    protected override string CreateImplementation(NodeContext context, Logger logger)
    {
        return string.Empty;
    }
}