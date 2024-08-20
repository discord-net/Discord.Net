using Discord.Models;
using Discord.Paging;

namespace Discord.Rest;

public sealed class RestPagedIndexableLink<TActor, TId, TEntity, TModel, TPartial, TPartialModel, TParams, TParamsModel> : 
    IRestClientProvider,
    IPagedIndexableLink<TActor, TId, TEntity, TModel, TPartial, TParams>
    where TActor :
    class,
    IActor<TId, TEntity>, 
    IActor<TId, TPartial>, 
    IEntityProvider<TEntity, TModel>,
    IEntityProvider<TPartial, TPartialModel>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
    where TId : IEquatable<TId>
    where TPartial : RestEntity<TId>, IEntity<TId, TPartialModel>, IRestConstructable<TPartial, TActor, TPartialModel>
    where TModel : class, IEntityModel<TId>
    where TPartialModel : class, IEntityModel<TId>
    where TParams : class, IPagingParams<TParams, TParamsModel>
    where TParamsModel : class
{
    public DiscordRestClient Client { get; }

    public TActor this[TId id] => _indexableLink[id]; 
    
    private readonly RestPagedLink<TActor, TId, TPartial, TPartialModel, TParams, TParamsModel> _pagedLink;
    private readonly RestIndexableLink<TActor, TId, TEntity, TModel> _indexableLink;

    public RestPagedIndexableLink(
        DiscordRestClient client,
        IPathable path,
        RestIndexableLink<TActor, TId, TEntity, TModel> indexableLink,
        Func<TParamsModel, IEnumerable<TPartialModel>> transformer)
    {
        Client = client;
        
        _indexableLink = indexableLink;
        _pagedLink = new(client, path, new(Client, indexableLink.ActorFactory), transformer);
    }

    public IAsyncPaged<TPartial> PagedAsync(TParams? args = default, RequestOptions? options = null)
        => _pagedLink.PagedAsync(args, options);

    public TActor Specifically(TId id)
        => _indexableLink.Specifically(id);
}

public sealed class RestPagedIndexableLink<TActor, TId, TEntity, TModel, TParams, TParamsModel> : 
    IRestClientProvider,
    IPagedIndexableLink<TActor, TId, TEntity, TModel, TParams>
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
    where TParams : class, IPagingParams<TParams, TParamsModel>
    where TParamsModel : class
{
    public DiscordRestClient Client { get; }

    public TActor this[TId id] => _indexableLink[id]; 
    
    private readonly RestPagedLink<TActor, TId, TEntity, TModel, TParams, TParamsModel> _pagedLink;
    private readonly RestIndexableLink<TActor, TId, TEntity, TModel> _indexableLink;

    public RestPagedIndexableLink(
        DiscordRestClient client,
        IPathable path,
        RestIndexableLink<TActor, TId, TEntity, TModel> indexableLink,
        Func<TParamsModel, IEnumerable<TModel>> transformer)
    {
        Client = client;
        
        _indexableLink = indexableLink;
        _pagedLink = new(client, path, indexableLink, transformer);
    }

    public IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null)
        => _pagedLink.PagedAsync(args, options);

    public TActor Specifically(TId id)
        => _indexableLink.Specifically(id);
}