using Discord.Models;

namespace Discord.Rest;

public sealed class RestLoadable<TId, TEntity, TCoreEntity, TModel>(
    DiscordRestClient client,
    TId id,
    IApiOutRoute<TModel> outRoute,
    Func<TId, TModel?, TEntity?> factory,
    TModel? cachedModel = null
):
    ILoadableEntity<TId, TCoreEntity>,
    IDisposable, IAsyncDisposable where TCoreEntity : class, IEntity<TId>
    where TEntity : class, IEntity<TId>, TCoreEntity
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel
{
    public TId Id
    {
        get => id;
        internal set
        {
            id = value;
            if (Value is not null && !Value.Id.Equals(value))
                Value = null;
        }
    }

    public TEntity? Value
    {
        get
        {
            if (_value is not null)
                return _value;

            if (cachedModel is null) return null;

            // model is only valid only once
            _value = factory(id, cachedModel);
            cachedModel = null;
            return _value;

        }
        set => _value = value;
    }

    private TEntity? _value;

    public async ValueTask<TEntity?> FetchAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        var model = await client.ApiClient.ExecuteAsync(outRoute, options ?? client.DefaultRequestOptions, token);

        return Value = factory(Id, model);
    }

    async ValueTask<TCoreEntity?> ILoadableEntity<TCoreEntity>.LoadAsync(RequestOptions? options,
        CancellationToken token)
        => await FetchAsync(options, token);

    TCoreEntity? ILoadableEntity<TCoreEntity>.Value => Value;

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromConstructable<TConstructable>(
        DiscordRestClient client,
        TId id,
        Func<TId, IApiOutRoute<TModel>> route,
        TModel? value = null
    )
        where TConstructable : TEntity, IConstructable<TConstructable, TModel, DiscordRestClient>
        => FromConstructable<TConstructable>(client, id, route(id), value);

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromConstructable<TConstructable>(
        DiscordRestClient client,
        TId id,
        IApiOutRoute<TModel> outRoute,
        TModel? value = null
    )
        where TConstructable : TEntity, IConstructable<TConstructable, TModel, DiscordRestClient>
    {
        return new RestLoadable<TId, TEntity, TCoreEntity, TModel>(
            client,
            id,
            outRoute,
            (_, model) => model is null ? null : TConstructable.Construct(client, model),
            value
        );
    }

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromContextConstructable<TConstructable, TContext>(
        DiscordRestClient client,
        TId id,
        Func<TContext, TId, IApiOutRoute<TModel>> route,
        TContext context,
        TModel? value = null
    )
        where TConstructable : TEntity, IContextConstructable<TConstructable, TModel, TContext, DiscordRestClient>
        => FromContextConstructable<TConstructable, TContext>(client, id, route(context, id), context, value);

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromContextConstructable<TConstructable, TContext>(
        DiscordRestClient client,
        TId id,
        IApiOutRoute<TModel> outRoute,
        TContext context,
        TModel? value = null
    )
        where TConstructable : TEntity, IContextConstructable<TConstructable, TModel, TContext, DiscordRestClient>
    {
        return new RestLoadable<TId, TEntity, TCoreEntity, TModel>(
            client,
            id,
            outRoute,
            (_, model) => model is null ? null : TConstructable.Construct(client, model, context),
            value
        );
    }

    public void Dispose()
    {
        Value = null;
    }

    public async ValueTask DisposeAsync() => await client.DisposeAsync();
}
