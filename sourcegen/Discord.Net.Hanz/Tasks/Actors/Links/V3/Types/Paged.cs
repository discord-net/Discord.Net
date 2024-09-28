using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Types;

public class Paged : ILinkTypeProcessor
{
    public void AddOverrideMembers(List<string> members, LinksV3.Target target, LinkSchematics.Entry type, ImmutableList<LinkSchematics.Entry> path)
    {
        var pagedType = type.Symbol.TypeArguments.Length == 1
            ? target.LinkTarget.Entity.ToDisplayString()
            : "TPaged";
        
        members.AddRange([
            $"new IAsyncPaged<{pagedType}> PagedAsync(TParams? args = default, RequestOptions? options = null);",
            $"IAsyncPaged<{pagedType}> {target.FormattedCoreLinkType}.{LinksV3.FormatTypeName(type.Symbol)}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);",
        ]);
        
        if (path.Count > 0)
        {
            members.AddRange([
                $"IAsyncPaged<{pagedType}> {target.LinkTarget.Actor}.{LinksV3.FormatTypeName(type.Symbol)}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);"
            ]);
        }
        
        foreach (var ancestor in target.EntityAssignableAncestors)
        {
            var overrideType = ancestor.EntityAssignableAncestors.Count > 0
                ? $"{ancestor.LinkTarget.Actor}{LinksV3.FormatPath(path.Add(type))}"
                : $"{ancestor.FormattedCoreLinkType}.{LinksV3.FormatTypeName(type.Symbol)}";

            var ancestorPagedType = pagedType == "TPaged" ? "TPaged" : ancestor.LinkTarget.Entity.ToDisplayString();
            
            members.AddRange([
                $"IAsyncPaged<{ancestorPagedType}> {overrideType}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);"
            ]);
        }
    }
    
    public string? CreateProvider(LinksV3.Target target, Logger logger) => null;
}