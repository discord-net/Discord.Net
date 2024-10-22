// using Discord.Net.Hanz.Tasks.Actors.V3;
// using Microsoft.CodeAnalysis;
// using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;
//
// namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
//
// public class PagedNode(LinkTarget target, LinkSchematics.Entry entry) : LinkTypeNode(target, entry)
// {
//     public bool PagesEntity => Entry.Symbol.TypeArguments.Length == 1;
//
//     public ITypeSymbol PagedEntityType
//         => Entry.Symbol.TypeArguments.Length == 2
//             ? Entry.Symbol.TypeParameters[0]
//             : Target.Entity;
//
//     public string PagingProviderType
//         => $"Func<{Target.Actor}.{GetTypeName()}, TParams?, RequestOptions?, IAsyncPaged<{PagedEntityType}>>";// $"Discord.IPagedLinkProvider<{PagedEntityType}, TParams>";
//
//     private protected override void Visit(NodeContext context, Logger logger)
//     {
//         Properties.Clear();
//         Properties.Add(new("PagingProvider", PagingProviderType));
//         
//         base.Visit(context, logger);
//         
//         RedefinesLinkMembers = Ancestors.Count > 0;
//     }
//
//     public override string Build(NodeContext context, Logger logger)
//     {
//         AddDefaultProvider();
//         
//         return base.Build(context, logger);
//     }
//
//     protected override void AddMembers(List<string> members, NodeContext context, Logger logger)
//     {
//         if (!IsTemplate || !RedefinesLinkMembers) return;
//         
//         members.AddRange([
//             $"new IAsyncPaged<{PagedEntityType}> PagedAsync(TParams? args = default, RequestOptions? options = null);",
//             $"IAsyncPaged<{PagedEntityType}> {FormattedLinkType}.{LinksV4.FormatTypeName(Entry.Symbol)}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);",
//         ]);
//
//         if (ParentLinks.Any())
//         {
//             members.AddRange([
//                 $"IAsyncPaged<{PagedEntityType}> {Target.Actor}.{LinksV4.FormatTypeName(Entry.Symbol)}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);"
//             ]);
//         }
//
//         foreach (var ancestor in Ancestors)
//         {
//             var overrideType =
//                 $"{(
//                     ancestor.Ancestors.Count > 0
//                         ? $"{ancestor.Target.Actor}{FormatRelativeTypePath()}"
//                         : ancestor.FormattedLinkType
//                 )}.{LinksV4.FormatTypeName(Entry.Symbol)}";
//
//             var ancestorPagedType = PagesEntity ? ancestor.Target.Entity.ToDisplayString() : "TPaged";
//
//             members.AddRange([
//                 $"IAsyncPaged<{ancestorPagedType}> {overrideType}.PagedAsync(TParams? args, RequestOptions? options) => PagedAsync(args, options);"
//             ]);
//         }
//     }
//
//     protected override void CreateImplementation(
//         List<string> members,
//         List<string> bases,
//         NodeContext context,
//         Logger logger)
//     {
//         var memberModifier = ImplementationBase is not null
//             ? "override "
//             : ImplementationChild is not null
//                 ? "virtual "
//                 : string.Empty;
//         
//         members.Add(
//             $"""
//              public {memberModifier}IAsyncPaged<{PagedEntityType}> PagedAsync(TParams? args = default, RequestOptions? options = null)
//                  => PagingProvider(this, args, options);
//              """
//         );
//
//         if (RedefinesLinkMembers)
//         {
//             members.Add(
//                 $"""
//                  IAsyncPaged<{PagedEntityType}> {FormatAsTypePath()}.PagedAsync(TParams? args, RequestOptions? options)
//                      => PagedAsync(args, options);
//                  """
//             );
//         }
//         else
//         {
//             var corePagedEntityType = PagesEntity ? Target.GetCoreEntity() : PagedEntityType;
//
//             members.AddRange([
//                 $"""
//                  IAsyncPaged<{PagedEntityType}> {FormattedLinkType}.{GetTypeName()}.PagedAsync(TParams? args, RequestOptions? options)
//                      => PagedAsync(args, options);
//                  """,
//                 $"""
//                  IAsyncPaged<{corePagedEntityType}> {FormattedCoreLinkType}.{GetTypeName()}.PagedAsync(TParams? args, RequestOptions? options)
//                      => PagedAsync(args, options);
//                  """
//             ]);
//         }
//     }
//
//     private void AddDefaultProvider()
//     {
//         if (Parent is not ActorNode || RootActorNode is null || IsCore || !PagesEntity) return;
//
//         if (
//             Target.GetCoreEntity()
//                 .Interfaces
//                 .FirstOrDefault(x =>
//                     x.Name is "IPagedFetchableOfMany"
//                 ) is not { } fetchable
//         ) return;
//
//         if (fetchable.TypeArguments[1] is not INamedTypeSymbol routeModel)
//             return;
//
//         if (
//             !routeModel.Equals(Target.Model, SymbolEqualityComparer.Default)
//             // &&
//             //!Hierarchy.Implements(Target.Model, routeModel)
//         ) return;
//         
//         RootActorNode.AdditionalTypes.Add(
//             $$"""
//             internal static new IAsyncPaged<{{Target.Entity}}> DefaultPagingProvider(
//                 {{Target.Actor}}.Paged<{{fetchable.TypeArguments[2]}}> link,
//                 {{fetchable.TypeArguments[2]}}? args,
//                 RequestOptions? options
//             )    
//             {
//                 return new RestPager<{{Target.Entity}}, IEnumerable<{{Target.Model}}>, {{fetchable.TypeArguments[2]}}>(
//                     link.Client,
//                     models => models.Select(link.CreateEntity),
//                     args,
//                     link,
//                     options
//                 );
//             }
//             """
//         );
//     }
// }