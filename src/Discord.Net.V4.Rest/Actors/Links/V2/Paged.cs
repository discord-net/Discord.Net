using Discord.Models;

namespace Discord.Rest;

public partial class RestLinkTypeV2<TActor, TId, TEntity, TModel>
{
    public partial class Paged<TParams>(
        DiscordRestClient client,
        IActorProvider<TActor, TId> actorProvider,
        IPagingProvider<TParams, TEntity, TModel> pagingProvider
    ) :
        RestLinkV2<TActor, TId, TEntity, TModel>(client, actorProvider),
        ILinkType<TActor, TId, TEntity, TModel>.Paged<TParams>
        where TParams : class, IPagingParams<TParams, IEnumerable<TModel>>
    {
        public IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null)
            => pagingProvider.CreatePagedAsync(args, options);
    }
    
    public partial class Paged<TPaged, TPagedModel, TParams>(
        DiscordRestClient client,
        IActorProvider<TActor, TId> actorProvider,
        IPagingProvider<TParams, TPaged, TPagedModel> pagingProvider
    ) :
        RestLinkV2<TActor, TId, TEntity, TModel>(client, actorProvider),
        ILinkType<TActor, TId, TEntity, TModel>.Paged<TPaged, TParams>
        where TParams : class, IPagingParams<TParams, IEnumerable<TModel>>
        where TPaged : IEntity<TId, TPagedModel>
        where TPagedModel : IEntityModel<TId>
        
    {
        public IAsyncPaged<TPaged> PagedAsync(TParams? args = default, RequestOptions? options = null)
            => pagingProvider.CreatePagedAsync(args, options);
    }

    // public partial class Paged<TParams, TPageModel>(
    //     DiscordRestClient client,
    //     IActorProvider<TActor, TId> actorProvider,
    //     IPathable path,
    //     Func<TPageModel, IEnumerable<TModel>> mapper,
    //     Func<TModel, TPageModel, TEntity>? entityFactory = null
    // ) :
    //     RestLinkV2<TActor, TId, TEntity, TModel>(client, actorProvider),
    //     ILinkType<TActor, TId, TEntity, TModel>.Paged<TParams>
    //     where TParams : class, IPagingParams<TParams, TPageModel>
    //     where TPageModel : class
    // {
    //     public IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null)
    //     {
    //         return new RestPager<TId, TEntity, TModel, TPageModel, TParams>(
    //             Client,
    //             path,
    //             options ?? Client.DefaultRequestOptions,
    //             mapper,
    //             entityFactory ?? Factory,
    //             args
    //         );
    //     }
    //
    //     private TEntity Factory(TModel model, TPageModel pageModel)
    //         => CreateEntity(model);
    // }
    //
    // public partial class Paged<TPaged, TPagedModel, TParams, TApiModel>(
    //     DiscordRestClient client,
    //     IActorProvider<TActor, TId> actorProvider,
    //     IPathable path,
    //     Func<TApiModel, IEnumerable<TPagedModel>> mapper
    // ) :
    //     RestLinkV2<TActor, TId, TEntity, TModel>(client, actorProvider),
    //     ILinkType<TActor, TId, TEntity, TModel>.Paged<TPaged, TParams>
    //     where TParams : class, IPagingParams<TParams, TApiModel>
    //     where TApiModel : class
    //     where TPaged : class, IEntity<TId, TPagedModel>, IRestConstructable<TPaged, TActor, TPagedModel>
    //     where TPagedModel : class, IEntityModel<TId>
    // {
    //     public IAsyncPaged<TPaged> PagedAsync(TParams? args = default, RequestOptions? options = null)
    //     {
    //         return new RestPager<TId, TPaged, TPagedModel, TApiModel, TParams>(
    //             Client,
    //             path,
    //             options ?? Client.DefaultRequestOptions,
    //             mapper,
    //             (model, _) => TPaged.Construct(Client, GetActor(model.Id), model),
    //             args
    //         );
    //     }
    // }
}