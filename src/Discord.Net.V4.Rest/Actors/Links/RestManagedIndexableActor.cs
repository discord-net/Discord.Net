using Discord.Models;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest;

// internal static partial class RestManagedIndexableActor
// {
//     public static RestManagedIndexableLink<TActor, TId, TEntity, TCore, TModel> Create<
//         [TransitiveFill] TActor,
//         TId,
//         TEntity,
//         [Not(nameof(TEntity)), Interface] TCore,
//         TModel
//     >(
//         Template<TActor> template,
//         DiscordRestClient client,
//         IEnumerable<TModel> models,
//         Func<DiscordRestClient, TModel, TEntity> factory,
//         RestIndexableLink<TActor, TId, TEntity> indexableLink
//     )
//         where TActor : class, IRestActor<TId, TEntity>
//         where TEntity : RestEntity<TId>, TCore, IProxied<TActor>, IEntityOf<TModel>
//         where TId : IEquatable<TId>
//         where TCore : class, IEntity<TId, TModel>
//         where TModel : class, IEntityModel<TId>
//     {
//         return new RestManagedIndexableLink<TActor, TId, TEntity, TCore, TModel>(
//             client,
//             models,
//             factory,
//             indexableLink
//         );
//     }
// }
//
// public sealed class RestFetchableManagedIndexableLink<TActor, TId, TEntity, TCore, TModel, TApi>(
//     DiscordRestClient client,
//     IEnumerable<TModel> models,
//     Func<DiscordRestClient, TModel, TEntity> factory,
//     RestIndexableLink<TActor, TId, TEntity> indexableLink,
//     IApiOutRoute<TApi> fetchRoute,
//     Func<TApi, IEnumerable<TModel>> mapper
// ) :
//     RestManagedIndexableLink<TActor, TId, TEntity, TCore, TModel>(client, models, factory, indexableLink)
//     where TActor : class, IRestActor<TId, TEntity>
//     where TEntity : RestEntity<TId>, TCore, IProxied<TActor>, IEntityOf<TModel>
//     where TId : IEquatable<TId>
//     where TCore : class, IEntity<TId, TModel>
//     where TModel : class, IEntityModel<TId>
//     where TApi : class
// {
//     public async Task<IReadOnlyCollection<TEntity>> FetchAllAsync(
//         RequestOptions? options = null,
//         CancellationToken token = default)
//     {
//         var result = await Client.RestApiClient.ExecuteAsync(
//             fetchRoute,
//             options ?? Client.DefaultRequestOptions,
//             token
//         );
//
//         if (result is null)
//         {
//             Update(Array.Empty<TId>());
//             return [];
//         }
//
//         Update(mapper(result));
//         return Entities;
//     }
// }
//
// public class RestManagedIndexableLink<TActor, TId, TEntity, TCore, TModel> :
//     IDefinedIndexableLink<TActor, TId, TCore>,
//     IReadOnlyDictionary<TId, TEntity>
//     where TActor : class, IRestActor<TId, TEntity>
//     where TEntity : RestEntity<TId>, TCore, IProxied<TActor>, IEntityOf<TModel>
//     where TId : IEquatable<TId>
//     where TCore : class, IEntity<TId, TModel>
//     where TModel : class, IEntityModel<TId>
// {
//     public IReadOnlyDictionary<TId, TEntity> All { get; private set; }
//
//     public IReadOnlyCollection<TId> Ids
//         => _ids ??= All.Keys.ToList().AsReadOnly();
//
//     public IReadOnlyCollection<TEntity> Entities
//         => _entities ??= All.Values.ToList().AsReadOnly();
//
//     private IReadOnlyCollection<TId>? _ids;
//     private IReadOnlyCollection<TEntity>? _entities;
//
//     protected readonly DiscordRestClient Client;
//     private readonly Func<DiscordRestClient, TModel, TEntity> _factory;
//     private readonly RestIndexableLink<TActor, TId, TEntity> _indexableLink;
//
//     public RestManagedIndexableLink(
//         DiscordRestClient client,
//         IEnumerable<TModel> models,
//         Func<DiscordRestClient, TModel, TEntity> factory,
//         RestIndexableLink<TActor, TId, TEntity> indexableLink)
//     {
//         Client = client;
//         _factory = factory;
//         _indexableLink = indexableLink;
//
//         All = models.ToDictionary(x => x.Id, x => factory(client, x)).AsReadOnly();
//     }
//
//     internal void Update(IEnumerable<TModel> models)
//     {
//         All = models.ToDictionary(x => x.Id, x => _factory(Client, x)).AsReadOnly();
//         _ids = null;
//         _entities = null;
//     }
//
//     internal void Update(IEnumerable<TId> ids)
//     {
//         All = ImmutableDictionary<TId, TEntity>.Empty;
//         _ids = ids.ToImmutableList();
//         _entities = null;
//     }
//
//     public TActor Specifically(TId id) => _indexableLink.Specifically(id);
//
//     public IEnumerable<TActor> Specifically(IEnumerable<TId> ids)
//         => ids.Select(_indexableLink.Specifically);
//
//     public IEnumerator<KeyValuePair<TId, TEntity>> GetEnumerator() => All.GetEnumerator();
//
//     IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//
//     public int Count => All.Count;
//
//     public bool ContainsKey(TId key)
//         => All.ContainsKey(key);
//
//     public bool TryGetValue(TId key, [MaybeNullWhen(false)] out TEntity value)
//         => All.TryGetValue(key, out value);
//
//     public TEntity this[TId key] => All[key];
//
//     IEnumerable<TId> IReadOnlyDictionary<TId, TEntity>.Keys => Ids;
//     IEnumerable<TEntity> IReadOnlyDictionary<TId, TEntity>.Values => Entities;
// }
