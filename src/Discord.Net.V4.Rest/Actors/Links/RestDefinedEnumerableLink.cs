using Discord.Models;

namespace Discord.Rest;

// public sealed class RestDefinedEnumerableLink<TActor, TId, TEntity, TCore, TModel> :
//     IRestClientProvider,
//     IDefinedEnumerableIndexableLink<TActor, TId, TCore, TModel>
//     where TActor :
//     class,
//     IRestActor<TId, TEntity>,
//     IActor<TId, TCore>,
//     IEntityProvider<TEntity, TModel>,
//     IEntityProvider<TCore, TModel>
//     where TId : IEquatable<TId>
//     where TEntity : RestEntity<TId>, IEntity<TId, TModel>, IRestConstructableEntity<TEntity, TActor, TModel>
//     where TCore : class, IEntity<TId, TModel>
//     where TModel : IEntityModel<TId>
// {
//     public DiscordRestClient Client { get; }
//
//     public IReadOnlyCollection<TId> Ids => DefinedLink.Ids;
//     
//     internal RestDefinedLink<TActor, TId, TEntity, TModel> DefinedLink { get; }
//     internal RestEnumerableLink<TActor, TId, TEntity, TCore, TModel> EnumerableLink { get; }
//     
//     public RestDefinedEnumerableLink(
//         DiscordRestClient client,
//         RestIndexableLink<TActor, TId, TEntity, TModel> indexableLink,
//         ApiModelProviderDelegate<IEnumerable<TModel>> apiProvider,
//         IReadOnlyCollection<TId> ids)
//     {
//         Client = client;
//         
//         DefinedLink = new(client, indexableLink, ids);
//         EnumerableLink = new(client, indexableLink, apiProvider);
//     }
//     
//     public ValueTask<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null, CancellationToken token = default)
//     {
//         throw new NotImplementedException();
//     }
//     
//     async ValueTask<IReadOnlyCollection<TCore>> IEnumerableLink<TActor, TId, TCore, TModel>.AllAsync(
//         RequestOptions? options,
//         CancellationToken token)
//     {
//         var result = await AllAsync(options, token);
//
//         if (result is IReadOnlyCollection<TCore> core) return core;
//
//         return result.Cast<TCore>().ToList().AsReadOnly();
//     }
//
//     public TActor Specifically(TId id)
//         => Index
//
//     public IEnumerable<TActor> Specifically(IEnumerable<TId> ids)
//     {
//         throw new NotImplementedException();
//     }
// }