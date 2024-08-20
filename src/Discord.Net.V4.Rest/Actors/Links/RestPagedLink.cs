using Discord.Models;
using Discord.Paging;

namespace Discord.Rest;

public sealed partial class RestPagedLink<TActor, TId, TEntity, TModel, TParams, TParamsModel> : 
    IRestClientProvider,
    IPagedLink<TActor, TId, TEntity, TModel, TParams>
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
    where TParams : class, IPagingParams<TParams, TParamsModel>
    where TParamsModel : class
{
    public DiscordRestClient Client { get; }
    
    private readonly IPathable _path;
    private readonly RestIndexableLink<TActor, TId, TEntity, TModel> _indexableLink;
    private readonly Func<TParamsModel, IEnumerable<TModel>> _transformer;

    public RestPagedLink(
        DiscordRestClient client,
        IPathable path,
        RestIndexableLink<TActor, TId, TEntity, TModel> indexableLink,
        Func<TParamsModel, IEnumerable<TModel>> transformer)
    {
        _path = path;
        _indexableLink = indexableLink;
        _transformer = transformer;
        Client = client;
    }

    [SourceOfTruth]
    internal TEntity CreateEntity(TModel model)
        => TEntity.Construct(Client, _indexableLink[model.Id], model);

    public IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null)
    {
        return new RestPager<TId, TEntity, TModel, TParamsModel, TParams>(
            Client,
            _path,
            options ?? Client.DefaultRequestOptions,
            _transformer,
            (model, _) => TEntity.Construct(Client, _indexableLink[model.Id], model),
            args
        );
    }
}