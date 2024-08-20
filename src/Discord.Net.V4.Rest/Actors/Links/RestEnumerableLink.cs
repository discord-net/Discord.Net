using Discord.Models;

namespace Discord.Rest;

public sealed partial class RestEnumerableLink<TActor, TId, TEntity, TCore, TModel> :
    IRestClientProvider,
    IEnumerableLink<TActor, TId, TCore, TModel>
    where TEntity :
    RestEntity<TId>,
    IEntity<TId, TModel>,
    IRestConstructable<TEntity, TActor, TModel>
    where TActor :
    class,
    IRestActor<TId, TEntity>,
    IActor<TId, TCore>,
    IEntityProvider<TEntity, TModel>,
    IEntityProvider<TCore, TModel>
    where TId : IEquatable<TId>
    where TCore : class, IEntity<TId, TModel>
    where TModel : IEntityModel<TId>
{
    public DiscordRestClient Client { get; }

    private readonly RestIndexableLink<TActor, TId, TEntity, TModel> _indexableLink;
    private readonly ApiModelProviderDelegate<IEnumerable<TModel>> _apiProvider;

    public RestEnumerableLink(
        DiscordRestClient client,
        RestIndexableLink<TActor, TId, TEntity, TModel> indexableLink,
        ApiModelProviderDelegate<IEnumerable<TModel>> apiProvider)
    {
        _indexableLink = indexableLink;
        _apiProvider = apiProvider;
        Client = client;
    }

    [SourceOfTruth]
    internal TCore CreateEntity(TModel model)
        => (_indexableLink[model.Id] as IEntityProvider<TCore, TModel>).CreateEntity(model);

    public async ValueTask<IReadOnlyCollection<TEntity>> AllAsync(
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var models = await _apiProvider(Client, options, token);

        return models
            .Select(model =>
                TEntity.Construct(Client, _indexableLink[model.Id], model)
            )
            .ToList()
            .AsReadOnly();
    }

    async ValueTask<IReadOnlyCollection<TCore>> IEnumerableLink<TActor, TId, TCore, TModel>.AllAsync(
        RequestOptions? options,
        CancellationToken token)
    {
        var result = await AllAsync();

        if (result is IReadOnlyCollection<TCore> core) return core;

        return result.OfType<TCore>().ToList().AsReadOnly();
    }
}