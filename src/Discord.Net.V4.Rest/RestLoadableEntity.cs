using Discord.Models;

namespace Discord.Rest;

public sealed class RestLoadable<TId, TEntity, TCoreEntity, TModel>(
    DiscordRestClient client,
    IIdentifiableEntityOrModel<TId, TEntity, TModel> deferredEntity,
    IApiOutRoute<TModel> outRoute,
    Func<TId, TModel?, TEntity?> factory
):
    ILoadableEntity<TId, TCoreEntity>,
    IDisposable, IAsyncDisposable where TCoreEntity : class, IEntity<TId>
    where TEntity : class, IEntity<TId>, TCoreEntity
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    public TId Id
    {
        get => deferredEntity.Id;
        internal set
        {
            deferredEntity = (IdentifiableEntityOrModel<TId, TEntity, TModel>)value;
            if (Value is not null && !Value.Id.Equals(value))
                Value = null;
        }
    }

    public TEntity? Value
    {
        get => _value;
        set => _value = value;
    }

    private TEntity? _value = deferredEntity.Entity;

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
        IIdentifiableEntityOrModel<TId, TEntity, TModel> deferredEntity,
        Func<TId, IApiOutRoute<TModel>> route
    )
        where TConstructable : TEntity, IConstructable<TConstructable, TModel, DiscordRestClient>
        => FromConstructable<TConstructable>(client, deferredEntity, route(deferredEntity.Id));

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromConstructable<TConstructable>(
        DiscordRestClient client,
        IIdentifiableEntityOrModel<TId, TEntity, TModel> deferredEntity,
        IApiOutRoute<TModel> outRoute
    )
        where TConstructable : TEntity, IConstructable<TConstructable, TModel, DiscordRestClient>
    {
        return new RestLoadable<TId, TEntity, TCoreEntity, TModel>(
            client,
            deferredEntity,
            outRoute,
            (_, model) => model is null ? null : TConstructable.Construct(client, model)
        );
    }

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromContextConstructable<TConstructable, TContext>(
        DiscordRestClient client,
        IIdentifiableEntityOrModel<TId, TEntity, TModel> deferredEntity,
        Func<TContext, TId, IApiOutRoute<TModel>> route,
        TContext context
    )
        where TConstructable : TEntity, IContextConstructable<TConstructable, TModel, TContext, DiscordRestClient>
        => FromContextConstructable<TConstructable, TContext>(client, deferredEntity, route(context, deferredEntity.Id), context);

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromContextConstructable<TConstructable, TContext>(
        DiscordRestClient client,
        IIdentifiableEntityOrModel<TId, TEntity, TModel> deferredEntity,
        IApiOutRoute<TModel> outRoute,
        TContext context
    )
        where TConstructable : TEntity, IContextConstructable<TConstructable, TModel, TContext, DiscordRestClient>
    {
        return new RestLoadable<TId, TEntity, TCoreEntity, TModel>(
            client,
            deferredEntity,
            outRoute,
            (_, model) => model is null ? null : TConstructable.Construct(client, model, context)
        );
    }

    public void Dispose()
    {
        Value = null;
    }

    public async ValueTask DisposeAsync() => await client.DisposeAsync();
}
