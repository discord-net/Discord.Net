using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Types;

public class Paged : ILinkTypeProcessor
{
    public ConstructorRequirements AddImplementation(
        List<string> members,
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path)
    {
        // var pagedFetchableOfManyInterface = target.LinkTarget.GetCoreEntity()
        //     .AllInterfaces
        //     .FirstOrDefault(x => x.Name == "IPagedFetchableOfMany");
        //
        // var canUsePagedFetchable =
        //     pagedFetchableOfManyInterface?
        //         .TypeArguments
        //         .ElementAtOrDefault(1)?
        //         .Equals(target.LinkTarget.Model, SymbolEqualityComparer.Default)
        //     ?? false;

        var pagedType = path.Add(type).First(x => x.Symbol.Name == "Paged");
        
        var overrideTarget = target.EntityAssignableAncestors.Count > 0
            ? $"{target.LinkTarget.GetCoreActor()}{LinksV3.FormatPath(path.Add(type))}"
            : $"{target.FormattedCoreLinkType}.{LinksV3.FormatTypeName(pagedType.Symbol)}";

        var pagingType = pagedType.Symbol.TypeArguments.Length switch
        {
            2 => "TPaged",
            _ => target.LinkTarget.Entity.ToDisplayString()
        };

        var pagingProviderType = $"Discord.IPagedLinkProvider<{pagingType}, TParams>";
        
        members.AddRange([
            $$"""
              public IAsyncPaged<{{pagingType}}> PagedAsync(TParams? args = default, RequestOptions? options = null)
                  => _pagingProvider.PagedAsync(args, options);
              """,
            $"IAsyncPaged<{(pagingType == "TPaged" ? pagingType : target.LinkTarget.GetCoreEntity().ToDisplayString())}> {overrideTarget}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);",
            $"private readonly {pagingProviderType} _pagingProvider;"
        ]);

        return new ConstructorRequirements().Require("_pagingProvider", pagingProviderType);

//         var pagedFetchableOfManyInterface = target.LinkTarget.GetCoreEntity()
//             .AllInterfaces
//             .FirstOrDefault(x => x.Name == "IPagedFetchableOfMany");
//
//         var canUseFetchable =
//             pagedFetchableOfManyInterface?
//                 .TypeArguments
//                 .ElementAtOrDefault(1)?
//                 .Equals(target.LinkTarget.Model, SymbolEqualityComparer.Default)
//             ?? false;
//
//         var overrideTarget = target.EntityAssignableAncestors.Count > 0
//             ? $"{target.LinkTarget.GetCoreActor()}{LinksV3.FormatPath(path.Add(type))}"
//             : $"{target.FormattedCoreLinkType}.Paged";
//
//         var apiProviderType = $"ApiModelProviderDelegate<IEnumerable<{target.LinkTarget.Model}>>";
//
//         var extraArgs = GetExtraParameters(target);
//         var formattedExtraArgs = FormatExtraParameters(extraArgs);
//         var formattedInvocationParameters = extraArgs.Count == 0
//             ? string.Empty
//             : $", {string.Join(", ", extraArgs.Select(x => $"{x.Name}: {x.Name}"))}";
//
//         var fetchModels = canUseFetchable
//             ? $"await {target.LinkTarget.GetCoreEntity()}.FetchManyRoute(_path{formattedInvocationParameters}).AsRequiredProvider()(Client, options, token);"
//             : "await _apiProvider(Client, options, token);";
//
//         members.AddRange([
//             canUseFetchable 
//                 ? "private readonly IPathable _path;"
//                 : $"private readonly {apiProviderType} _apiProvider;",
//             $$"""
//               public async ITask<IReadOnlyCollection<{{target.LinkTarget.Entity}}>> AllAsync({{formattedExtraArgs}}RequestOptions? options = null, CancellationToken token = default)
//               {
//                   var models = {{fetchModels}}
//                   
//                   if (models is null) return [];
//                   
//                   return models.Select(CreateEntity).ToList().AsReadOnly();
//               }
//               """,
//             $"ITask<IReadOnlyCollection<{target.LinkTarget.GetCoreEntity()}>> {overrideTarget}.AllAsync(RequestOptions? options, CancellationToken token) => AllAsync(options, token);"
//         ]);
    }

    public void AddOverrideMembers(List<string> members, LinksV3.Target target, LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path)
    {
        if (target.EntityAssignableAncestors.Count == 0) return;

        var pagedType = type.Symbol.TypeArguments.Length == 1
            ? target.LinkTarget.Entity.ToDisplayString()
            : "TPaged";

        members.AddRange([
            $"new IAsyncPaged<{pagedType}> PagedAsync(TParams? args = default, RequestOptions? options = null);",
            $"IAsyncPaged<{pagedType}> {target.FormattedLinkType}.{LinksV3.FormatTypeName(type.Symbol)}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);",
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
                : $"{ancestor.FormattedLinkType}.{LinksV3.FormatTypeName(type.Symbol)}";

            var ancestorPagedType = pagedType == "TPaged" ? "TPaged" : ancestor.LinkTarget.Entity.ToDisplayString();

            members.AddRange([
                $"IAsyncPaged<{ancestorPagedType}> {overrideType}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);"
            ]);
        }
    }

    public string? CreateProvider(LinksV3.Target target, Logger logger) => null;
}