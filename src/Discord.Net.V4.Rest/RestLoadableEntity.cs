using Discord.Models;

namespace Discord.Rest;

public sealed class RestLoadable<TId, TEntity, TCoreEntity, TModel>(
    DiscordRestClient client,
    IIdentifiableEntityOrModel<TId, TEntity, TModel> identity,
    IApiOutRoute<TModel> outRoute,
    RestLoadable<TId, TEntity, TCoreEntity, TModel>.EntityFactory factory
):
    ILoadableEntity<TId, TCoreEntity>,
    IDisposable, IAsyncDisposable
    where TCoreEntity : class, IEntity<TId>
    where TEntity : class, IEntity<TId>, IEntityOf<TModel>, TCoreEntity
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    public delegate TEntity? EntityFactory(DiscordRestClient client, TId id, TModel? model);

    public TId Id
    {
        get => Identity.Id;
        internal set
        {
            if (value.Equals(Identity.Id)) return;

            identity = new IdentifiableEntityOrModel<TId, TEntity, TModel>(value);
            Value = null;
        }
    }

    public TEntity? Value
    {
        get => _cachedEntityValue ??= Identity.Entity;
        set
        {
            _cachedEntityValue = value;

            if (value is null)
            {
                identity = IIdentifiableEntityOrModel<TId, TEntity, TModel>.Of(identity.Id);
            }
        }
    }

    internal IIdentifiableEntityOrModel<TId, TEntity, TModel> Identity
    {
        get => identity;
        set => identity = value;
    }

    private TEntity? _cachedEntityValue;

    public async ValueTask<TEntity?> FetchAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        var model = await client.ApiClient.ExecuteAsync(outRoute, options ?? client.DefaultRequestOptions, token);

        return Value = factory(client, Id, model);
    }

    async ValueTask<TCoreEntity?> ILoadableEntity<TCoreEntity>.LoadAsync(RequestOptions? options,
        CancellationToken token)
        => await FetchAsync(options, token);

    TCoreEntity? ILoadableEntity<TCoreEntity>.Value => Value;

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromConstructable<TConstructable>(
        DiscordRestClient client,
        IIdentifiableEntityOrModel<TId, TEntity, TModel> identity,
        Func<TId, IApiOutRoute<TModel>> route
    )
        where TConstructable : TEntity, IConstructable<TConstructable, TModel, DiscordRestClient>
        => FromConstructable<TConstructable>(client, identity, route(identity.Id));

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromConstructable<TConstructable>(
        DiscordRestClient client,
        IIdentifiableEntityOrModel<TId, TEntity, TModel> identity,
        IApiOutRoute<TModel> outRoute
    )
        where TConstructable : TEntity, IConstructable<TConstructable, TModel, DiscordRestClient>
    {
        return new RestLoadable<TId, TEntity, TCoreEntity, TModel>(
            client,
            identity,
            outRoute,
            static (client, _, model) => model is null ? null : TConstructable.Construct(client, model)
        );
    }

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromContextConstructable<TConstructable, TContext>(
        DiscordRestClient client,
        IIdentifiableEntityOrModel<TId, TEntity, TModel> identity,
        Func<TContext, TId, IApiOutRoute<TModel>> route,
        TContext context
    )
        where TConstructable : TEntity, IContextConstructable<TConstructable, TModel, TContext, DiscordRestClient>
        => FromContextConstructable<TConstructable, TContext>(client, identity, route(context, identity.Id), context);

    public static RestLoadable<TId, TEntity, TCoreEntity, TModel> FromContextConstructable<TConstructable, TContext>(
        DiscordRestClient client,
        IIdentifiableEntityOrModel<TId, TEntity, TModel> identity,
        IApiOutRoute<TModel> outRoute,
        TContext context
    )
        where TConstructable : TEntity, IContextConstructable<TConstructable, TModel, TContext, DiscordRestClient>
    {
        return new RestLoadable<TId, TEntity, TCoreEntity, TModel>(
            client,
            identity,
            outRoute,
            (client, _, model) => model is null ? null : TConstructable.Construct(client, model, context)
        );
    }

    public void Dispose()
    {
        Value = null;
    }

    public async ValueTask DisposeAsync() => await client.DisposeAsync();
}
