using Discord.Models;

namespace Discord.Rest;

public class RestLoadableEntity<TId, TEntity, TModel, TCommon> : RestLoadableEntity<TId, TEntity, TCommon>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>, IEntity<TId>, TCommon, IModeled<TId, TModel>
    where TCommon : class, IEntity<TId>
    where TModel : class, IEntityModel<TId>
{
    internal RestLoadableEntity(DiscordRestClient client, TId id, LoadEntity loadFunc) : base(client, id, loadFunc)
    { }

    internal static RestLoadableEntity<TId, TEntity, TModel, TCommon> Create<T>(DiscordRestClient client, TId id,
        ApiRoute<T> route, Func<DiscordRestClient, TModel, TEntity> factory)
        where T : class, TModel
    {
        return new RestLoadableEntity<TId, TEntity, TModel, TCommon>(
            client,
            id,
            async (delegateClient, _, options, token) =>
            {
                var result = await delegateClient.ApiClient.ExecuteAsync(route, options, token);

                return result is null ? null : factory(client, result);
            }
        );
    }
}

public class RestLoadableEntity<TId, TEntity, TCommon> : ILoadableEntity<TId, TCommon>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>, IEntity<TId>, TCommon
    where TCommon : class, IEntity<TId>
{
    internal delegate Task<TEntity?> LoadEntity(DiscordRestClient client, TId id, RequestOptions options,
        CancellationToken token);

    public TId Id { get; }
    public TEntity? Value { get; }

    private readonly DiscordRestClient _client;
    private readonly LoadEntity _loadFunc;

    internal RestLoadableEntity(DiscordRestClient client, TId id, LoadEntity loadFunc)
    {
        Id = id;
        Value = null;
        _loadFunc = loadFunc;
        _client = client;
    }

    public ValueTask<TEntity?> LoadAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        return new ValueTask<TEntity?>(_loadFunc(_client, Id, options ?? _client.DefaultRequestOptions, token));
    }

    TCommon? ILoadableEntity<TCommon>.Value => Value;

    async ValueTask<TCommon?> ILoadableEntity<TCommon>.LoadAsync(RequestOptions? options, CancellationToken token)
        => await LoadAsync(options, token);
}
