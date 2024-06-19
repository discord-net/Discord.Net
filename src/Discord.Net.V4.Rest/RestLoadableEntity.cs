using Discord.Models;

namespace Discord.Rest;

public sealed class RestLoadable<TId, TEntity, TCoreEntity, TModel>(
    DiscordRestClient client,
    TId id,
    ApiRoute<TModel> route,
    Func<TId, TModel?, TEntity?> factory
):
    ILoadableEntity<TId, TCoreEntity>,
    IDisposable, IAsyncDisposable where TCoreEntity : class, IEntity<TId>
    where TEntity : RestEntity<TId>, TCoreEntity
    where TId : IEquatable<TId>
    where TModel : class
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

    public TEntity? Value { get; private set; }

    public async ValueTask<TEntity?> FetchAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        var model = await client.ApiClient.ExecuteAsync(route, options ?? client.DefaultRequestOptions, token);

        return Value = factory(Id, model);
    }

    async ValueTask<TCoreEntity?> ILoadableEntity<TCoreEntity>.LoadAsync(RequestOptions? options,
        CancellationToken token)
        => await FetchAsync(options, token);

    TCoreEntity? ILoadableEntity<TCoreEntity>.Value => Value;

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromConstructable<TConstructable>(
        DiscordRestClient client,
        TId id,
        Func<TId, ApiRoute<TModel>> route
    )
        where TConstructable : TEntity, IConstructable<TConstructable, TModel, DiscordRestClient>
        => FromConstructable<TConstructable>(client, id, route(id));

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromConstructable<TConstructable>(
        DiscordRestClient client,
        TId id,
        ApiRoute<TModel> route
    )
        where TConstructable : TEntity, IConstructable<TConstructable, TModel, DiscordRestClient>
    {
        return new RestLoadable<TId, TEntity, TCoreEntity, TModel>(
            client,
            id,
            route,
            (id, model) => model is null ? null : TConstructable.Construct(client, model)
        );
    }

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromContextConstructable<TConstructable, TContext>(
        DiscordRestClient client,
        TId id,
        Func<TContext, TId, ApiRoute<TModel>> route,
        TContext context
    )
        where TConstructable : TEntity, IContextConstructable<TConstructable, TModel, TContext, DiscordRestClient>
        => FromContextConstructable<TConstructable, TContext>(client, id, route(context, id), context);

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromContextConstructable<TConstructable, TContext>(
        DiscordRestClient client,
        TId id,
        ApiRoute<TModel> route,
        TContext context
    )
        where TConstructable : TEntity, IContextConstructable<TConstructable, TModel, TContext, DiscordRestClient>
    {
        return new RestLoadable<TId, TEntity, TCoreEntity, TModel>(
            client,
            id,
            route,
            (id, model) => model is null ? null : TConstructable.Construct(client, model, context)
        );
    }

    public void Dispose()
    {
        Value = null;
    }

    public async ValueTask DisposeAsync() => await client.DisposeAsync();
}
